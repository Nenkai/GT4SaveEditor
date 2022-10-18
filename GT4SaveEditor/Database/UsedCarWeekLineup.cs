
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT4SaveEditor.Database
{
    public class UsedCarWeekLineup
    {
        public List<UsedCarEntry> _80s { get; set; } = new List<UsedCarEntry>();
        public List<UsedCarEntry> Early90s { get; set; } = new List<UsedCarEntry>();
        public List<UsedCarEntry> Late90s { get; set; } = new List<UsedCarEntry>();

    }
}
