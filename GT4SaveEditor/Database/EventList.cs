using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GT4SaveEditor.Database
{
    public class EventList
    {
        public List<EventCategory> Categories { get; set; } = new();

        public void Load(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                    continue;

                string[] spl = line.Split('|');
                if (spl.Length <= 1)
                    continue;

                EventCategory category = new EventCategory();
                category.Name = spl[0];

                for (var i = 1; i < spl.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spl[i]))
                        category.Events.Add(new GameEvent() { Label = spl[i] });
                }

                Categories.Add(category);
            }
        }

        public void LoadEventIndices(GT4Database database)
        {
            foreach (EventCategory category in Categories)
            {
                foreach (GameEvent @event in category.Events)
                {
                    (int RowID, int CourseID, string GameMode) @eventData = database.GetRaceRowIndexByLabel(@event.Label);
                    @event.DbIndex = eventData.RowID;
                    @event.CourseLabel = database.GetCourseLabelByID(eventData.CourseID);
                    @event.GameMode = eventData.GameMode;
                }
            }
        }
    }

    public class EventCategory
    {
        public string Name { get; set; }

        public List<GameEvent> Events { get; set; } = new();

        public override string ToString()
        {
            return $"{Name} ({Events.Count} event(s))";
        }
    }

    public class GameEvent
    {
        public string CourseLabel { get; set; }
        public string GameMode { get; set; }
        public string Label { get; set; }
        public int DbIndex { get; set; }

        public override string ToString()
        {
            if (GameMode == "RACE_MODE_SINGLE")
                return $"Event ({CourseLabel})";
            else if (GameMode == "RACE_MODE_MISSION")
                return $"Mission ({CourseLabel})";
            else if (GameMode == "RACE_MODE_LICENSE")
                return $"License ({CourseLabel})";

            return "";
        }
    }
}
