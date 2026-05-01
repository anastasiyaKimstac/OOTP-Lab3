using System.Windows;
using System.Windows.Controls;
using OOTP_Lab3.Contracts;
using FriendPlugin;  // ← This using is required

namespace OOTP_Lab3.Adapters
{
    /// <summary>
    /// Adapter Pattern - Converts friend's incompatible interface to our IDataProcessor
    /// </summary>
    public class FriendPluginAdapter : IDataProcessor
    {
        private readonly IFriendEncryptor _friend;  // ← Now IFriendEncryptor is found
        private bool _isEnabled;
        private Button _button;

        public FriendPluginAdapter(IFriendEncryptor friend)
        {
            _friend = friend;
        }

        public string PluginId => $"Friend.{_friend.Name.Replace(" ", "")}";
        public string PluginName => $"{_friend.Name} (via Adapter)";
        public string Version => "1.0";
        public string Category => "Friend's Plugin";
        public string Description => $"Adapted from friend's plugin: {_friend.Name}";

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                UpdateButtonAppearance();
            }
        }

        public void Initialize(IPluginHost host) { }

        public UIElement GetUIElement()
        {
            _button = new Button
            {
                Content = $"🔌 {PluginName}",
                Width = 220,
                Margin = new Thickness(2),
                ToolTip = "Click to enable/disable friend's encryption"
            };

            _button.Click += (s, e) => IsEnabled = !IsEnabled;

            UpdateButtonAppearance();
            return _button;
        }

        private void UpdateButtonAppearance()
        {
            if (_button != null)
            {
                _button.Background = IsEnabled ?
                    System.Windows.Media.Brushes.LightGreen :
                    System.Windows.Media.Brushes.LightGray;
            }
        }

        public string ProcessBeforeSave(string data)
        {
            if (!IsEnabled || data == null) return data;
            return _friend.Encrypt(data);
        }

        public string ProcessAfterLoad(string data)
        {
            if (!IsEnabled || data == null) return data;
            return _friend.Decrypt(data);
        }

        public void Shutdown() { }
    }
}