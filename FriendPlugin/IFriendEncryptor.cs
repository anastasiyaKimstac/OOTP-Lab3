using System;

namespace FriendPlugin
{
    /// <summary>
    /// Friend's interface - DIFFERENT from our IDataProcessor
    /// This simulates a plugin received from a classmate
    /// </summary>
    public interface IFriendEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string text);
        string Name { get; }
    }
}