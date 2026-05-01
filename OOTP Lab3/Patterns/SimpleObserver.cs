using System;
using System.Collections.Generic;

namespace OOTP_Lab3.Patterns
{
    // Observer Pattern - For logging employee changes
    public interface IEmployeeObserver
    {
        void OnChange(string message);
    }

    public class LoggerObserver : IEmployeeObserver
    {
        public void OnChange(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[LOG] {DateTime.Now}: {message}");
        }
    }

    public class EmployeeSubject
    {
        private List<IEmployeeObserver> _observers = new List<IEmployeeObserver>();

        public void Attach(IEmployeeObserver observer) => _observers.Add(observer);
        public void Notify(string message)
        {
            foreach (var o in _observers) o.OnChange(message);
        }
    }
}