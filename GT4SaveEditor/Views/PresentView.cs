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
        public ObservableCollection<SavePresentItem> PresentCarUnlocks { get; set; } = new();
        public ObservableCollection<SavePresentItem> PresentCourseUnlocks { get; set; } = new();

        public void InitPresentListing()
        {
            PresentCarUnlocks.Clear();
            foreach (PresentCarEntry i in _presentCarDb.Presents)
                PresentCarUnlocks.Add(new SavePresentItem(Save.GameData.Profile.Presents, $"{i.Name} ({i.Label})", i.Index));

            PresentCourseUnlocks.Clear();
            foreach (PresentCourseEntry i in _presentCourseDb.Presents)
                PresentCourseUnlocks.Add(new SavePresentItem(Save.GameData.Profile.Presents, i.Name, Present.CourseIndexStart + i.Index));
        }

        public class SavePresentItem
        {
            public int Index { get; set; }
            public string Name { get; set; }

            public Present SavePresentEntity { get; set; }

            public bool IsChecked
            {
                get => SavePresentEntity.IsUnlocked(Index);
                set => SavePresentEntity.SetUnlocked(Index, value);
            }

            public SavePresentItem(Present savePresent, string name, int index)
            {
                SavePresentEntity = savePresent;
                Name = name;
                Index = index;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
