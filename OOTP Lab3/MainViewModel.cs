using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using OOTP_Lab3.Contracts;
using OOTP_Lab3.Models;
using OOTP_Lab3.PluginHost;
using OOTP_Lab3.Serialization;

namespace OOTP_Lab3
{
    /// <summary>
    /// Main ViewModel handling all business logic and UI state with plugin support
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged, IPluginHost
    {
        private readonly TextDeserializer _deserializer = new TextDeserializer();
        private readonly PluginManager _pluginManager;

        public ObservableCollection<IEmployee> Employees { get; set; } = new ObservableCollection<IEmployee>();

        /// <summary>
        /// Collection of plugin UI elements to display
        /// </summary>
        public ObservableCollection<UIElement> PluginButtons { get; set; } = new ObservableCollection<UIElement>();

        private IEmployee _selectedEmployee;
        public IEmployee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanEdit));
                UpdateEditForm();
            }
        }

        public bool CanEdit => SelectedEmployee != null;

        private string _editName;
        public string EditName
        {
            get => _editName;
            set { _editName = value; OnPropertyChanged(); }
        }

        private decimal _editSalary;
        public decimal EditSalary
        {
            get => _editSalary;
            set { _editSalary = value; OnPropertyChanged(); }
        }

        private int _editExperience;
        public int EditExperience
        {
            get => _editExperience;
            set { _editExperience = value; OnPropertyChanged(); }
        }

        private string _prop1Label = "";
        private string _prop2Label = "";
        private string _prop1Value = "";
        private string _prop2Value = "";

        public string Prop1Label
        {
            get => _prop1Label;
            set { _prop1Label = value; OnPropertyChanged(); }
        }

        public string Prop2Label
        {
            get => _prop2Label;
            set { _prop2Label = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowProp2)); }
        }

        public string Prop1Value
        {
            get => _prop1Value;
            set { _prop1Value = value; OnPropertyChanged(); }
        }

        public string Prop2Value
        {
            get => _prop2Value;
            set { _prop2Value = value; OnPropertyChanged(); }
        }

        public bool ShowProp2 => !string.IsNullOrEmpty(Prop2Label);

        // Additional properties for SeniorDeveloper support
        private string _prop3Label = "";
        private string _prop3Value = "";
        private bool _showProp3 = false;

        public string Prop3Label
        {
            get => _prop3Label;
            set { _prop3Label = value; OnPropertyChanged(); }
        }

        public string Prop3Value
        {
            get => _prop3Value;
            set { _prop3Value = value; OnPropertyChanged(); }
        }

        public bool ShowProp3
        {
            get => _showProp3;
            set { _showProp3 = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            LoadSampleData();

            // Initialize plugin manager
            _pluginManager = new PluginManager();
            _pluginManager.PluginLoaded += OnPluginLoaded;

            // Load all plugins
            _pluginManager.LoadAllPlugins(this);
        }

        private void OnPluginLoaded(object sender, IPlugin plugin)
        {
            // Add plugin UI elements to the toolbar
            var uiElement = plugin.GetUIElement();
            if (uiElement != null)
            {
                PluginButtons.Add(uiElement);
            }

            // Additional plugin UI can be added here
            Application.Current.Dispatcher.Invoke(() =>
            {
                ShowMessage($"Plugin '{plugin.PluginName}' loaded successfully!", "Plugin Loaded");
            });
        }

        private void LoadSampleData()
        {
            Employees.Add(new Manager { Name = "Alice Johnson", Salary = 95000, TeamSize = 8, Department = "Engineering", YearsOfExperience = 10 });
            Employees.Add(new Developer { Name = "Bob Smith", Salary = 85000, MainLanguage = "C#", ProjectsCompleted = 12, YearsOfExperience = 5 });
            Employees.Add(new Designer { Name = "Carol Davis", Salary = 72000, DesignTool = "Figma", Specialization = "UI Design", YearsOfExperience = 4 });
            Employees.Add(new Tester { Name = "David Wilson", Salary = 60000, TestingTool = "Selenium", BugsFound = 45, YearsOfExperience = 3 });
            Employees.Add(new Intern { Name = "Emma Brown", Salary = 25000, University = "MIT", Semester = 6, YearsOfExperience = 0 });
        }

        private void UpdateEditForm()
        {
            if (SelectedEmployee == null) return;

            EditName = SelectedEmployee.Name;
            EditSalary = SelectedEmployee.Salary;
            EditExperience = SelectedEmployee.YearsOfExperience;

            // Reset additional properties
            ShowProp3 = false;

            switch (SelectedEmployee)
            {
                case Manager m:
                    Prop1Label = "Team Size";
                    Prop1Value = m.TeamSize.ToString();
                    Prop2Label = "Department";
                    Prop2Value = m.Department;
                    break;
                case Developer d:
                    Prop1Label = "Main Language";
                    Prop1Value = d.MainLanguage;
                    Prop2Label = "Projects Completed";
                    Prop2Value = d.ProjectsCompleted.ToString();
                    break;
                case Designer d:
                    Prop1Label = "Design Tool";
                    Prop1Value = d.DesignTool;
                    Prop2Label = "Specialization";
                    Prop2Value = d.Specialization;
                    break;
                case Tester t:
                    Prop1Label = "Testing Tool";
                    Prop1Value = t.TestingTool;
                    Prop2Label = "Bugs Found";
                    Prop2Value = t.BugsFound.ToString();
                    break;
                case Intern i:
                    Prop1Label = "University";
                    Prop1Value = i.University;
                    Prop2Label = "Semester";
                    Prop2Value = i.Semester.ToString();
                    break;
                // Support for plugin-added employee types
                case dynamic dyn when dyn.GetType().Name == "SeniorDeveloper":
                    var seniorDev = SelectedEmployee;
                    Prop1Label = "Main Language";
                    Prop1Value = seniorDev.GetType().GetProperty("MainLanguage")?.GetValue(seniorDev)?.ToString() ?? "";
                    Prop2Label = "Projects Completed";
                    Prop2Value = seniorDev.GetType().GetProperty("ProjectsCompleted")?.GetValue(seniorDev)?.ToString() ?? "";
                    Prop3Label = "Specialization";
                    Prop3Value = seniorDev.GetType().GetProperty("Specialization")?.GetValue(seniorDev)?.ToString() ?? "";
                    ShowProp3 = true;
                    break;
            }
        }

        public void SaveChanges()
        {
            if (SelectedEmployee == null) return;

            SelectedEmployee.Name = EditName;
            SelectedEmployee.Salary = EditSalary;
            SelectedEmployee.YearsOfExperience = EditExperience;

            switch (SelectedEmployee)
            {
                case Manager m:
                    m.TeamSize = int.Parse(Prop1Value);
                    m.Department = Prop2Value;
                    break;
                case Developer d:
                    d.MainLanguage = Prop1Value;
                    d.ProjectsCompleted = int.Parse(Prop2Value);
                    break;
                case Designer d:
                    d.DesignTool = Prop1Value;
                    d.Specialization = Prop2Value;
                    break;
                case Tester t:
                    t.TestingTool = Prop1Value;
                    t.BugsFound = int.Parse(Prop2Value);
                    break;
                case Intern i:
                    i.University = Prop1Value;
                    i.Semester = int.Parse(Prop2Value);
                    break;
                // Handle plugin types
                case dynamic dyn when dyn.GetType().Name == "SeniorDeveloper":
                    var seniorDev = SelectedEmployee;
                    seniorDev.GetType().GetProperty("MainLanguage")?.SetValue(seniorDev, Prop1Value);
                    seniorDev.GetType().GetProperty("ProjectsCompleted")?.SetValue(seniorDev, int.Parse(Prop2Value));
                    seniorDev.GetType().GetProperty("Specialization")?.SetValue(seniorDev, Prop3Value);
                    break;
            }

            RefreshList();
        }

        public void AddEmployee(string type)
        {
            IEmployee emp = type switch
            {
                "Manager" => new Manager(),
                "Developer" => new Developer(),
                "Designer" => new Designer(),
                "Tester" => new Tester(),
                "Intern" => new Intern(),
                _ => throw new ArgumentException($"Unknown employee type: {type}")
            };

            Employees.Add(emp);
            SelectedEmployee = emp;
        }

        public void DeleteEmployee()
        {
            if (SelectedEmployee != null)
            {
                Employees.Remove(SelectedEmployee);
                SelectedEmployee = Employees.FirstOrDefault();
            }
        }

        public void SaveToFile(string path)
        {
            try
            {
                var serializer = new TextSerializer();
                var lines = Employees.Select(e => e.Accept(serializer));
                File.WriteAllLines(path, lines);
                MessageBox.Show($"Successfully saved {Employees.Count} employees to {path}", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to file: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"File not found: {path}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var lines = File.ReadAllLines(path);
                var newList = new ObservableCollection<IEmployee>();

                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    try
                    {
                        newList.Add(_deserializer.Deserialize(line));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Skipping invalid line: {line}\nError: {ex.Message}", "Warning",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                if (newList.Any())
                {
                    Employees.Clear();
                    foreach (var emp in newList) Employees.Add(emp);
                    SelectedEmployee = Employees.FirstOrDefault();
                    MessageBox.Show($"Successfully loaded {newList.Count} employees from {path}", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading from file: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshList()
        {
            var selected = SelectedEmployee;
            SelectedEmployee = null;
            SelectedEmployee = selected;
            OnPropertyChanged(nameof(Employees));
        }

        // IPluginHost implementation
        public void AddEmployee(IEmployee employee)
        {
            Employees.Add(employee);
        }

        public void RemoveEmployee(IEmployee employee)
        {
            Employees.Remove(employee);
        }

        public ObservableCollection<IEmployee> GetEmployees()
        {
            return Employees;
        }

        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SelectEmployee(IEmployee employee)
        {
            SelectedEmployee = employee;
        }

        public void SerializeToFile(string path)
        {
            SaveToFile(path);
        }

        public void DeserializeFromFile(string path)
        {
            LoadFromFile(path);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}