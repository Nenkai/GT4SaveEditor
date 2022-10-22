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
    /// Interaction logic for CarVariationPickerWindow.xaml
    /// </summary>
    public partial class CarVariationPickerWindow : Window
    {
        public ObservableCollection<CarEntityViewModel> VariationModels { get; set; } = new ObservableCollection<CarEntityViewModel>();

        public int SelectedVariation { get; set; } = -1;

        private GT4Database _db;

        private List<(string Name, int RGB)> _colors;


        public CarVariationPickerWindow(List<(string, int)> colors)
        {
            _colors = colors;
            InitializeComponent();

            InitVariationListing();
        }

        public void InitVariationListing()
        {
            VariationModels.Clear();

            foreach (var row in _colors)
            {
                Color col = Color.FromRgb((byte)(row.RGB), (byte)(row.RGB >> 8), (byte)(row.RGB >> 16));
                CarEntityViewModel model = new CarEntityViewModel()
                {
                    Name = row.Name,
                    Color = new SolidColorBrush(col),
                };

                VariationModels.Add(model);
            }
        }

        private void lv_VariationSelector_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext;
            if (item is not CarEntityViewModel model)
                return;

            SelectedVariation = lv_CarColors.SelectedIndex;

            Close();
            return;

        }
    }
}
