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
                MessageBox.Show("Changes saved successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}