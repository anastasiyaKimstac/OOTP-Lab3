using OOTP_Lab3.Contracts;

namespace OOTP_Lab3.Patterns
{
    /// <summary>
    /// Strategy Pattern - For different save formats
    /// Use case: Allows switching between different encryption/encoding strategies at runtime
    /// </summary>
    public interface ISaveStrategy
    {
        string Save(string data);
        string Load(string data);
        string Name { get; }
    }

    /// <summary>
    /// Strategy Pattern - Plain text strategy (no encryption)
    /// </summary>
    public class TxtSaveStrategy : ISaveStrategy
    {
        public string Name => "Plain Text";
        public string Save(string data) => data;
        public string Load(string data) => data;
    }

    /// <summary>
    /// Strategy Pattern - Encrypted save strategy using any IDataProcessor
    /// </summary>
    public class EncryptedSaveStrategy : ISaveStrategy
    {
        private IDataProcessor _processor;
        public string Name => $"Encrypted ({_processor?.PluginName ?? "None"})";

        public EncryptedSaveStrategy(IDataProcessor p)
        {
            _processor = p;
        }

        public string Save(string data) => _processor?.ProcessBeforeSave(data) ?? data;
        public string Load(string data) => _processor?.ProcessAfterLoad(data) ?? data;
    }
}