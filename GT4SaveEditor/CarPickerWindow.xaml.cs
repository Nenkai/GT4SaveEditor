using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Ookii.Dialogs.Wpf;

using PDTools.SaveFile.GT4;
using PDTools.SaveFile.GT4.UserProfile;
using PDTools.Structures.PS2;

using GT4SaveEditor.ViewModels;
using GT4SaveEditor.Database;

namespace GT4SaveEditor
{
    /// <summary>
    /// Interaction logic for CarPickerWindow.xaml
    /// </summary>
    public partial class CarPickerWindow : Window
    {
        public static ObservableCollection<CarEntityViewModel> GameCars { get; set; } = new ObservableCollection<CarEntityViewModel>();

        public static GT4GameType LoadedGameType { get; set; }

        public string SelectedLabel { get; set; }
        public int SelectedCarCode { get; set; }
        public int SelectedVariation { get; set; }
        private GT4Database _db;

        public CarPickerWindow(GT4Database db)
        {
            _db = db;
            InitializeComponent();
        }

        public static void InitCarListing(GT4Database db)
        {
            GameCars.Clear();

            foreach (var row in db.GetAllCarLabel_Code_Name())
            {
                CarEntityViewModel model = new CarEntityViewModel()
                {
                    Index = row.ID,
                    Name = row.Name,
                    Label = row.Label,
                };

                GameCars.Add(model);
            }
        }

        private void lv_CarSelector_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext;
            if (item is not CarEntityViewModel model)
                return;

            SelectedLabel = model.Label;
            SelectedCarCode = model.Index;

            // Check for potential colors
            var colors = _db.GetVariationNameAndRGBOfCar(SelectedLabel);

            if (colors.Count > 1)
            {
                var colorPickerView = new CarVariationPickerWindow(colors);
                colorPickerView.Owner = this;
                colorPickerView.ShowDialog();

                if (colorPickerView.SelectedVariation == -1)
                    return;

                SelectedVariation = colorPickerView.SelectedVariation;
            }

            Close();
        }
    }
}
