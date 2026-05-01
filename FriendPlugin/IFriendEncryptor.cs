using System;

namespace FriendPlugin
{
    /// <summary>
    /// Friend's interface - DIFFERENT from our IDataProcessor
    /// This simulates a plugin received from a classmate
    /// The interface is incompatible with our system, requiring the Adapter pattern
    /// </summary>
    public interface IFriendEncryptor
    {
        /// <summary>
        /// Encrypt the given text
        /// </summary>
        string Encrypt(string text);

        /// <summary>
        /// Decrypt the given text
        /// </summary>
        string Decrypt(string text);

        /// <summary>
        /// Name of the encryption plugin
        /// </summary>
        string Name { get; }
    }
}