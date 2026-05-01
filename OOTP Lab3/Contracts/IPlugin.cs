using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace OOTP_Lab3.Contracts
{
    /// <summary>
    /// Interface for all plugins to implement
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Unique identifier for the plugin
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// Plugin name displayed in UI
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Plugin version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Initialize the plugin with host reference
        /// </summary>
        void Initialize(IPluginHost host);

        /// <summary>
        /// Get UI elements to add to main interface
        /// </summary>
        UIElement GetUIElement();

        /// <summary>
        /// Called when plugin is being unloaded
        /// </summary>
        void Shutdown();
    }
}