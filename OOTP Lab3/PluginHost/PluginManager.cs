using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OOTP_Lab3.Contracts;
using OOTP_Lab3.Serialization;

namespace OOTP_Lab3.PluginHost
{
    public class PluginManager
    {
        private readonly List<IPlugin> _loadedPlugins = new List<IPlugin>();
        private readonly string _pluginsDirectory;
        private readonly TextDeserializer _deserializer;

        public IReadOnlyList<IPlugin> LoadedPlugins => _loadedPlugins;

        public event EventHandler<IPlugin> PluginLoaded;
        public event EventHandler<IDataProcessor> DataProcessorLoaded;

        public PluginManager(string pluginsDirectory = "Plugins")
        {
            _pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsDirectory);
            _deserializer = new TextDeserializer();

            if (!Directory.Exists(_pluginsDirectory))
            {
                Directory.CreateDirectory(_pluginsDirectory);
            }
        }

        public void LoadAllPlugins(IPluginHost host)
        {
            if (!Directory.Exists(_pluginsDirectory))
            {
                return;
            }

            var dllFiles = Directory.GetFiles(_pluginsDirectory, "*.dll");

            foreach (var dllPath in dllFiles)
            {
                LoadPluginFromFile(dllPath, host);
            }

            _deserializer.ScanAndRegisterPluginDeserializers();
        }

        public bool LoadPluginFromFile(string filePath, IPluginHost host)
        {
            try
            {
                var assembly = Assembly.LoadFrom(filePath);
                var pluginTypes = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var pluginType in pluginTypes)
                {
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType);
                    plugin.Initialize(host);
                    _loadedPlugins.Add(plugin);
                    PluginLoaded?.Invoke(this, plugin);

                    // Check if this is also a data processor
                    if (plugin is IDataProcessor dataProcessor)
                    {
                        host.RegisterDataProcessor(dataProcessor);
                        DataProcessorLoaded?.Invoke(this, dataProcessor);
                    }

                    System.Diagnostics.Debug.WriteLine($"Loaded plugin: {plugin.PluginName} v{plugin.Version}");
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load plugin from {filePath}: {ex.Message}");
                return false;
            }
        }

        public bool UnloadPlugin(IPlugin plugin)
        {
            try
            {
                plugin.Shutdown();
                _loadedPlugins.Remove(plugin);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to unload plugin {plugin.PluginName}: {ex.Message}");
                return false;
            }
        }

        public bool LoadPluginFromUserSelectedFile(IPluginHost host)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Plugin Files (*.dll)|*.dll",
                Title = "Select Plugin DLL"
            };

            if (dialog.ShowDialog() == true)
            {
                return LoadPluginFromFile(dialog.FileName, host);
            }
            return false;
        }
    }
}