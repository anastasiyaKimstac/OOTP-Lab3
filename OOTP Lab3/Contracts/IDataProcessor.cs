using System;

namespace OOTP_Lab3.Contracts
{
    /// <summary>
    /// Interface for data processing plugins (encryption, compression, etc.)
    /// </summary>
    public interface IDataProcessor : IPlugin
    {
        /// <summary>
        /// Process data before saving to file
        /// </summary>
        string ProcessBeforeSave(string data);

        /// <summary>
        /// Process data after loading from file
        /// </summary>
        string ProcessAfterLoad(string data);

        /// <summary>
        /// Indicates whether this processor is enabled
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Processor category (Encryption, Compression, etc.)
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Description of the processing
        /// </summary>
        string Description { get; }
    }
}