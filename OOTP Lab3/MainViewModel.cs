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
    public class MainViewModel : INotifyPropertyChanged, IPluginHost
    {
        private readonly TextDeserializer _deserializer = new TextDeserializer();
        private readonly PluginManager _pluginManager;

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
        private string _prop1Value = "";
        private string _prop2Value = "";

        public string Prop1Label { get => _prop1Label; set { _prop1Label = value; OnPropertyChanged(); } }
        public string Prop2Label { get => _prop2Label; set { _prop2Label = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowProp2)); } }
        public string Prop1Value { get => _prop1Value; set { _prop1Value = value; OnPropertyChanged(); } }
        public string Prop2Value { get => _prop2Value; set { _prop2Value = value; OnPropertyChanged(); } }
        public bool ShowProp2 => !string.IsNullOrEmpty(Prop2Label);

        private string _prop3Label = "";
        private string _prop3Value = "";
        private bool _showProp3 = false;

        public string Prop3Label { get => _prop3Label; set { _prop3Label = value; OnPropertyChanged(); } }
        public string Prop3Value { get => _prop3Value; set { _prop3Value = value; OnPropertyChanged(); } }
        public bool ShowProp3 { get => _showProp3; set { _showProp3 = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            LoadSampleData();

            _pluginManager = new PluginManager();
            _pluginManager.PluginLoaded += OnPluginLoaded;
            _pluginManager.LoadAllPlugins(this);
        }

        private void OnPluginLoaded(object sender, IPlugin plugin)
        {
            var uiElement = plugin.GetUIElement();
            if (uiElement != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PluginButtons.Add(uiElement);
                    ShowMessage($"Plugin '{plugin.PluginName}' loaded successfully!", "Plugin Loaded");
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
            ShowProp3 = false;

            // Use pattern matching without dynamic
            switch (SelectedEmployee.EmployeeType)
            {
                case "Manager":
                    var m = SelectedEmployee as Manager;
                    if (m != null)
                    {
                        Prop1Label = "Team Size";
                        Prop1Value = m.TeamSize.ToString();
                        Prop2Label = "Department";
                        Prop2Value = m.Department;
                    }
                    break;
                case "Developer":
                    var d = SelectedEmployee as Developer;
                    if (d != null)
                    {
                        Prop1Label = "Main Language";
                        Prop1Value = d.MainLanguage;
                        Prop2Label = "Projects Completed";
                        Prop2Value = d.ProjectsCompleted.ToString();
                    }
                    break;
                case "Designer":
                    var des = SelectedEmployee as Designer;
                    if (des != null)
                    {
                        Prop1Label = "Design Tool";
                        Prop1Value = des.DesignTool;
                        Prop2Label = "Specialization";
                        Prop2Value = des.Specialization;
                    }
                    break;
                case "Tester":
                    var t = SelectedEmployee as Tester;
                    if (t != null)
                    {
                        Prop1Label = "Testing Tool";
                        Prop1Value = t.TestingTool;
                        Prop2Label = "Bugs Found";
                        Prop2Value = t.BugsFound.ToString();
                    }
                    break;
                case "Intern":
                    var i = SelectedEmployee as Intern;
                    if (i != null)
                    {
                        Prop1Label = "University";
                        Prop1Value = i.University;
                        Prop2Label = "Semester";
                        Prop2Value = i.Semester.ToString();
                    }
                    break;
                case "SeniorDeveloper":
                    Prop1Label = "Main Language";
                    Prop1Value = GetPropertyValue("MainLanguage");
                    Prop2Label = "Projects Completed";
                    Prop2Value = GetPropertyValue("ProjectsCompleted");
                    Prop3Label = "Specialization";
                    Prop3Value = GetPropertyValue("Specialization");
                    ShowProp3 = true;
                    break;
            }
        }

        private string GetPropertyValue(string propertyName)
        {
            var prop = SelectedEmployee.GetType().GetProperty(propertyName);
            return prop?.GetValue(SelectedEmployee)?.ToString() ?? "";
        }

        private void SetPropertyValue(string propertyName, string value)
        {
            var prop = SelectedEmployee.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(SelectedEmployee, convertedValue);
            }
        }

        public void SaveChanges()
        {
            if (SelectedEmployee == null) return;

            SelectedEmployee.Name = EditName;
            SelectedEmployee.Salary = EditSalary;
            SelectedEmployee.YearsOfExperience = EditExperience;

            switch (SelectedEmployee.EmployeeType)
            {
                case "Manager":
                    (SelectedEmployee as Manager).TeamSize = int.Parse(Prop1Value);
                    (SelectedEmployee as Manager).Department = Prop2Value;
                    break;
                case "Developer":
                    (SelectedEmployee as Developer).MainLanguage = Prop1Value;
                    (SelectedEmployee as Developer).ProjectsCompleted = int.Parse(Prop2Value);
                    break;
                case "Designer":
                    (SelectedEmployee as Designer).DesignTool = Prop1Value;
                    (SelectedEmployee as Designer).Specialization = Prop2Value;
                    break;
                case "Tester":
                    (SelectedEmployee as Tester).TestingTool = Prop1Value;
                    (SelectedEmployee as Tester).BugsFound = int.Parse(Prop2Value);
                    break;
                case "Intern":
                    (SelectedEmployee as Intern).University = Prop1Value;
                    (SelectedEmployee as Intern).Semester = int.Parse(Prop2Value);
                    break;
                case "SeniorDeveloper":
                    SetPropertyValue("MainLanguage", Prop1Value);
                    SetPropertyValue("ProjectsCompleted", Prop2Value);
                    SetPropertyValue("Specialization", Prop3Value);
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
                var lines = Employees.Select(e => e.EmployeeType == "SeniorDeveloper"
                    ? serializer.SerializeDynamic(e)
                    : e.Accept(serializer));
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
                        newList.Add(_deserializer.Deserialize(line));
                    }
                    catch { }
                }

                if (newList.Any())
                {
                    Employees.Clear();
                    foreach (var emp in newList) Employees.Add(emp);
                    SelectedEmployee = Employees.FirstOrDefault();
                }
            }
            catch { }
        }

        private void RefreshList()
        {
            var selected = SelectedEmployee;
            SelectedEmployee = null;
            SelectedEmployee = selected;
            OnPropertyChanged(nameof(Employees));
        }

        // IPluginHost implementation
        public void AddEmployee(IEmployee employee) => Employees.Add(employee);
        public void RemoveEmployee(IEmployee employee) => Employees.Remove(employee);
        public ObservableCollection<IEmployee> GetEmployees() => Employees;
        public void ShowMessage(string message, string title) => MessageBox.Show(message, title);
        public void SelectEmployee(IEmployee employee) => SelectedEmployee = employee;
        public void SerializeToFile(string path) => SaveToFile(path);
        public void DeserializeFromFile(string path) => LoadFromFile(path);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}