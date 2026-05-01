using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;

namespace XOREncryptionPlugin
{
    public class XORProcessor : IDataProcessor
    {
        private IPluginHost _host;
        private bool _isEnabled = false;
        private Button _configButton;
        private byte _xorKey = 0x55;

        public string PluginId => "XOREncryptionPlugin";
        public string PluginName => "XOR Encryption";
        public string Version => "1.0.0";
        public string Category => "Encryption";
        public string Description => "XOR encryption with configurable key";

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
            // Создаем УВЕЛИЧЕННОЕ окно с центрированием
            var window = new Window
            {
                Title = PluginName,
                Width = 500,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Owner = Application.Current.MainWindow,
                Background = System.Windows.Media.Brushes.White
            };

            // Создаем ScrollViewer для прокрутки (на случай если контент не помещается)
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            // Создаем контент с увеличенными отступами
            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20)
            };

            // Заголовок
            stackPanel.Children.Add(new TextBlock
            {
                Text = $"🔐 {PluginName}",
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = System.Windows.Media.Brushes.DarkBlue
            });

            // Описание
            stackPanel.Children.Add(new TextBlock
            {
                Text = Description,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = System.Windows.Media.Brushes.Gray,
                FontSize = 12
            });

            // Разделитель
            stackPanel.Children.Add(new Separator
            {
                Margin = new Thickness(0, 0, 0, 20),
                Background = System.Windows.Media.Brushes.LightGray,
                Height = 1
            });

            // Группа статуса
            var statusGroup = new GroupBox
            {
                Header = "Status",
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(10)
            };

            var statusPanel = new StackPanel();
            statusPanel.Children.Add(new TextBlock
            {
                Text = "Current processor status:",
                Margin = new Thickness(0, 0, 0, 8),
                FontSize = 12
            });

            var enableCheckbox = new CheckBox
            {
                Name = "EnableCheckbox",
                Content = "Enable XOR encryption processor",
                IsChecked = IsEnabled,
                Margin = new Thickness(0, 0, 0, 5),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold
            };
            statusPanel.Children.Add(enableCheckbox);

            statusGroup.Content = statusPanel;
            stackPanel.Children.Add(statusGroup);

            // Группа настройки ключа
            var keyGroup = new GroupBox
            {
                Header = "Encryption Key Settings",
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(10)
            };

            var keyPanel = new StackPanel();

            keyPanel.Children.Add(new TextBlock
            {
                Text = "XOR Key (0-255):",
                Margin = new Thickness(0, 0, 0, 8),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            });

            var keyTextBox = new TextBox
            {
                Name = "KeyTextBox",
                Text = _xorKey.ToString(),
                Margin = new Thickness(0, 0, 0, 12),
                Width = 120,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 14,
                Padding = new Thickness(5)
            };
            keyPanel.Children.Add(keyTextBox);

            // Ползунок для выбора ключа
            keyPanel.Children.Add(new TextBlock
            {
                Text = "Quick select:",
                Margin = new Thickness(0, 0, 0, 5),
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.DarkGray
            });

            var slider = new Slider
            {
                Minimum = 0,
                Maximum = 255,
                Value = _xorKey,
                TickFrequency = 16,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
                Margin = new Thickness(0, 0, 0, 12),
                Width = 400
            };
            slider.ValueChanged += (s, args) =>
            {
                keyTextBox.Text = ((int)args.NewValue).ToString();
            };
            keyPanel.Children.Add(slider);

            keyPanel.Children.Add(new TextBlock
            {
                Text = "💡 Tip: Different keys produce different encryption results. Use the same key for encryption and decryption.",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.DarkGreen,
                Margin = new Thickness(0, 10, 0, 0)
            });

            keyGroup.Content = keyPanel;
            stackPanel.Children.Add(keyGroup);

            // Группа информации
            var infoGroup = new GroupBox
            {
                Header = "Information",
                Margin = new Thickness(0, 0, 0, 20),
                Padding = new Thickness(10)
            };

            var infoPanel = new StackPanel();
            infoPanel.Children.Add(new TextBlock
            {
                Text = "• XOR is a symmetric encryption method\n" +
                       "• The same key is used for both encryption and decryption\n" +
                       "• Encrypted data is stored in Base64 format for safety\n" +
                       "• Key range: 0-255 (byte value)",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 0),
                LineHeight = 18
            });

            infoGroup.Content = infoPanel;
            stackPanel.Children.Add(infoGroup);

            // Кнопки
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var applyButton = new Button
            {
                Content = "✓ Apply Settings",
                Width = 130,
                Height = 38,
                Margin = new Thickness(0, 0, 15, 0),
                Background = System.Windows.Media.Brushes.LightGreen,
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var cancelButton = new Button
            {
                Content = "✗ Cancel",
                Width = 100,
                Height = 38,
                Background = System.Windows.Media.Brushes.LightCoral,
                FontSize = 13,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            buttonPanel.Children.Add(applyButton);
            buttonPanel.Children.Add(cancelButton);
            stackPanel.Children.Add(buttonPanel);

            scrollViewer.Content = stackPanel;
            window.Content = scrollViewer;

            // Обработчик кнопки Apply
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
                    _host.ShowMessage($"{PluginName} enabled with key {_xorKey}", "Processor Enabled");
                }
                else
                {
                    _host.ShowMessage($"{PluginName} disabled", "Processor Disabled");
                }

                UpdateButtonAppearance();
                window.DialogResult = true;
                window.Close();
            };

            // Обработчик кнопки Cancel
            cancelButton.Click += (s, args) =>
            {
                window.DialogResult = false;
                window.Close();
            };

            // Показываем окно
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
                var bytes = Encoding.UTF8.GetBytes(data);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= _xorKey;
                }
                return Convert.ToBase64String(bytes);
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
                var bytes = Convert.FromBase64String(data);
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= _xorKey;
                }
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"XOR decryption failed: {ex.Message}");
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