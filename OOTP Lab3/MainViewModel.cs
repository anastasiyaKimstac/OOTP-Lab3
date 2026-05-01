using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using OOTP_Lab3.Models;
using OOTP_Lab3.Serialization;

namespace OOTP_Lab3
{
    /// <summary>
    /// Main ViewModel handling all business logic and UI state
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TextDeserializer _deserializer = new TextDeserializer();

        public ObservableCollection<IEmployee> Employees { get; set; } = new ObservableCollection<IEmployee>();

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

        public MainViewModel()
        {
            LoadSampleData();
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}