using FriendPlugin;
using OOTP_Lab3.Adapters;
using OOTP_Lab3.Contracts;
using OOTP_Lab3.Models;
using OOTP_Lab3.Patterns;
using OOTP_Lab3.PluginHost;
using OOTP_Lab3.Serialization;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OOTP_Lab3
{
    public class MainViewModel : INotifyPropertyChanged, IPluginHost
    {
        private readonly TextDeserializer _deserializer = new TextDeserializer();
        private readonly PluginManager _pluginManager;
        private EmployeeSubject _subject = new EmployeeSubject();
        private FriendPluginAdapter _friendAdapter;

        public PluginManager PluginManager => _pluginManager;

        public ObservableCollection<IEmployee> Employees { get; set; } = new ObservableCollection<IEmployee>();
        public ObservableCollection<UIElement> PluginButtons { get; set; } = new ObservableCollection<UIElement>();

        // Collection for data processor plugins
        public ObservableCollection<UIElement> ProcessorMenuItems { get; set; } = new ObservableCollection<UIElement>();

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

        // Active data processor
        private IDataProcessor _activeProcessor;
        public IDataProcessor ActiveProcessor
        {
            get => _activeProcessor;
            set
            {
                _activeProcessor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ActiveProcessorName));
                OnPropertyChanged(nameof(IsProcessorActive));
            }
        }

        public string ActiveProcessorName => _activeProcessor?.PluginName ?? "None";
        public bool IsProcessorActive => _activeProcessor != null && _activeProcessor.IsEnabled;

        // Collection of all loaded data processors
        private List<IDataProcessor> _dataProcessors = new List<IDataProcessor>();

        public MainViewModel()
        {
            LoadSampleData();

            _pluginManager = new PluginManager();
            _pluginManager.PluginLoaded += OnPluginLoaded;
            _pluginManager.DataProcessorLoaded += OnDataProcessorLoaded;
            _pluginManager.LoadAllPlugins(this);

            PluginStatus = $"Loaded {_pluginManager.LoadedPlugins.Count} plugins";

            // Load friend's plugin with adapter - will be added LAST
            try
            {
                var friendPlugin = new ReverseEncryptor();
                _friendAdapter = new FriendPluginAdapter(friendPlugin);

                // CRITICAL FIX: Initialize the adapter with host reference
                _friendAdapter.Initialize(this);  // ← ADD THIS LINE!

                RegisterDataProcessor(_friendAdapter);

                // Delayed addition to ensure it appears LAST in menu
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var menuItem = _friendAdapter.GetUIElement();
                    if (menuItem != null)
                    {
                        ProcessorMenuItems.Add(menuItem);
                        System.Diagnostics.Debug.WriteLine("[Adapter] Friend plugin added LAST to menu");

                        // Optional: Also add to toolbar
                        var toolbarButton = _friendAdapter.GetToolbarButton();
                        if (toolbarButton != null)
                        {
                            PluginButtons.Add(toolbarButton);
                            System.Diagnostics.Debug.WriteLine("[Adapter] Friend plugin added to toolbar");
                        }
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);

                System.Diagnostics.Debug.WriteLine("[Adapter] Friend plugin loaded successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Adapter] Failed to load friend plugin: {ex.Message}");
            }

            // Observer pattern usage
            _subject.Attach(new LoggerObserver());

            // Log initial state
            _subject.Notify("Application started");
        }

        private void OnPluginLoaded(object sender, IPlugin plugin)
        {
            var uiElement = plugin.GetUIElement();
            if (uiElement != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (plugin.PluginId == "SeniorDeveloperPlugin")
                    {
                        PluginButtons.Insert(0, uiElement);
                    }
                    else
                    {
                        PluginButtons.Add(uiElement);
                    }

                    PluginStatus = $"Loaded {_pluginManager.LoadedPlugins.Count} plugins";
                });
            }
        }

        private void OnDataProcessorLoaded(object sender, IDataProcessor processor)
        {
            _dataProcessors.Add(processor);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var menuItem = processor.GetUIElement();
                if (menuItem != null)
                {
                    ProcessorMenuItems.Add(menuItem);
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Processor added: {processor.PluginName}, Total: {ProcessorMenuItems.Count}");
                }
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

            string oldName = SelectedEmployee.Name;

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

            // Observer notification
            _subject.Notify($"Employee changed: {oldName} → {SelectedEmployee.Name}");
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
                _subject.Notify($"Employee added: {emp.Name} ({emp.EmployeeType})");
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
                var empToDelete = SelectedEmployee;
                Employees.Remove(SelectedEmployee);
                SelectedEmployee = Employees.FirstOrDefault();
                _subject.Notify($"Employee deleted: {empToDelete.Name}");
            }
        }

        public void SaveToFile(string path)
        {
            try
            {
                var serializer = new TextSerializer();
                var lines = Employees.Select(e => serializer.SerializeDynamic(e));

                var allData = string.Join(Environment.NewLine, lines);

                if (ActiveProcessor != null && ActiveProcessor.IsEnabled)
                {
                    allData = ActiveProcessor.ProcessBeforeSave(allData);
                    ShowMessage($"Data encrypted with {ActiveProcessor.PluginName}", "Encryption Applied");
                }

                File.WriteAllText(path, allData);
                MessageBox.Show($"Successfully saved {Employees.Count} employees", "Success");
                _subject.Notify($"Saved {Employees.Count} employees to {Path.GetFileName(path)}");
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
                var allData = File.ReadAllText(path);

                if (ActiveProcessor != null && ActiveProcessor.IsEnabled)
                {
                    allData = ActiveProcessor.ProcessAfterLoad(allData);
                    ShowMessage($"Data decrypted with {ActiveProcessor.PluginName}", "Decryption Applied");
                }

                var lines = allData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var newList = new ObservableCollection<IEmployee>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

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
                    foreach (var emp in newList)
                    {
                        Employees.Add(emp);
                    }
                    SelectedEmployee = Employees.FirstOrDefault();
                    MessageBox.Show($"Successfully loaded {newList.Count} employees", "Success");
                    _subject.Notify($"Loaded {newList.Count} employees from {Path.GetFileName(path)}");
                }
                else
                {
                    MessageBox.Show("No valid employees found in file", "Warning");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error");
            }
        }

        public void SetActiveDataProcessor(IDataProcessor processor)
        {
            if (ActiveProcessor != null && ActiveProcessor != processor)
            {
                ActiveProcessor.IsEnabled = false;
            }

            ActiveProcessor = processor;

            if (processor != null)
            {
                ShowMessage($"Active processor set to: {processor.PluginName}", "Processor Changed");
                _subject.Notify($"Active processor changed to: {processor.PluginName}");
            }
        }

        private void RefreshList()
        {
            var selected = SelectedEmployee;

            var currentList = Employees.ToList();
            Employees.Clear();

            foreach (var emp in currentList)
            {
                Employees.Add(emp);
            }

            SelectedEmployee = selected;
            OnPropertyChanged(nameof(Employees));
        }

        // IPluginHost implementation
        public void AddEmployee(IEmployee employee)
        {
            if (employee != null)
            {
                Employees.Add(employee);
                PluginStatus = $"Total employees: {Employees.Count}";
                _subject.Notify($"Employee added via plugin: {employee.Name}");
            }
        }

        public void RemoveEmployee(IEmployee employee)
        {
            if (employee != null)
            {
                Employees.Remove(employee);
                PluginStatus = $"Total employees: {Employees.Count}";
                _subject.Notify($"Employee removed via plugin: {employee.Name}");
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

        // Data processor methods
        public void RegisterDataProcessor(IDataProcessor processor)
        {
            if (!_dataProcessors.Contains(processor))
            {
                _dataProcessors.Add(processor);
            }
        }

        public void UnregisterDataProcessor(IDataProcessor processor)
        {
            _dataProcessors.Remove(processor);
            if (ActiveProcessor == processor)
            {
                ActiveProcessor = null;
            }
        }

        public IDataProcessor GetActiveDataProcessor()
        {
            return ActiveProcessor;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}