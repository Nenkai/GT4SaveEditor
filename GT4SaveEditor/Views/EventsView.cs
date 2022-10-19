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

using Humanizer;

using PDTools.SaveFile.GT4;
using PDTools.SaveFile.GT4.UserProfile;
using PDTools.Structures.PS2;
using PDTools.Enums.PS2;

using GT4SaveEditor.Database;

namespace GT4SaveEditor
{
    public partial class MainWindow
    {
        private void InitEventListing()
        {
            foreach (EventCategory category in _eventDb.Categories)
            {
                lb_Categories.Items.Add(category);
            }

            PopulateEventsFromCategory(_eventDb.Categories[0]);

            cb_EventResult.Items.Clear();
            foreach (var i in Enum.GetValues<Result>())
                cb_EventResult.Items.Add(i.Humanize());

            cb_EventCurrentResult.Items.Clear();
            foreach (var i in Enum.GetValues<Result>())
                cb_EventCurrentResult.Items.Add(i.Humanize());
        }

        private void lb_Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            PopulateEventsFromCategory(selectedCategory);
        }

        private void lb_Events_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            gb_EventResultSettings.IsEnabled = true;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            ByteUpDown_EventAspecPoints.Value = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex].ASpecPoints;
            cb_EventResult.SelectedIndex = (int)unit.GetPermanentResult();
            cb_EventCurrentResult.SelectedIndex = (int)unit.GetCurrentResult();
        }

        private void cb_EventResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetPermanentResult((Result)cb_EventResult.SelectedIndex);
        }

        private void cb_EventCurrentResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetCurrentResult((Result)cb_EventResult.SelectedIndex);
        }

        private void ByteUpDown_EventAspecPoints_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.ASpecPoints = (byte)ByteUpDown_EventAspecPoints.Value;
        }

        private void PopulateEventsFromCategory(EventCategory category)
        {
            lb_Events.Items.Clear();

            foreach (var @event in category.Events)
            {
                lb_Events.Items.Add(@event);
            }
        }
    }
}
