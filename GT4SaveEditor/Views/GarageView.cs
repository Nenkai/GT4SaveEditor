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
using System.Collections.ObjectModel;

using Ookii.Dialogs.Wpf;

using PDTools.SaveFile.GT4;
using PDTools.SaveFile.GT4.UserProfile;
using PDTools.Structures.PS2;

namespace GT4SaveEditor
{
    public partial class MainWindow
    {
        public ObservableCollection<CarEntity> GarageCars { get; set; } = new ObservableCollection<CarEntity>();

        private void InitGarageListing()
        {
            if (Save.GameData.Profile.Garage.RidingCarIndex != -1)
            {
                var car = Save.GameData.Profile.Garage.Cars[Save.GameData.Profile.Garage.RidingCarIndex];
                lb_CurrentCarName.Content = $"{_gt4Database.GetCarNameByCode(car.CarCode.Code)} - (Index: {Save.GameData.Profile.Garage.RidingCarIndex})";
                btn_EditCurrentCar.IsEnabled = true;
            }
            else
            {
                lb_CurrentCarName.Content = $"Not currently riding any car";
                btn_EditCurrentCar.IsEnabled = false;
            }

            int garageCarCount = Save.GameData.Profile.Garage.GetCarCount();
            for (var i = 0; i < garageCarCount; i++)
            {
                if (Save.GameData.Profile.Garage.Cars[i].IsFreeSlot())
                {
                    var car = Save.GameData.Profile.Garage.Cars[i];
                    string label = _gt4Database.GetCarLabelByCode(car.CarCode.Code);
                    uint color = _gt4Database.GetVariationRGBOfCarLabel(label, (int)car.VariationIndex);
                    Color col = Color.FromRgb((byte)(color), (byte)(color >> 8), (byte)(color >> 16));

                    GarageCars.Add(new CarEntity()
                    {
                        Index = i,
                        Name = _gt4Database.GetCarNameByCode(car.CarCode.Code),
                        Label = label,
                        Color = new SolidColorBrush(col),
                        CarData = car,
                    });
                }
            }

            gb_Garage.Header = $"Garage Cars ({garageCarCount}/1000)";
        }

        private void btn_EditCurrentCar_Click(object sender, RoutedEventArgs e)
        {
            if (Save.GameData.Profile.Garage.RidingCarIndex != -1)
            {
                var view = new CarGarageEditorWindow(Save.GameData.Profile.Garage.CurrentCar, _gt4Database);
                view.ShowDialog();
            }
        }

        private void mi_Garage_SetAsCurrentCar_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void mi_Garage_EditCar_Click(object sender, RoutedEventArgs e)
        {
            CarEntity entity = (CarEntity)lv_GarageCars.SelectedItem;
            if (!entity.CarData.GarageDataExists)
            {
                MessageBox.Show("This car was never ridden, no garage data exists yet.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var car = Save.GarageFile.GetCar((uint)entity.Index);
            if (car is null)
            {
                MessageBox.Show("Failed to fetch that car from the garage file, failed to decrypt it. " +
                    "This may be due to Float inaccuracies between PS2/PCSX2 and PC, cannot be solved.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var view = new CarGarageEditorWindow(car, _gt4Database);
            view.ShowDialog();

            Save.GarageFile.PushCar(car, Save.GarageFile.UniqueID, (uint)entity.Index);
        }

        public class CarEntity
        {
            public Brush Color { get; set; }
            public string Name { get; set; }
            public string Label { get; set; }
            public int Index { get; set; }
            public GarageScratchUnit CarData { get; set; }

            public override string ToString()
            {
                return $"{Name} (ID:{Index})";
            }
        }
    }
}
