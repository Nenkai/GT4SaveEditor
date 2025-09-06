using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace GT4SaveEditor.Database
{
    public class GT4Database
    {
        private SQLiteConnection _sConn;
        public string FileName { get; set; }

        public bool Connected { get; set; }

        private Dictionary<int, CachedCar> _cachedCarsByCode = [];
        private Dictionary<string, CachedCar> _cachedCarsByLabel = [];
        private Dictionary<(int, int), uint> _cachedVariationColors = [];

        private Dictionary<int, CachedRace> _cachedRacesByCode = [];
        private Dictionary<string, CachedRace> _cachedRacesByLabel = [];

        private Dictionary<int, string> _cachedCourseLabels = [];

        public class CachedCar
        {
            public string Label { get; set; }
            public string Name { get; set; }
            public int VariationID { get; set; }
        }

        public class CachedRace
        {
            public int RowId { get; set; }
            public int CourseId { get; set; }
            public string RaceMode { get; set; }
        }


        public GT4Database()
        {

        }


        public bool CreateConnection(string fileName)
        {
            if (fileName == FileName)
                return true; // Already loaded

            FileName = fileName;
            _sConn = new SQLiteConnection($"Data Source={fileName};Version=3;New=False;Compress=True;");

            try
            {
                _sConn.Open();
                CacheDbCarInfo();
                Connected = true;
            }
            catch (Exception e)
            {
                Connected = false;
                return false;
            }

            return true;
        }

        private void CacheDbCarInfo()
        {
            if (_cachedCarsByLabel.Count == 0)
            {
                var res = ExecuteQuery($"SELECT RowId, Label FROM GENERIC_CAR");
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        int dbCode = res.GetInt32(0);
                        string label = res.GetString(1);
                        var cachedCar = new CachedCar() { Label = label };
                        _cachedCarsByCode.TryAdd(dbCode, cachedCar);
                        _cachedCarsByLabel.TryAdd(label, cachedCar);

                    }
                }

                res = ExecuteQuery($"SELECT RowId, Name FROM CAR_NAME_american");
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        int dbCode = res.GetInt32(0);
                        string carName = res.GetString(1);
                        _cachedCarsByCode[dbCode].Name = carName;
                    }
                }

                res = ExecuteQuery($"SELECT Label, VariationID FROM CAR_VARIATION_american");
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        string label = res.GetString(0);
                        string variationIdStr = res.GetString(1);
                        string[] spl = variationIdStr.Split(':');
                        if (spl.Length != 2 || !int.TryParse(spl[1], out int variationRowId))
                            continue;

                        _cachedCarsByLabel[label].VariationID = variationRowId;
                    }
                }
            }

            if (_cachedVariationColors.Count == 0)
            {
                var res = ExecuteQuery($"SELECT RowId, VarOrder, ColorChip0 FROM VARIATIONamerican");
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        int variationId = res.GetInt32(0);
                        int varOrder = res.GetInt32(1);
                        uint color = (uint)res.GetInt32(2);
                        _cachedVariationColors.Add((variationId, varOrder), color);
                    }
                }
            }

            if (_cachedRacesByLabel.Count == 0)
            {
                var res = ExecuteQuery($"SELECT Label, _rowid_, CourseID, RaceMode FROM RACE");
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        string label = res.GetString(0);
                        int rowId = res.GetInt32(1);
                        string courseIdStr = res.GetString(2);
                        int courseId = int.Parse(courseIdStr.Split(':')[1]);
                        string raceMode = res.GetString(3);

                        var cachedRace = new CachedRace()
                        {
                            RowId = rowId,
                            CourseId = courseId,
                            RaceMode = raceMode
                        };

                        _cachedRacesByCode.TryAdd(rowId, cachedRace);
                        _cachedRacesByLabel.Add(label, cachedRace);
                    }
                }
            }

            if (_cachedCourseLabels.Count == 0)
            {
                var res = ExecuteQuery($"SELECT RowId, Label FROM COURSE");
                if (res.HasRows)
                {
                    while (res.Read())
                    {
                        int rowId = res.GetInt32(0);
                        string label = res.GetString(1);
                        _cachedCourseLabels.Add(rowId, label);
                    }
                }
            }
        }

        public string GetCarNameByLabel(string label)
        {
            var res = ExecuteQuery($"SELECT Name FROM CAR_NAME_american WHERE Label = \"{label}\"");
            res.Read();

            return res.GetString(0);
        }

        public (int RowID, int CourseID, string RaceMode) GetRaceRowIndexByLabel(string label)
        {
            if (_cachedRacesByLabel.TryGetValue(label, out CachedRace cachedRace))
                return (cachedRace.RowId, cachedRace.CourseId, cachedRace.RaceMode);
            else
                return (-1, -1, "Unknown");
        }

        public string GetCourseLabelByID(int id)
        {
            if (_cachedCourseLabels.TryGetValue(id, out string label))
                return label;
            else
                return $"Unknown course {id}";
        }

        public uint? GetVariationRGBOfCarLabel(string label, int varOrder)
        {
            int? variationRowId = GetVariationIdFromCarLabel(label);
            if (variationRowId is null)
                return null;

            if (_cachedVariationColors.TryGetValue((variationRowId.Value, varOrder + 1), out uint color))
                return color;

            return null;
        }

        public int? GetVariationIdFromCarLabel(string label)
        {
            CacheDbCarInfo();

            if (_cachedCarsByLabel.TryGetValue(label, out CachedCar cachedCar))
                return cachedCar.VariationID;

            return null;
        }

        public List<string> GetAllRaceLabels()
        {
            var res = ExecuteQuery($"SELECT Label FROM RACE");

            List<string> strs = new List<string>();
            while (res.Read())
                strs.Add(res.GetString(0));
            return strs;
        }


        public string GetCarNameByCode(int code)
        {
            if (_cachedCarsByCode.TryGetValue(code, out CachedCar cachedCar))
                return cachedCar.Name;
            else
                return $"Unknown car {code}";
        }

        public string GetCarLabelByCode(int code)
        {
            if (_cachedCarsByCode.TryGetValue(code, out CachedCar cachedCar))
                return cachedCar.Label;
            else
                return $"Unknown car {code}";
        }

        public List<(int ID, string Label, string Name)> GetAllCarLabel_Code_Name()
        {
            var res = ExecuteQuery($"SELECT g.RowId, g.Label, cn.Name FROM GENERIC_CAR g JOIN CAR_NAME_american cn ON g.RowId = cn.RowId ORDER BY cn.Name");
            res.Read();

            List<(int ID, string Label, string Name)> strs = new();
            while (res.Read())
                strs.Add((res.GetInt32(0), res.GetString(1), res.GetString(2)));

            return strs;
        }

        public List<int> GetAllCarCodes()
        {
            var res = ExecuteQuery($"SELECT RowId FROM GENERIC_CAR");

            List<int> ids = new();
            while (res.Read())
                ids.Add(res.GetInt32(0));
            return ids;
        }

        public List<(string Name, int ID)> GetVariationNameAndRGBOfCar(string label)
        {
            List<(string Name, int ID)> strs = new();

            int? variationRowId = GetVariationIdFromCarLabel(label);
            if (variationRowId is null)
                return strs;

            var res = ExecuteQuery($"SELECT Name, ColorChip0 FROM VARIATIONamerican WHERE RowId = \"{variationRowId}\" ORDER BY VarOrder");

            while (res.Read())
                strs.Add((res.GetString(0), res.GetInt32(1)));
            return strs;
        }

        public SQLiteDataReader ExecuteQuery(string query)
        {
            SQLiteCommand cmd = _sConn.CreateCommand();
            cmd.CommandText = query;

            return cmd.ExecuteReader();
        }
    }
}
