using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using OOTP_Lab3.Contracts;
using OOTP_Lab3.Models;
using OOTP_Lab3.PluginHost;
using OOTP_Lab3.Serialization;

namespace OOTP_Lab3
{
    public class MainViewModel : INotifyPropertyChanged, IPluginHost
    {
        private readonly TextDeserializer _deserializer = new TextDeserializer();
        private readonly PluginManager _pluginManager;

        public PluginManager PluginManager => _pluginManager;
        public ObservableCollection<IEmployee> Employees { get; set; } = new ObservableCollection<IEmployee>();
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
        public string EditName { get => _editName; set { _editName = value; OnPropertyChanged(); } }

        private decimal _editSalary;
        public decimal EditSalary { get => _editSalary; set { _editSalary = value; OnPropertyChanged(); } }

        private int _editExperience;
        public int EditExperience { get => _editExperience; set { _editExperience = value; OnPropertyChanged(); } }

        private string _prop1Label = "";
        private string _prop2Label = "";
        private string _prop3Label = "";
        private string _prop1Value = "";
        private string _prop2Value = "";
        private string _prop3Value = "";
        private bool _showProp3 = false;

        public string Prop1Label { get => _prop1Label; set { _prop1Label = value; OnPropertyChanged(); } }
        public string Prop2Label { get => _prop2Label; set { _prop2Label = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowProp2)); } }
        public string Prop3Label { get => _prop3Label; set { _prop3Label = value; OnPropertyChanged(); } }
        public string Prop1Value { get => _prop1Value; set { _prop1Value = value; OnPropertyChanged(); } }
        public string Prop2Value { get => _prop2Value; set { _prop2Value = value; OnPropertyChanged(); } }
        public string Prop3Value { get => _prop3Value; set { _prop3Value = value; OnPropertyChanged(); } }
        public bool ShowProp2 => !string.IsNullOrEmpty(Prop2Label);
        public bool ShowProp3 { get => _showProp3; set { _showProp3 = value; OnPropertyChanged(); } }

        private string _pluginStatus = "Ready";
        public string PluginStatus
        {
            get => _pluginStatus;
            set { _pluginStatus = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            LoadSampleData();

            _pluginManager = new PluginManager();
            _pluginManager.PluginLoaded += OnPluginLoaded;
            _pluginManager.LoadAllPlugins(this);

            PluginStatus = $"Loaded {_pluginManager.LoadedPlugins.Count} plugins";
        }

        private void OnPluginLoaded(object sender, IPlugin plugin)
        {
            var uiElement = plugin.GetUIElement();
            if (uiElement != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PluginButtons.Add(uiElement);
                   
                    PluginStatus = $"Loaded {_pluginManager.LoadedPlugins.Count} plugins";
                });
            }
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

            Prop1Label = "";
            Prop2Label = "";
            Prop3Label = "";
            Prop1Value = "";
            Prop2Value = "";
            Prop3Value = "";
            ShowProp3 = false;

            var properties = SelectedEmployee.GetType().GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "Name" && p.Name != "Salary" &&
                           p.Name != "YearsOfExperience" && p.Name != "EmployeeType")
                .OrderBy(p => p.Name)
                .ToList();

            if (properties.Count > 0)
            {
                Prop1Label = properties[0].Name;
                Prop1Value = properties[0].GetValue(SelectedEmployee)?.ToString() ?? "";
            }
            if (properties.Count > 1)
            {
                Prop2Label = properties[1].Name;
                Prop2Value = properties[1].GetValue(SelectedEmployee)?.ToString() ?? "";
            }
            if (properties.Count > 2)
            {
                Prop3Label = properties[2].Name;
                Prop3Value = properties[2].GetValue(SelectedEmployee)?.ToString() ?? "";
                ShowProp3 = true;
            }
        }

        private void SetPropertyValue(string propertyName, string value)
        {
            var prop = SelectedEmployee.GetType().GetProperty(propertyName);
            if (prop != null && !string.IsNullOrEmpty(value))
            {
                try
                {
                    var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                    prop.SetValue(SelectedEmployee, convertedValue);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to set {propertyName}: {ex.Message}");
                }
            }
        }

        public void SaveChanges()
        {
            if (SelectedEmployee == null) return;

            SelectedEmployee.Name = EditName;
            SelectedEmployee.Salary = EditSalary;
            SelectedEmployee.YearsOfExperience = EditExperience;

            var properties = SelectedEmployee.GetType().GetProperties()
                .Where(p => p.Name != "Id" && p.Name != "Name" && p.Name != "Salary" &&
                           p.Name != "YearsOfExperience" && p.Name != "EmployeeType")
                .OrderBy(p => p.Name)
                .ToList();

            var values = new[] { Prop1Value, Prop2Value, Prop3Value };
            for (int i = 0; i < properties.Count && i < values.Length; i++)
            {
                if (!string.IsNullOrEmpty(values[i]))
                {
                    SetPropertyValue(properties[i].Name, values[i]);
                }
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
                _ => TryCreatePluginEmployee(type)
            };

            if (emp != null)
            {
                Employees.Add(emp);
                SelectedEmployee = emp;
            }
        }

        private IEmployee TryCreatePluginEmployee(string typeName)
        {

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name == typeName && typeof(IEmployee).IsAssignableFrom(t) && !t.IsAbstract);

                if (type != null)
                {
                    try
                    {
                        return (IEmployee)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to create {typeName}: {ex.Message}");
                    }
                }
            }
            throw new ArgumentException($"Unknown employee type: {typeName}");
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
                var lines = Employees.Select(e =>
                {
                    return serializer.SerializeDynamic(e);
                });
                File.WriteAllLines(path, lines);
                MessageBox.Show($"Successfully saved {Employees.Count} employees", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        public void LoadFromFile(string path)
        {
            if (!File.Exists(path)) return;

            try
            {
                var lines = File.ReadAllLines(path);
                var newList = new ObservableCollection<IEmployee>();

                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    try
                    {
                        var employee = _deserializer.Deserialize(line);
                        if (employee != null)
                            newList.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to deserialize line: {ex.Message}");
                    }
                }

                if (newList.Any())
                {
                    Employees.Clear();
                    foreach (var emp in newList) Employees.Add(emp);
                    SelectedEmployee = Employees.FirstOrDefault();
                    MessageBox.Show($"Successfully loaded {newList.Count} employees", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error");
            }
        }

        private void RefreshList()
        {
            var selected = SelectedEmployee;
            SelectedEmployee = null;
            SelectedEmployee = selected;
            OnPropertyChanged(nameof(Employees));
        }

        public void AddEmployee(IEmployee employee)
        {
            if (employee != null)
            {
                Employees.Add(employee);
                PluginStatus = $"Total employees: {Employees.Count}";
            }
        }

        public void RemoveEmployee(IEmployee employee)
        {
            if (employee != null)
            {
                Employees.Remove(employee);
                PluginStatus = $"Total employees: {Employees.Count}";
            }
        }

        public ObservableCollection<IEmployee> GetEmployees() => Employees;

        public void ShowMessage(string message, string title)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, title);
            });
        }

        public void SelectEmployee(IEmployee employee) => SelectedEmployee = employee;
        public void SerializeToFile(string path) => SaveToFile(path);
        public void DeserializeFromFile(string path) => LoadFromFile(path);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}