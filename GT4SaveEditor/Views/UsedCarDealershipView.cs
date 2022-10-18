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

using PDTools.SaveFile.GT4.UserProfile;
using GT4SaveEditor.Database;

namespace GT4SaveEditor
{
    public partial class MainWindow
    {
        private void InitUsedCarListing()
        {
            int week = Save.GameData.Profile.UsedCar.Week;
            UsedCarWeekLineup lineup = _usedCarList.WeeklyLineups[week];

            lb_UCD_80.Items.Clear();
            for (var i = 0; i < lineup._80s.Count; i++)
            {
                var lbi = new ListBoxItem();
                if (!Save.GameData.Profile.UsedCar.IsCarSoldout(i))
                {
                    lbi.Foreground = Brushes.Black;
                    lbi.Content = _gt4Database.GetCarNameByLabel(lineup._80s[i].CarLabel);
                    lb_UCD_80.Items.Add(lbi);
                }
                else
                {
                    lbi.Foreground = Brushes.Gray;
                    lbi.Content = _gt4Database.GetCarNameByLabel(lineup._80s[i].CarLabel);
                    lb_UCD_80.Items.Add(lbi);
                }
            }

            lb_UCD_Early90.Items.Clear();
            for (var i = 0; i < lineup.Early90s.Count; i++)
            {
                var lbi = new ListBoxItem();
                if (!Save.GameData.Profile.UsedCar.IsCarSoldout(80 + i))
                {
                    lbi.Foreground = Brushes.Black;
                    lbi.Content = _gt4Database.GetCarNameByLabel(lineup.Early90s[i].CarLabel);
                    lb_UCD_Early90.Items.Add(lbi);
                }
                else
                {
                    lbi.Foreground = Brushes.Gray;
                    lbi.Content = _gt4Database.GetCarNameByLabel(lineup.Early90s[i].CarLabel);
                    lb_UCD_Early90.Items.Add(lbi);
                }
            }

            lb_UCD_Late90.Items.Clear();
            for (var i = 0; i < lineup.Late90s.Count; i++)
            {
                var lbi = new ListBoxItem();
                if (!Save.GameData.Profile.UsedCar.IsCarSoldout(160 + i))
                {
                    lbi.Foreground = Brushes.Black;
                    lbi.Content = _gt4Database.GetCarNameByLabel(lineup.Late90s[i].CarLabel);
                    lb_UCD_Late90.Items.Add(lbi);
                }
                else
                {
                    lbi.Foreground = Brushes.Gray;
                    lbi.Content = _gt4Database.GetCarNameByLabel(lineup.Late90s[i].CarLabel);
                    lb_UCD_Late90.Items.Add(lbi);
                }
            }
        }

        private void UpDown_UCDWeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!string.IsNullOrEmpty(_usedCarList.Region))
                InitUsedCarListing();
        }

        private void mi_UCD80_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (lb_UCD_80.SelectedIndex == -1)
                return;

            int index = lb_UCD_80.SelectedIndex;

            var lbi = lb_UCD_80.SelectedItem as ListBoxItem;
            if (Save.GameData.Profile.UsedCar.IsCarSoldout(UsedCar._80sCars_StartID + index))
            {
                Save.GameData.Profile.UsedCar.SetUsedCarStatus(UsedCar._80sCars_StartID + index, false);
                lbi.Foreground = Brushes.Black;
            }
            else
            {
                Save.GameData.Profile.UsedCar.SetUsedCarStatus(UsedCar._80sCars_StartID + index, true);
                lbi.Foreground = Brushes.Gray;
            }
        }

        private void mi_UCDEarly90_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (lb_UCD_Early90.SelectedIndex == -1)
                return;

            int index = lb_UCD_Early90.SelectedIndex;

            var lbi = lb_UCD_Early90.SelectedItem as ListBoxItem;
            if (Save.GameData.Profile.UsedCar.IsCarSoldout(UsedCar.Early90sCars_StartID + index))
            {
                Save.GameData.Profile.UsedCar.SetUsedCarStatus(UsedCar.Early90sCars_StartID + index, false);
                lbi.Foreground = Brushes.Black;
            }
            else
            {
                Save.GameData.Profile.UsedCar.SetUsedCarStatus(UsedCar.Early90sCars_StartID + index, true);
                lbi.Foreground = Brushes.Gray;
            }
        }

        private void mi_UCDLate90_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (lb_UCD_Late90.SelectedIndex == -1)
                return;

            int index = lb_UCD_Late90.SelectedIndex;

            var lbi = lb_UCD_Late90.SelectedItem as ListBoxItem;
            if (Save.GameData.Profile.UsedCar.IsCarSoldout(UsedCar.Late90sCars_StartID + index))
            {
                Save.GameData.Profile.UsedCar.SetUsedCarStatus(UsedCar.Late90sCars_StartID + index, false);
                lbi.Foreground = Brushes.Black;
            }
            else
            {
                Save.GameData.Profile.UsedCar.SetUsedCarStatus(UsedCar.Late90sCars_StartID + index, true);
                lbi.Foreground = Brushes.Gray;
            }
        }
    }
}
