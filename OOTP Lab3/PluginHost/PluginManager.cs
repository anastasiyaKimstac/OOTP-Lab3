using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OOTP_Lab3.Contracts;
using OOTP_Lab3.Serialization;

namespace OOTP_Lab3.PluginHost
{
    /// <summary>
    /// Manages dynamic loading and unloading of plugins
    /// </summary>
    public class PluginManager
    {
        private readonly List<IPlugin> _loadedPlugins = new List<IPlugin>();
        private readonly string _pluginsDirectory;
        private readonly TextDeserializer _deserializer;

        public IReadOnlyList<IPlugin> LoadedPlugins => _loadedPlugins;

        /// <summary>
        /// Event fired when a plugin is loaded
        /// </summary>
        public event EventHandler<IPlugin> PluginLoaded;

        public PluginManager(string pluginsDirectory = "Plugins")
        {
            _pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsDirectory);
            _deserializer = new TextDeserializer();

            if (!Directory.Exists(_pluginsDirectory))
            {
                Directory.CreateDirectory(_pluginsDirectory);
            }
        }

        /// <summary>
        /// Load all plugins from the plugins directory
        /// </summary>
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

        /// <summary>
        /// Load a plugin from a specific file path
        /// </summary>
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

                    
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load plugin from {filePath}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Unload a specific plugin
        /// </summary>
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
    }
}