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
using System.ComponentModel;

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

        private UsedCarList _usedCarList { get; set; } = new();
        private GT4Database _gt4Database { get; set; }

        private bool[] _profileTabNeedPopulate = new bool[3];

        public MainWindow()
        {
            InitializeComponent();

            for (var i = 0; i < _profileTabNeedPopulate.Length; i++)
                _profileTabNeedPopulate[i] = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // This is a merged database from all regions
            _gt4Database = new GT4Database("Resources/GT4.db");
            _gt4Database.CreateConnection();
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
                    OnSaveLoaded();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnSaveLoaded()
        {
            this.DataContext = Save;

            if (Save.GameType == GT4GameType.GT4_EU)
                _usedCarList.LoadList("EU");
            else if (Save.GameType == GT4GameType.GT4_JP)
                _usedCarList.LoadList("JP");
            else if (Save.GameType == GT4GameType.GT4_US)
                _usedCarList.LoadList("US");
            else if (Save.GameType == GT4GameType.GT4_KR)
                _usedCarList.LoadList("KR");

            MainTabControl.IsEnabled = true;
            MenuItem_Save.IsEnabled = true;
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
            // Fix this atrocity later
            Save.GarageFile.Money = ulong.Parse(label_Money.Text);
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tabControl_Profile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                if (_profileTabNeedPopulate[tabControl.SelectedIndex])
                {
                    if (tabControl.SelectedIndex == 1) // Garage
                    {
                        InitGarageListing();
                    }
                    else if (tabControl.SelectedIndex == 2) // Used Car
                    {
                        // Init UCD
                        InitUsedCarListing();
                    }

                    _profileTabNeedPopulate[tabControl.SelectedIndex] = false;
                }
            }
        }
    }
}
