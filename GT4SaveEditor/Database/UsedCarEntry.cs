
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT4SaveEditor.Database
{
    public class UsedCarEntry
    {
        public string CarLabel { get; set; }
        public int ColorIndex { get; set; }

        public UsedCarEntry(string carLabel, int colorIndex)
        {
            CarLabel = carLabel;
            ColorIndex = colorIndex;
        }

        public override string ToString()
        {
            return $"{CarLabel} ({ColorIndex}";
        }
    }
}
