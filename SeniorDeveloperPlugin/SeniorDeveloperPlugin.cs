using System;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;

namespace SeniorDeveloperPlugin
{
    public class SeniorDeveloperPlugin : IPlugin
    {
        private IPluginHost _host;
        private Button _addButton;

        public string PluginId => "SeniorDeveloperPlugin";
        public string PluginName => "Senior Developer Plugin";
        public string Version => "1.0.0";

        public void Initialize(IPluginHost host)
        {
            _host = host;

            // Отладочный вывод в консоль
            System.Diagnostics.Debug.WriteLine("=========================================");
            System.Diagnostics.Debug.WriteLine("Senior Developer Plugin INITIALIZED!");
            System.Diagnostics.Debug.WriteLine($"Plugin ID: {PluginId}");
            System.Diagnostics.Debug.WriteLine($"Plugin Name: {PluginName}");
            System.Diagnostics.Debug.WriteLine($"Plugin Version: {Version}");
            System.Diagnostics.Debug.WriteLine("=========================================");

            // Показать сообщение пользователю
            host.ShowMessage($"Plugin '{PluginName}' v{Version} loaded!", "Plugin Loaded");
        }

        public UIElement GetUIElement()
        {
            _addButton = new Button
            {
                Content = "👨‍💼 Senior Dev",
                Width = 90,
                Margin = new Thickness(2),
                ToolTip = "Add Senior Developer (Plugin)",
                FontSize = 11,
                Background = System.Windows.Media.Brushes.LightGreen
            };
            _addButton.Click += AddSeniorDeveloper_Click;

            System.Diagnostics.Debug.WriteLine("GetUIElement() called - button created");

            return _addButton;
        }

        private void AddSeniorDeveloper_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Senior Dev button CLICKED!");

            var seniorDev = new SeniorDeveloper();
            _host.AddEmployee(seniorDev);
            _host.SelectEmployee(seniorDev);
            _host.ShowMessage($"{seniorDev.Name} added successfully!", "Plugin");

            System.Diagnostics.Debug.WriteLine($"Added: {seniorDev.Name}, Salary: {seniorDev.Salary}");
        }

        public void Shutdown()
        {
            System.Diagnostics.Debug.WriteLine("Senior Developer Plugin SHUTDOWN");
            if (_addButton != null)
            {
                _addButton.Click -= AddSeniorDeveloper_Click;
            }
        }
    }
}