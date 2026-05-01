using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;

namespace XOREncryptionPlugin
{
    /// <summary>
    /// XOR encryption plugin - simple XOR encryption with a key
    /// </summary>
    public class XORProcessor : IDataProcessor
    {
        private IPluginHost _host;
        private bool _isEnabled = false;
        private Button _configButton;
        private byte _xorKey = 0x55; // Default XOR key

        public string PluginId => "XOREncryptionPlugin";
        public string PluginName => "XOR Encryption";
        public string Version => "1.0.0";
        public string Category => "Encryption";
        public string Description => "XOR encryption with configurable key (moderate encryption)";

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
                Content = $"🔑 {PluginName}",
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
            // Create a simple configuration dialog
            var window = new Window
            {
                Title = PluginName,
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                        new TextBlock { Text = $"Plugin: {PluginName}", FontWeight = FontWeights.Bold, Margin = new Thickness(0,0,0,10) },
                        new TextBlock { Text = Description, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,0,0,10) },
                        new TextBlock { Text = "Current status:", Margin = new Thickness(0,5,0,0) },
                        new CheckBox { Name = "EnableCheckbox", Content = "Enable processor", IsChecked = IsEnabled, Margin = new Thickness(0,5,0,10) },
                        new TextBlock { Text = "XOR Key (0-255):", Margin = new Thickness(0,5,0,0) },
                        new TextBox { Name = "KeyTextBox", Text = _xorKey.ToString(), Margin = new Thickness(0,5,0,10) },
                        new Button { Content = "Apply", Height = 30, Margin = new Thickness(0,10,0,0) }
                    }
                }
            };

            var applyButton = ((StackPanel)window.Content).Children[^1] as Button;
            var enableCheckbox = ((StackPanel)window.Content).Children[3] as CheckBox;
            var keyTextBox = ((StackPanel)window.Content).Children[5] as TextBox;

            applyButton.Click += (s, args) =>
            {
                IsEnabled = enableCheckbox.IsChecked ?? false;

                if (byte.TryParse(keyTextBox.Text, out byte key))
                {
                    _xorKey = key;
                }

                if (IsEnabled)
                {
                    _host.SetActiveDataProcessor(this);
                }

                UpdateButtonAppearance();
                window.Close();
            };

            window.ShowDialog();
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
                return ApplyXOR(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XOR encryption failed: {ex.Message}");
                return data;
            }
        }

        public string ProcessAfterLoad(string data)
        {
            if (!IsEnabled) return data;

            try
            {
                return ApplyXOR(data); // XOR is symmetric
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XOR decryption failed: {ex.Message}");
                return data;
            }
        }

        private string ApplyXOR(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var bytes = Encoding.UTF8.GetBytes(input);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= _xorKey;
            }
            return Convert.ToBase64String(bytes);
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