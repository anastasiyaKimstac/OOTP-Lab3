using System;
using System.Windows;
using Microsoft.Win32;

namespace OOTP_Lab3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            string type = button?.Content.ToString() ?? "";
            ViewModel.AddEmployee(type);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedEmployee != null)
            {
                if (MessageBox.Show($"Delete {ViewModel.SelectedEmployee.Name}?", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ViewModel.DeleteEmployee();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SaveChanges();
                MessageBox.Show("Changes saved successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error");
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = ".txt",
                FileName = "employees.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                ViewModel.SaveToFile(dialog.FileName);
            }
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = ".txt"
            };

            if (dialog.ShowDialog() == true)
            {
                ViewModel.LoadFromFile(dialog.FileName);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoadPlugin_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Plugin DLLs (*.dll)|*.dll",
                Title = "Select Plugin DLL"
            };

            if (dialog.ShowDialog() == true)
            {
                // Need to add this method to PluginManager
                var result = ViewModel.PluginManager.LoadPluginFromFile(dialog.FileName, ViewModel);
                if (result)
                {
                    MessageBox.Show("Plugin loaded successfully!", "Success");
                }
                else
                {
                    MessageBox.Show("Failed to load plugin!", "Error");
                }
            }
        }

        private void ClearProcessor_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ActiveProcessor != null)
            {
                ViewModel.ActiveProcessor.IsEnabled = false;
                ViewModel.SetActiveDataProcessor(null);
                MessageBox.Show("Active data processor cleared!", "Processor Cleared");
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Employee Management System\n\n" +
                "Version: 5.0\n" +
                "Features:\n" +
                "• Plugin architecture for employee types\n" +
                "• Data processing plugins (encryption/decryption)\n" +
                "• Dynamic UI generation\n" +
                "• Save/Load with data processing\n\n" +
                "Created for OOTP Lab #5",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}