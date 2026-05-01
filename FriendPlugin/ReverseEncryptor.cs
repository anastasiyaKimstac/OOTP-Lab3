using System;

namespace FriendPlugin
{
    /// <summary>
    /// Friend's implementation - Simple Reverse encryption
    /// This is the actual plugin received from a classmate
    /// </summary>
    public class ReverseEncryptor : IFriendEncryptor
    {
        public string Name => "Reverse Cipher (Friend)";

        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            char[] arr = text.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public string Decrypt(string text) => Encrypt(text); // Reverse is symmetric
    }
}