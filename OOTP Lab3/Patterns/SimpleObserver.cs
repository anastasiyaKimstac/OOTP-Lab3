using System;
using System.Collections.Generic;

namespace OOTP_Lab3.Patterns
{
    /// <summary>
    /// Observer Pattern - Interface for observing employee changes
    /// Use case: Logging, UI updates, audit trails when employees are added/removed/changed
    /// </summary>
    public interface IEmployeeObserver
    {
        void OnChange(string message);
    }

    /// <summary>
    /// Observer Pattern - Logger implementation that writes to debug output
    /// </summary>
    public class LoggerObserver : IEmployeeObserver
    {
        private readonly string _logFile;

        public LoggerObserver(string logFile = null)
        {
            _logFile = logFile;
        }

        public void OnChange(string message)
        {
            string logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            System.Diagnostics.Debug.WriteLine(logMessage);

            // Optional: Write to file
            if (!string.IsNullOrEmpty(_logFile))
            {
                System.IO.File.AppendAllText(_logFile, logMessage + Environment.NewLine);
            }
        }
    }

    /// <summary>
    /// Observer Pattern - Subject that maintains list of observers and notifies them
    /// </summary>
    public class EmployeeSubject
    {
        private List<IEmployeeObserver> _observers = new List<IEmployeeObserver>();

        public void Attach(IEmployeeObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                System.Diagnostics.Debug.WriteLine($"[Observer] Attached: {observer.GetType().Name}");
            }
        }

        public void Detach(IEmployeeObserver observer)
        {
            _observers.Remove(observer);
            System.Diagnostics.Debug.WriteLine($"[Observer] Detached: {observer.GetType().Name}");
        }

        public void Notify(string message)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnChange(message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Observer] Error notifying {observer.GetType().Name}: {ex.Message}");
                }
            }
        }

        public int ObserverCount => _observers.Count;
    }
}