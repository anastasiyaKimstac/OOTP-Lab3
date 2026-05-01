using System;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;

namespace SeniorDeveloperPlugin
{
    /// <summary>
    /// Plugin that adds SeniorDeveloper to the employee hierarchy
    /// </summary>
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
            _host.ShowMessage($"Senior Developer Plugin v{Version} loaded!", "Plugin Loaded");
        }

        public UIElement GetUIElement()
        {
            _addButton = new Button
            {
                Content = "👨‍💼 Senior Dev",
                Width = 65,
                Margin = new Thickness(2),
                ToolTip = "Add Senior Developer (Plugin)"
            };
            _addButton.Click += AddSeniorDeveloper_Click;
            return _addButton;
        }

        private void AddSeniorDeveloper_Click(object sender, RoutedEventArgs e)
        {
            var seniorDev = new SeniorDeveloper
            {
                Name = "New Senior Developer",
                Salary = 110000,
                YearsOfExperience = 8,
                MainLanguage = "C#",
                ProjectsCompleted = 25,
                Seniority = "Senior",
                MentoredJuniors = 5,
                Specialization = "Backend"
            };

            _host.AddEmployee(seniorDev);
            _host.SelectEmployee(seniorDev);
            _host.ShowMessage($"{seniorDev.Name} added successfully!", "Employee Added");
        }

        public void Shutdown()
        {
            if (_addButton != null)
            {
                _addButton.Click -= AddSeniorDeveloper_Click;
            }
        }
    }
}