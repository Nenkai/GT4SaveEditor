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
using PDTools.Structures;
using PDTools.Enums.PS2;

using GT4SaveEditor.ViewModels;

namespace GT4SaveEditor
{
    public partial class MainWindow
    {
        public ObservableCollection<CarEntityViewModel> GarageCars { get; set; } = new ObservableCollection<CarEntityViewModel>();

        private void InitGarageListing()
        {
            GarageCars.Clear();

            UpdateCurrentCarStatus();

            int garageCarCount = Save.GameData.Profile.Garage.GetCarCount();
            for (var i = 0; i < garageCarCount; i++)
            {
                if (Save.GameData.Profile.Garage.Cars[i].IsSlotTaken)
                {
                    CarEntityViewModel model = CreateGarageCarModel(i, Save.GameData.Profile.Garage.Cars[i]);
                    GarageCars.Add(model);
                }
            }

            btn_Garage_AddCar.IsEnabled = garageCarCount < GarageScratch.MAX_CARS;
            gb_Garage.Header = $"Garage Cars ({garageCarCount}/1000)";
        }

        private void btn_EditCurrentCar_Click(object sender, RoutedEventArgs e)
        {
            if (Save.GameData.Profile.Garage.RidingCarIndex != -1)
            {
                var view = new CarGarageEditorWindow(Save.GameData.Profile.Garage.CurrentCar, _gt4Database);
                view.Owner = this;
                view.ShowDialog();
            }
        }

        private void mi_Garage_DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (lv_GarageCars.SelectedItem is not CarEntityViewModel)
                return;

            for (int i = lv_GarageCars.SelectedItems.Count - 1; i >= 0; i--)
            {
                object? item = lv_GarageCars.SelectedItems[i];
                var selected = item as CarEntityViewModel;
                if (selected.Index == Save.GameData.Profile.Garage.RidingCarIndex)
                    Save.GameData.Profile.Garage.RidingCarIndex = -1;

                selected.CarData.IsSlotTaken = false;
                GarageCars.Remove(selected);
            }

            UpdateCurrentCarStatus();
            UpdateTitle();
        }

        private void btn_Garage_AddCar_Click(object sender, RoutedEventArgs e)
        {
            if (Save.GameData.Profile.Garage.IsFull())
                return; // This button should be disabled, but just incase

            if (Save.Type != CarPickerWindow.LoadedGameType)
            {
                CarPickerWindow.InitCarListing(_gt4Database);
                CarPickerWindow.LoadedGameType = Save.Type;
            }

            var view = new CarPickerWindow(_gt4Database);
            view.Owner = this;
            view.ShowDialog();

            if (string.IsNullOrEmpty(view.SelectedLabel))
                return;

            // Find first free slot to use
            int firstFreeIndex = Save.GameData.Profile.Garage.GetFirstUnusedSlotIndex();

            // Create the unit
            GarageScratchUnit unit = new GarageScratchUnit();
            unit.CarCode = new DbCode(view.SelectedCarCode, (int)PartsTypeGT4.GENERIC_CAR);
            unit.IsSlotTaken = true;
            unit.Odometer = 0;
            unit.VariationIndex = (uint)view.SelectedVariation;

            // Set it
            Save.GameData.Profile.Garage.Cars[firstFreeIndex] = unit;

            CarEntityViewModel entity = CreateGarageCarModel(firstFreeIndex, unit);
            GarageCars.Add(entity);

            UpdateCurrentCarStatus();
            UpdateTitle();
        }

        private void btn_Garage_AddAllMissingCars_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will add all the missing cars into your garage. Continue?", "Prompt",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var ids = _gt4Database.GetAllCarCodes();
            int added = 0;

            for (var i = 0; i < ids.Count; i++)
            {
                if (!Save.GameData.Profile.Garage.HasCarCode(ids[i]))
                {
                    GarageScratchUnit unit = new GarageScratchUnit();
                    unit.CarCode = new DbCode(ids[i], (int)PartsTypeGT4.GENERIC_CAR);
                    unit.IsSlotTaken = true;

                    int firstFreeIndex = Save.GameData.Profile.Garage.GetFirstUnusedSlotIndex();
                    if (firstFreeIndex == -1)
                        break;

                    Save.GameData.Profile.Garage.Cars[firstFreeIndex] = unit;
                    CarEntityViewModel entity = CreateGarageCarModel(firstFreeIndex, unit);
                    GarageCars.Add(entity);

                    added++;
                }
            }

            UpdateTitle();
        }


        private void btn_Garage_Wipe_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will WIPE your garage. Continue? (You still can revert this by not saving).", "Prompt",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            Save.GameData.Profile.Garage.Clear();
            Save.GameData.Profile.Garage.RidingCarIndex = -1;

            GarageCars.Clear();
            UpdateCurrentCarStatus();

            UpdateTitle();
        }

        private CarEntityViewModel CreateGarageCarModel(int index, GarageScratchUnit car)
        {
            string label = _gt4Database.GetCarLabelByCode(car.CarCode.Code);
            uint color = _gt4Database.GetVariationRGBOfCarLabel(label, (int)car.VariationIndex) ?? 0;
            Color col = Color.FromRgb((byte)(color), (byte)(color >> 8), (byte)(color >> 16));

            var model = new CarEntityViewModel()
            {
                Index = index,
                Name = _gt4Database.GetCarNameByCode(car.CarCode.Code),
                Label = label,
                Color = new SolidColorBrush(col),
                CarData = car,
            };

            return model;
        }

        private void UpdateCurrentCarStatus()
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
            gb_Garage.Header = $"Garage Cars ({garageCarCount}/1000)";
        }
    }
}
