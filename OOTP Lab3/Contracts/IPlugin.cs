using System.Windows;

namespace OOTP_Lab3.Contracts
{
    /// <summary>
    /// Interface for all plugins to implement
    /// </summary>
    public interface IPlugin
    {
        string PluginId { get; }
        string PluginName { get; }
        string Version { get; }
        void Initialize(IPluginHost host);
        UIElement GetUIElement();
        void Shutdown();
    }
}