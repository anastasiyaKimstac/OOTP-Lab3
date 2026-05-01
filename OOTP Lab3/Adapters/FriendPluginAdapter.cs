using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;
using FriendPlugin;

namespace OOTP_Lab3.Adapters
{
    /// <summary>
    /// Adapter Pattern - Converts friend's incompatible interface to our IDataProcessor
    /// </summary>
    public class FriendPluginAdapter : IDataProcessor
    {
        private readonly IFriendEncryptor _friend;
        private bool _isEnabled;
        private Button _menuButton;
        private Button _toolbarButton;
        private IPluginHost _host;

        public FriendPluginAdapter(IFriendEncryptor friend)
        {
            _friend = friend;
        }

        public string PluginId => $"Friend.{_friend.Name.Replace(" ", "")}";
        public string PluginName => $"{_friend.Name} (via Adapter)";
        public string Version => "1.0";
        public string Category => "Friend's Plugin";
        public string Description => $"Adapted from friend's plugin: {_friend.Name}";

        public int Priority => 999;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                UpdateButtonAppearances();

                // CRITICAL FIX: Set as active processor when enabled
                if (_isEnabled && _host != null)
                {
                    _host.SetActiveDataProcessor(this);
                    System.Diagnostics.Debug.WriteLine($"[Adapter] {PluginName} set as ACTIVE processor");
                }
            }
        }

        public void Initialize(IPluginHost host)
        {
            _host = host;
            System.Diagnostics.Debug.WriteLine($"[Adapter] Initialized: {PluginName}");
        }

        public UIElement GetUIElement()
        {
            _menuButton = new Button
            {
                Content = $"🔌 {PluginName}",
                Width = 260,
                Margin = new Thickness(2),
                ToolTip = "Click to enable/disable friend's encryption plugin",
                Tag = this
            };

            _menuButton.Click += (s, e) => ToggleEnabled();
            UpdateButtonAppearances();
            return _menuButton;
        }

        public UIElement GetToolbarButton()
        {
            _toolbarButton = new Button
            {
                Content = $"👥 {_friend.Name}",
                Width = 160,
                Margin = new Thickness(2),
                ToolTip = "Friend's encryption plugin (via Adapter pattern)",
                Tag = this
            };

            _toolbarButton.Click += (s, e) => ToggleEnabled();
            UpdateButtonAppearances();
            return _toolbarButton;
        }

        private void ToggleEnabled()
        {
            IsEnabled = !IsEnabled;
            System.Diagnostics.Debug.WriteLine($"[Adapter] Friend plugin toggled: {(IsEnabled ? "ENABLED" : "DISABLED")}");
        }

        private void UpdateButtonAppearances()
        {
            var bg = IsEnabled ?
                System.Windows.Media.Brushes.LightGreen :
                System.Windows.Media.Brushes.LightGray;

            if (_menuButton != null) _menuButton.Background = bg;
            if (_toolbarButton != null) _toolbarButton.Background = bg;
        }

        public string ProcessBeforeSave(string data)
        {
            if (!IsEnabled || data == null) return data;
            System.Diagnostics.Debug.WriteLine($"[Adapter] ENCRYPTING: {data.Substring(0, Math.Min(50, data.Length))}...");
            string result = _friend.Encrypt(data);
            System.Diagnostics.Debug.WriteLine($"[Adapter] ENCRYPTED: {result.Substring(0, Math.Min(50, result.Length))}...");
            return result;
        }

        public string ProcessAfterLoad(string data)
        {
            if (!IsEnabled || data == null) return data;
            System.Diagnostics.Debug.WriteLine($"[Adapter] DECRYPTING...");
            return _friend.Decrypt(data);
        }

        public void Shutdown()
        {
            IsEnabled = false;
            System.Diagnostics.Debug.WriteLine($"[Adapter] Shutdown: {_friend.Name}");
        }
    }
}