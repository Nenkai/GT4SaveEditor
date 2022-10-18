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
using GT4SaveEditor.Database;

namespace GT4SaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private GT4Save _save;
        public GT4Save Save 
        {
            get => _save;
            set { _save = value; OnPropertyChanged(nameof(Save)); }
        }

        private UsedCarList _usedCarList { get; set; } = new();
        private GT4Database _gt4Database { get; set; } = new();
        private EventList _eventList { get; set; } = new();

        private bool[] _profileTabNeedPopulate = new bool[4];

        public MainWindow()
        {
            InitializeComponent();

            for (var i = 0; i < _profileTabNeedPopulate.Length; i++)
                _profileTabNeedPopulate[i] = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _eventList.Load("Resources/EventList.txt");

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


        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = "Select GT4 Save Directory to save to";
            vistaOpenFileDialog.UseDescriptionForTitle = true;
            if (vistaOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    Save.GameData.Profile.RaceRecords.Records[16].ASpecPoints = 251;
                    Save.SaveToDirectory(vistaOpenFileDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                    if (tabControl.SelectedIndex == 1)
                    {
                        InitEventListing();
                    }
                    else if (tabControl.SelectedIndex == 2) // Garage
                    {
                        InitGarageListing();
                    }
                    else if (tabControl.SelectedIndex == 3) // Used Car
                    {
                        // Init UCD
                        InitUsedCarListing();
                    }

                    _profileTabNeedPopulate[tabControl.SelectedIndex] = false;
                }
            }
        }

        private void OnSaveLoaded()
        {
            switch (Save.GameType)
            {
                case GT4GameType.Unknown:
                    break;
                case GT4GameType.GT4_EU:
                    _usedCarList.LoadList("Resources/UsedCarLineups/GT4_CN_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_EU2560.sqlite");
                    break;
                case GT4GameType.GT4_US:
                case GT4GameType.GT4O_US:
                    _usedCarList.LoadList("Resources/UsedCarLineups/GT4_US_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_PREMIUM_US2560.sqlite");
                    break;
                case GT4GameType.GT4_JP:
                case GT4GameType.GT4O_JP:
                    _usedCarList.LoadList("Resources/UsedCarLineups/GT4_JP_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_PREMIUM_JP2560.sqlite");
                    break;
                case GT4GameType.GT4_KR:
                    _usedCarList.LoadList("Resources/UsedCarLineups/GT4_KR_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_KR2560.sqlite");
                    break;
                default:
                    break;
            }

            _eventList.LoadEventIndices(_gt4Database);

            MainTabControl.IsEnabled = true;
            MenuItem_Save.IsEnabled = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
