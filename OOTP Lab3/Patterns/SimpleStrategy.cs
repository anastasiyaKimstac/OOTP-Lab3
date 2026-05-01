using OOTP_Lab3.Contracts;

namespace OOTP_Lab3.Patterns
{
    // Strategy Pattern - For different save formats
    public interface ISaveStrategy
    {
        string Save(string data);
        string Load(string data);
    }

    public class TxtSaveStrategy : ISaveStrategy
    {
        public string Save(string data) => data;
        public string Load(string data) => data;
    }

    public class EncryptedSaveStrategy : ISaveStrategy
    {
        private IDataProcessor _processor;
        public EncryptedSaveStrategy(IDataProcessor p) { _processor = p; }
        public string Save(string data) => _processor.ProcessBeforeSave(data);
        public string Load(string data) => _processor.ProcessAfterLoad(data);
    }
}