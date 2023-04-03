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
using System.IO;

using Ookii.Dialogs.Wpf;

using PDTools.SaveFile.GT4;
using GT4SaveEditor.Database;
using Humanizer;
using System.Collections.ObjectModel;
using PDTools.SaveFile.GT4.UserProfile;

namespace GT4SaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string Version { get; set; } = "0.5.1";

        private GT4Save _save;
        public GT4Save Save 
        {
            get => _save;
            set { _save = value; OnPropertyChanged(nameof(Save)); }
        }

        private UsedCarDatabase _usedCarDb { get; set; } = new();
        private GT4Database _gt4Database { get; set; } = new();
        private EventDatabase _eventDb { get; set; } = new();
        public PresentCarDatabase _presentCarDb { get; set; } = new();
        public PresentCourseDatabase _presentCourseDb { get; set; } = new();

        private bool[] _profileTabNeedPopulate = new bool[5];

        public MainWindow()
        {
            InitializeComponent();

            UpdateTitle();

            for (var i = 0; i < _profileTabNeedPopulate.Length; i++)
                _profileTabNeedPopulate[i] = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _eventDb.Load("Resources/EventList.txt");
            _presentCarDb.Load("Resources/PresentCarList.txt");
            _presentCourseDb.Load("Resources/PresentCourseList.txt");


            foreach (var en in Enum.GetValues(typeof(GT4SaveType)))
            {
                if ((GT4SaveType)en == GT4SaveType.Unknown)
                    continue;

                MenuItem_SaveToAnotherRegion.Items.Add(new MenuItem()
                {
                    Header = $"_{((GT4SaveType)en).Humanize()}"
                });

            }
        }

        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = "Select GT4 Save Directory (example: BASCUS-97436GAMEDATA)";
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
                    Save.SaveToDirectory(vistaOpenFileDialog.SelectedPath);
                    MessageBox.Show($"Successfully saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuItem_SaveToAnotherRegion_Click(object sender, RoutedEventArgs e)
        {
            int idx = MenuItem_SaveToAnotherRegion.Items.IndexOf(e.OriginalSource);
            GT4SaveType type = (GT4SaveType)(idx + 1);

            if (type == Save.Type)
            {
                MessageBox.Show($"This save is already from that region.", "Hmm", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show($"Converting a save to another region will do the minimum to ensure that it will work for the specified region.\n\n" +
                    $"However, it may still break in certain cases (i.e using a car that doesn't exist in another region), so proceed with caution.\n" +
                    $"It is your responsibility if the game breaks in weird ways, it is expected on larger saves. Proceed?", "Warning",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            if ((GT4Save.IsGT4Online(type) != GT4Save.IsGT4Online(Save.Type)))
            {
                if (MessageBox.Show($"You are trying to convert from GT4<->GT4O.\n" +
                    $"Since the garage file cannot be decrypted correctly, all car data (tuning) will need to be deleted from the new save. Proceed?", "Hmm",
                    MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
                    return;
            }

            string gameDataname = GT4Save.GameDataRegionNames.FirstOrDefault(x => x.Value == type).Key;

            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = $"Select an extracted memory card directory (or an existing save '{gameDataname}' directory) to save to";
            vistaOpenFileDialog.UseDescriptionForTitle = true;

            if (vistaOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    string dir = System.IO.Path.Combine(vistaOpenFileDialog.SelectedPath, gameDataname);

                    string folderName = System.IO.Path.GetFileName(vistaOpenFileDialog.SelectedPath);
                    if (folderName == gameDataname)
                    {
                        dir = vistaOpenFileDialog.SelectedPath;
                    }
                    else if (GT4Save.GameDataRegionNames.ContainsKey(folderName))
                    {
                        MessageBox.Show($"This folder name is reserved for another region", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                    }

                    var newSave = new GT4Save();
                    Save.CopyTo(newSave);
                    newSave.ConvertToType(type);
                    newSave.SaveToDirectory(dir);

                    MessageBox.Show($"Successfully saved to {dir}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuItem_Encrypt_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = "Select GT4 Save Directory (example: BASCUS-97436GAMEDATA)";
            vistaOpenFileDialog.UseDescriptionForTitle = true;
            if (vistaOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    string gameType = GT4Save.DetectGameTypeFromSaveDirectory(vistaOpenFileDialog.SelectedPath);
                    if (string.IsNullOrEmpty(gameType))
                    {
                        MessageBox.Show($"No saves found in that memory card folder.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!GT4Save.GameDataRegionNames.TryGetValue(gameType, out GT4SaveType type))
                    {
                        MessageBox.Show($"Could not detect game type for that save file.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string encPath = System.IO.Path.Combine(vistaOpenFileDialog.SelectedPath, gameType);
                    string decPath = encPath + ".decrypted";

                    if (!File.Exists(decPath))
                    {
                        MessageBox.Show($"Original encrypted save file does not exist in that directory and is required to re-encrypt.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!File.Exists(decPath))
                    {
                        MessageBox.Show($"Decrypted save file does not exist in that directory.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    byte[] encFile = File.ReadAllBytes(encPath);
                    if (!GT4GameData.DecryptGameDataBuffer(encFile, out Memory<byte> saveBuffer, out bool useOldRandomUpdateCrypto))
                    {
                        MessageBox.Show($"Failed to decrypt the save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] decFile = File.ReadAllBytes(decPath);
                    int expectedSize = GT4GameData.GetExpectedGameDataSize(type);
                    if (decFile.Length != expectedSize)
                    {
                        MessageBox.Show($"Size of decrypted file does not match expected size (0x{expectedSize:X8}). Do not remove or append bytes to the decrypted save file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    byte[] reencrypted = GT4GameData.EncryptGameDataBuffer(decFile, useOldRandomUpdateCrypto);
                    File.WriteAllBytes(encPath, reencrypted);
                    MessageBox.Show($"Save successfully re-encrypted to '{encPath}'.\n" +
                        $"NOTE: If you are not using the save on PCSX2, remove the leftover .decrypted file.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the save: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuItem_Decrypt_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vistaOpenFileDialog = new VistaFolderBrowserDialog();
            vistaOpenFileDialog.Description = "Select GT4 Save Directory (example: BASCUS-97436GAMEDATA)";
            vistaOpenFileDialog.UseDescriptionForTitle = true;
            if (vistaOpenFileDialog.ShowDialog() == true)
            {
                try
                {
                    string gameType = GT4Save.DetectGameTypeFromSaveDirectory(vistaOpenFileDialog.SelectedPath);
                    if (string.IsNullOrEmpty(gameType))
                    {
                        MessageBox.Show($"No saves found in that memory card folder.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string encPath = System.IO.Path.Combine(vistaOpenFileDialog.SelectedPath, gameType);
                    if (!File.Exists(encPath))
                    {
                        MessageBox.Show($"Game Data file does not exist (file is usually named the same as the folder)", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    byte[] file = File.ReadAllBytes(encPath);
                    if (GT4GameData.DecryptGameDataBuffer(file, out Memory<byte> saveBuffer, out _))
                    {
                        string decPath = System.IO.Path.Combine(vistaOpenFileDialog.SelectedPath, gameType + ".decrypted");
                        File.WriteAllBytes(decPath, saveBuffer.ToArray());
                        MessageBox.Show($"Save successfully decrypted to '{decPath}'.\n\n\n" +
                            "Important Notes!\n" +
                            "- The garage file cannot be decrypted as it depends on shuffled crypto which might differ between hardware.\n\n" +
                            "- When re-encrypting, keep the original file in the directory and keep all file names including decrypted files identical.\n\n" +
                            "- Do not remove or append bytes to the decrypted file.", 
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Failed to decrypt the save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

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
                    else if (tabControl.SelectedIndex == 3)
                    {
                        InitPresentListing();
                    }
                    else if (tabControl.SelectedIndex == 4) // Used Car
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
            switch (Save.Type)
            {
                case GT4SaveType.Unknown:
                    break;
                case GT4SaveType.GT4_EU:
                    _usedCarDb.LoadList("Resources/UsedCarLineups/GT4_EU_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_EU2560.sqlite");
                    break;
                case GT4SaveType.GT4_US:
                case GT4SaveType.GT4O_US:
                    _usedCarDb.LoadList("Resources/UsedCarLineups/GT4_US_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_PREMIUM_US2560.sqlite");
                    break;
                case GT4SaveType.GT4_JP:
                case GT4SaveType.GT4O_JP:
                    _usedCarDb.LoadList("Resources/UsedCarLineups/GT4_JP_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_PREMIUM_JP2560.sqlite");
                    break;
                case GT4SaveType.GT4_KR:
                    _usedCarDb.LoadList("Resources/UsedCarLineups/GT4_KR_UCD.txt");
                    _gt4Database.CreateConnection("Resources/Databases/GT4_KR2560.sqlite");
                    break;
                default:
                    break;
            }

            for (var i = 0; i < _profileTabNeedPopulate.Length; i++)
                _profileTabNeedPopulate[i] = true;

            _eventDb.LoadEventIndices(Save.Type, _gt4Database);

            MainTabControl.IsEnabled = true;
            MenuItem_Save.IsEnabled = true;
            MenuItem_SaveToAnotherRegion.IsEnabled = true;

            UpdateTitle();

            tabControl_Profile.SelectedIndex = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool muteChanges = false;
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentDate = GameCalendar.SelectedDate.Value;
            TimeSpan elapsed = currentDate - PDTools.SaveFile.GT4.UserProfile.Calendar.GetOriginDate();

            muteChanges = true;
            iud_CurrentDay.Value = (int)elapsed.TotalDays;
            iud_CurrentWeek.Value = (int)elapsed.TotalDays / 7;
            muteChanges = false;

            UpdateTitle();
        }

        private void iud_CurrentDay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (muteChanges)
                return;

            DateTime newDate = PDTools.SaveFile.GT4.UserProfile.Calendar.GetOriginDate() + TimeSpan.FromDays((int)iud_CurrentDay.Value);
            GameCalendar.SelectedDate = newDate;

            UpdateTitle();
        }

        private void iud_CurrentWeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (muteChanges)
                return;

            DateTime newDate = PDTools.SaveFile.GT4.UserProfile.Calendar.GetOriginDate() + TimeSpan.FromDays((int)iud_CurrentWeek.Value * 7);
            GameCalendar.SelectedDate = newDate;

            UpdateTitle();
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"GT4 Save Editor - Version {this.Version} by Nenkai#9075\n" +
                $"Credits:\n" +
                $"- Hatersbby, Submaniac, pez2k - Providing saves from various regions", "About Window");
        }

        private void label_ProfileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTitle();
        }

        private void upd_Money_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateTitle();
        }

        public void UpdateTitle()
        {
            this.Title = $"GT4 Save Editor {Version}";

            if (Save != null)
            {
                
                this.Title += $" | {Save.Type.Humanize()} | " +
                    $"{Save.GameData.Profile.UserName} / Cr. {Save.GarageFile.Money} / ";

                if (_gt4Database.Connected)
                {
                    if (Save.GameData.Profile.Garage.RidingCarIndex == -1)
                        this.Title += "Current Car: None / ";
                    else
                    {
                        GarageScratchUnit car = Save.GameData.Profile.Garage.Cars[Save.GameData.Profile.Garage.RidingCarIndex];
                        this.Title += $"Current Car: {_gt4Database.GetCarNameByCode(car.CarCode.Code)} / ";
                    }
                }

                this.Title += $"{Save.GameData.Profile.Garage.GetCarCount()} Car(s)";
            }
        }
    }
}
