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
    public partial class MainWindow
    {
        private void InitGarageListing()
        {
            if (Save.GameData.Profile.Garage.RidingCarIndex != -1)
            {
                var car = Save.GameData.Profile.Garage.Cars[Save.GameData.Profile.Garage.RidingCarIndex];
                lb_CurrentCarName.Content = $"{_gt4Database.GetCarNameByCode(car.CarCode.Code)}";
                btn_EditCurrentCar.IsEnabled = true;
            }
            else
            {
                lb_CurrentCarName.Content = $"Not currently riding any car";
                btn_EditCurrentCar.IsEnabled = false;
            }
        }

        private void btn_EditCurrentCar_Click(object sender, RoutedEventArgs e)
        {
            if (Save.GameData.Profile.Garage.RidingCarIndex != -1)
            {
                var view = new CarGarageEditorWindow(Save.GameData.Profile.Garage.CurrentCar, _gt4Database);
                view.ShowDialog();
            }
        }
    }
}
