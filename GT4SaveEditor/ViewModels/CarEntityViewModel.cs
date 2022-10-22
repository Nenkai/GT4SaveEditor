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

using PDTools.SaveFile.GT4;
using PDTools.SaveFile.GT4.UserProfile;
using PDTools.Structures.PS2;

namespace GT4SaveEditor.ViewModels
{
    public class CarEntityViewModel
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
