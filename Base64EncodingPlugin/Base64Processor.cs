using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;

namespace Base64EncodingPlugin
{
    /// <summary>
    /// Base64 encoding plugin - converts data to Base64 format
    /// </summary>
    public class Base64Processor : IDataProcessor
    {
        private IPluginHost _host;
        private bool _isEnabled = false;
        private Button _configButton;

        public string PluginId => "Base64EncodingPlugin";
        public string PluginName => "Base64 Encoding";
        public string Version => "1.0.0";
        public string Category => "Encoding";
        public string Description => "Encodes data to Base64 format (no encryption, just encoding)";

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public void Initialize(IPluginHost host)
        {
            _host = host;
            System.Diagnostics.Debug.WriteLine($"Initialized: {PluginName}");
        }

        public UIElement GetUIElement()
        {
            _configButton = new Button
            {
                Content = $"📦 {PluginName}",
                Width = 180,
                Margin = new Thickness(2),
                ToolTip = Description,
                FontSize = 11,
                Tag = this
            };
            _configButton.Click += ConfigButton_Click;
            return _configButton;
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Plugin: {PluginName}\nCategory: {Category}\n\n{Description}\n\n" +
                $"Current status: {(IsEnabled ? "ENABLED" : "DISABLED")}\n\n" +
                $"Enable this processor?",
                PluginName,
                MessageBoxButton.YesNo,
                IsEnabled ? MessageBoxImage.Information : MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                IsEnabled = true;
                _host.SetActiveDataProcessor(this);
                _host.ShowMessage($"{PluginName} enabled", "Processor Enabled");
            }
            else if (IsEnabled)
            {
                IsEnabled = false;
                _host.ShowMessage($"{PluginName} disabled", "Processor Disabled");
            }

            UpdateButtonAppearance();
        }

        private void UpdateButtonAppearance()
        {
            if (_configButton != null)
            {
                _configButton.Background = IsEnabled ?
                    System.Windows.Media.Brushes.LightGreen :
                    System.Windows.Media.Brushes.LightGray;
            }
        }

        public string ProcessBeforeSave(string data)
        {
            if (!IsEnabled) return data;

            try
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base64 encoding failed: {ex.Message}");
                return data;
            }
        }

        public string ProcessAfterLoad(string data)
        {
            if (!IsEnabled) return data;

            try
            {
                var bytes = Convert.FromBase64String(data);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base64 decoding failed: {ex.Message}");
                return data;
            }
        }

        public void Shutdown()
        {
            _isEnabled = false;
            if (_configButton != null)
            {
                _configButton.Click -= ConfigButton_Click;
            }
            System.Diagnostics.Debug.WriteLine($"Shutdown: {PluginName}");
        }
    }
}