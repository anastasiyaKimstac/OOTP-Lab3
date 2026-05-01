using System.Collections.ObjectModel;
using OOTP_Lab3.Models;

namespace OOTP_Lab3.Contracts
{
    /// <summary>
    /// Host interface that plugins can interact with
    /// </summary>
    public interface IPluginHost
    {
        void AddEmployee(IEmployee employee);
        void RemoveEmployee(IEmployee employee);
        ObservableCollection<IEmployee> GetEmployees();
        void ShowMessage(string message, string title);
        void SelectEmployee(IEmployee employee);
        void SerializeToFile(string path);
        void DeserializeFromFile(string path);

        // New methods for data processor plugins
        void RegisterDataProcessor(IDataProcessor processor);
        void UnregisterDataProcessor(IDataProcessor processor);
        IDataProcessor GetActiveDataProcessor();
        void SetActiveDataProcessor(IDataProcessor processor);
    }
}