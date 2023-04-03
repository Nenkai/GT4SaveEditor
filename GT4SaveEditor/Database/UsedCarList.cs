using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GT4SaveEditor.Database
{
    public class UsedCarDatabase
    {
        public List<UsedCarWeekLineup> WeeklyLineups { get; set; } = new();

        public string Region { get; set; }

        public void LoadList(string region)
        {
            if (region == Region)
                return; // Already loaded

            Region = region;

            WeeklyLineups.Clear();

            string[] lines = File.ReadAllLines(region);

            string currentCategory = "";
            UsedCarWeekLineup currentLineup = new UsedCarWeekLineup();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                    continue;

                string[] spl = line.Split('|');
                if (spl.Length < 2)
                    continue;

                if (spl[0] == "category")
                {
                    if (spl[1] == "usedcar_00_carlist" && currentCategory == "usedcar_02_carlist")
                    {
                        // New week
                        WeeklyLineups.Add(currentLineup);
                        currentLineup = new UsedCarWeekLineup();
                    }

                    currentCategory = spl[1];
                }
                else
                {
                    string carName = spl[0];
                    int.TryParse(spl[1], out int colorIndex);

                    var entry = new UsedCarEntry(carName, colorIndex);

                    if (currentCategory == "usedcar_00_carlist")
                        currentLineup._80s.Add(entry);
                    else if (currentCategory == "usedcar_01_carlist")
                        currentLineup.Early90s.Add(entry);
                    else if (currentCategory == "usedcar_02_carlist")
                        currentLineup.Late90s.Add(entry);
                }
            }

            WeeklyLineups.Add(currentLineup);
        }
    }
}
