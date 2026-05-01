using System.Collections.ObjectModel;
using OOTP_Lab3.Models;

namespace OOTP_Lab3.Contracts
{
    /// <summary>
    /// Host interface that plugins can interact with
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// Add a new employee to the collection
        /// </summary>
        void AddEmployee(IEmployee employee);

        /// <summary>
        /// Remove an employee from the collection
        /// </summary>
        void RemoveEmployee(IEmployee employee);

        /// <summary>
        /// Get all employees
        /// </summary>
        ObservableCollection<IEmployee> GetEmployees();

        /// <summary>
        /// Show a message to the user
        /// </summary>
        void ShowMessage(string message, string title);

        /// <summary>
        /// Select an employee in the UI
        /// </summary>
        void SelectEmployee(IEmployee employee);

        /// <summary>
        /// Serialize all employees to file
        /// </summary>
        void SerializeToFile(string path);

        /// <summary>
        /// Deserialize employees from file
        /// </summary>
        void DeserializeFromFile(string path);
    }
}