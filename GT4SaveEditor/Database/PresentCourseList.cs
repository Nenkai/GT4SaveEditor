using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

using PDTools.SaveFile.GT4;

namespace GT4SaveEditor.Database
{
    public class PresentCourseDatabase
    {
        public List<PresentCourseEntry> Presents { get; set; } = new();

        public void Load(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                string? line = lines[i];
                if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                    continue;

                string[] spl = line.Split('|');
                if (spl.Length <= 1)
                    continue;

                PresentCourseEntry present = new PresentCourseEntry();
                present.Name = spl[0];
                present.Label = spl[1];
                present.Condition = spl[2];
                
                present.Index = i;

                Presents.Add(present);
            }
        }
    }

    public class PresentCourseEntry
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public string Condition { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
