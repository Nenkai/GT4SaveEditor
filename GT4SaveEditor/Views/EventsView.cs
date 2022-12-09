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

            cb_EventType.Items.Clear();
            foreach (var i in Enum.GetValues<EventType>())
                cb_EventType.Items.Add(i.Humanize());

            cb_EventResult.Items.Clear();
            foreach (var i in Enum.GetValues<Result>())
                cb_EventResult.Items.Add(i.Humanize());

            cb_EventCurrentResult.Items.Clear();
            foreach (var i in Enum.GetValues<Result>())
                cb_EventCurrentResult.Items.Add(i.Humanize());

            cb_EventLicenseOrMissionResult.Items.Clear();
            foreach (var i in Enum.GetValues<Result>())
                cb_EventLicenseOrMissionResult.Items.Add(i.Humanize());
        }

        #region Listbox Selection Changed
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
            ByteUpDown_EventAspecPoints.Value = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex].ASpecScore;
            cb_EventType.SelectedIndex = (int)unit.GetEventType();
            cb_EventResult.SelectedIndex = (int)unit.GetPermanentResult();
            cb_EventCurrentResult.SelectedIndex = (int)unit.GetCurrentResult();
            cb_EventLicenseOrMissionResult.SelectedIndex = (int)unit.GetUnknownLicenseOrMissionResult();
        }

        private void cb_EventType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetEventType((EventType)cb_EventType.SelectedIndex);
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
            unit.SetCurrentResult((Result)cb_EventCurrentResult.SelectedIndex);
        }

        private void cb_EventLicenseOrMissionResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetLicenseOrMissionResult((Result)cb_EventLicenseOrMissionResult.SelectedIndex);
        }

        private void ByteUpDown_EventAspecPoints_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.ASpecScore = (byte)ByteUpDown_EventAspecPoints.Value;
        }

        #endregion

        #region Category Context Menu Events
        /// <summary>
        /// Sets all category events to 1st
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_Categories_SetCategoryAllFirstEvents_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventTypeWithResult(selectedCategory, EventType.Event, Result._1);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
            {
                cb_EventType.SelectedIndex = (int)EventType.Event;
                cb_EventResult.SelectedIndex = (int)Result._1;
                cb_EventCurrentResult.SelectedIndex = (int)Result._1;
                cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.none;
            }
        }

        /// <summary>
        /// Sets all category licenses to gold
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_Categories_SetCategoryAllGoldLicenses_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventTypeWithResult(selectedCategory, EventType.License, Result.gold);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
            {
                cb_EventType.SelectedIndex = (int)EventType.License;
                cb_EventResult.SelectedIndex = (int)Result.gold;
                cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.gold;
                cb_EventCurrentResult.SelectedIndex = (int)Result.none;
            }
        }

        /// <summary>
        /// Sets all category missions to gold
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_Categories_SetCategoryAllGoldMissions_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventTypeWithResultAndScore(selectedCategory, EventType.Mission, Result._1, 250);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
            {
                cb_EventType.SelectedIndex = (int)EventType.Mission;
                cb_EventResult.SelectedIndex = (int)Result._1;
                cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result._1;
                cb_EventCurrentResult.SelectedIndex = (int)Result.none;
                ByteUpDown_EventAspecPoints.Value = 250;
            }
        }

        /// <summary>
        /// Resets category events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_Categories_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            ResetCategoryEvents(selectedCategory);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
            {
                cb_EventType.SelectedIndex = (int)EventType.None;
                cb_EventResult.SelectedIndex = (int)Result.none;
                cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.none;
                cb_EventCurrentResult.SelectedIndex = (int)Result.none;
                ByteUpDown_EventAspecPoints.Value = 0;
            }
        }

        /// <summary>
        /// Resets category event progress but keep aspec points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_Categories_ResetKeepAspec_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventResult(selectedCategory, Result.none);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
            {
                cb_EventResult.SelectedIndex = (int)Result.none;
                cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.none;
                cb_EventCurrentResult.SelectedIndex = (int)Result.none;
            }
        }

        private void lb_Categories_SetAll0Points_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventScore(selectedCategory, 0);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
                ByteUpDown_EventAspecPoints.Value = 0;
        }

        private void lb_Categories_SetAll200Points_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventScore(selectedCategory, 200);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
                ByteUpDown_EventAspecPoints.Value = 200;
        }

        private void lb_Categories_SetAll254Points_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex == -1)
                return;

            EventCategory selectedCategory = (EventCategory)lb_Categories.SelectedItem;
            SetCategoryEventScore(selectedCategory, 254);

            if (lb_Events.SelectedItem != null && selectedCategory.Events.Contains(lb_Events.SelectedItem as GameEvent))
                ByteUpDown_EventAspecPoints.Value = 254;
        }
        #endregion

        #region Events Context Menu Events
        private void lb_Events_SetAsEvent1st_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex != -1 && lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetEventType(EventType.Event);
            unit.SetPermanentResult(Result._1);
            unit.SetLicenseOrMissionResult(Result.none);
            unit.SetCurrentResult(Result._1);

            cb_EventResult.SelectedIndex = (int)Result._1;
            cb_EventCurrentResult.SelectedIndex = (int)Result._1;
            cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.none;
            cb_EventType.SelectedIndex = (int)EventType.Event;
        }

        private void lb_Events_SetAsGoldedLicense_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex != -1 && lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetEventType(EventType.License);
            unit.SetPermanentResult(Result.gold);
            unit.SetLicenseOrMissionResult(Result.gold);
            unit.SetCurrentResult(Result.none);

            cb_EventResult.SelectedIndex = (int)Result.gold;
            cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.gold;
            cb_EventCurrentResult.SelectedIndex = (int)Result.none;
            cb_EventType.SelectedIndex = (int)EventType.License;
        }

        private void lb_Events_SetAsMission1st_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex != -1 && lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetEventType(EventType.License);
            unit.SetPermanentResult(Result._1);
            unit.SetLicenseOrMissionResult(Result._1);
            unit.SetCurrentResult(Result.none);

            cb_EventResult.SelectedIndex = (int)Result._1;
            cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result._1;
            cb_EventCurrentResult.SelectedIndex = (int)Result.none;
            cb_EventType.SelectedIndex = (int)EventType.Mission;
            ByteUpDown_EventAspecPoints.Value = 250;
        }

        private void lb_Events_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Categories.SelectedIndex != -1 && lb_Events.SelectedIndex == -1)
                return;

            GameEvent @event = (GameEvent)lb_Events.SelectedItem;
            RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
            unit.SetEventType(EventType.None);
            unit.SetPermanentResult(Result.none);
            unit.SetCurrentResult(Result.none);
            unit.SetLicenseOrMissionResult(Result.none);
            unit.ASpecScore = 0;

            cb_EventResult.SelectedIndex = (int)Result.none;
            cb_EventLicenseOrMissionResult.SelectedIndex = (int)Result.none;
            cb_EventCurrentResult.SelectedIndex = (int)Result.none;
            cb_EventType.SelectedIndex = (int)EventType.None;
            ByteUpDown_EventAspecPoints.Value = 0;
        }

        #endregion

        /// <summary>
        /// Sets the score for a category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="score"></param>
        private void SetCategoryEventScore(EventCategory category, byte score)
        {
            foreach (var @event in category.Events)
            {
                RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
                unit.ASpecScore = score;
            }
        }

        /// <summary>
        /// Sets the result for a category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="result"></param>
        private void SetCategoryEventResult(EventCategory category, Result result)
        {
            foreach (var @event in category.Events)
            {
                RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
                unit.SetPermanentResult(result);
                unit.SetLicenseOrMissionResult(result);
                unit.SetCurrentResult(result);
            }
        }

        /// <summary>
        /// Sets the event type and result for a category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventType"></param>
        /// <param name="result"></param>
        private void SetCategoryEventTypeWithResult(EventCategory category, EventType eventType, Result result)
        {
            foreach (var @event in category.Events)
            {
                RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
                unit.SetEventType(eventType);
                unit.SetPermanentResult(result);

                if (eventType == EventType.Event)
                {
                    unit.SetLicenseOrMissionResult(Result.none);
                    unit.SetCurrentResult(result);
                }
                else if (eventType != EventType.None)
                {
                    unit.SetLicenseOrMissionResult(result);
                    unit.SetCurrentResult(Result.none);
                }
            }
        }

        /// <summary>
        /// Sets the event type and result for a category, with specified score
        /// </summary>
        private void SetCategoryEventTypeWithResultAndScore(EventCategory category, EventType eventType, Result result, byte score)
        {
            foreach (var @event in category.Events)
            {
                RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
                unit.SetEventType(eventType);
                unit.SetPermanentResult(result);

                if (eventType == EventType.Event)
                {
                    unit.SetLicenseOrMissionResult(Result.none);
                    unit.SetCurrentResult(result);
                }
                else if (eventType != EventType.None)
                {
                    // This one is set for licenses & missions
                    unit.SetLicenseOrMissionResult(result);
                    unit.SetCurrentResult(Result.none);
                }

                unit.ASpecScore = score;
            }
        }

        /// <summary>
        /// Sets all events in category
        /// </summary>
        /// <param name="category"></param>
        private void ResetCategoryEvents(EventCategory category)
        {
            foreach (var @event in category.Events)
            {
                RaceRecordUnit unit = Save.GameData.Profile.RaceRecords.Records[@event.DbIndex];
                unit.SetEventType(EventType.None);
                unit.SetPermanentResult(Result.none);
                unit.SetCurrentResult(Result.none);
                unit.SetLicenseOrMissionResult(Result.none);
                unit.ASpecScore = 0;
            }
        }

        private void PopulateEventsFromCategory(EventCategory category)
        {
            lb_Events.Items.Clear();

            foreach (var @event in category.Events)
            {
                lb_Events.Items.Add(@event);
            }

            gb_EventResultSettings.IsEnabled = lb_Events.SelectedIndex != -1;
        }
    }
}
