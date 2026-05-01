using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;

namespace ROT13EncryptionPlugin
{
    /// <summary>
    /// ROT13 encryption plugin - simple character shifting encryption
    /// </summary>
    public class ROT13Processor : IDataProcessor
    {
        private IPluginHost _host;
        private bool _isEnabled = false;
        private Button _configButton;

        public string PluginId => "ROT13EncryptionPlugin";
        public string PluginName => "ROT13 Encryption";
        public string Version => "1.0.0";
        public string Category => "Encryption";
        public string Description => "Simple ROT13 character encryption (weak encryption)";

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
            // Return settings button for menu
            _configButton = new Button
            {
                Content = $"🔐 {PluginName}",
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

            // Update button appearance
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
                return ApplyROT13(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ROT13 encryption failed: {ex.Message}");
                return data;
            }
        }

        public string ProcessAfterLoad(string data)
        {
            if (!IsEnabled) return data;

            try
            {
                return ApplyROT13(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ROT13 decryption failed: {ex.Message}");
                return data;
            }
        }

        private string ApplyROT13(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var result = new StringBuilder();
            foreach (char c in input)
            {
                if (c >= 'a' && c <= 'z')
                {
                    result.Append((char)(((c - 'a' + 13) % 26) + 'a'));
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    result.Append((char)(((c - 'A' + 13) % 26) + 'A'));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
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