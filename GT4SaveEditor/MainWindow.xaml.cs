using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Ookii.Dialogs.Wpf;

using PDTools.SaveFile.GT4;

namespace GT4SaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static GT4Save Save { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = "Select GT4 Save Directory";
            vistaOpenFileDialog.UseDescriptionForTitle = true;
            if (vistaOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    Save = GT4Save.Load(vistaOpenFileDialog.SelectedPath);
                    this.DataContext = Save;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = "Select GT4 Save Directory to save to";
            vistaOpenFileDialog.UseDescriptionForTitle = true;
            if (vistaOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    Save.SaveToDirectory(vistaOpenFileDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void label_Money_TextChanged(object sender, TextChangedEventArgs e)
        {
            Save.GarageFile.Money = ulong.Parse(label_Money.Text);
        }
    }
}
