using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GT4SaveEditor.Database
{
    public class GT4Database
    {
        private SQLiteConnection _sConn;
        public string FileName { get; set; }

        public bool Connected { get; set; }

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
                Connected = true;
            }
            catch (Exception e)
            {
                Connected = false;
                return false;
            }

            return true;
        }

        public string GetCarNameByLabel(string label)
        {
            var res = ExecuteQuery($"SELECT Name FROM CAR_NAME_american WHERE Label = \"{label}\"");
            res.Read();

            return res.GetString(0);
        }

        public (int RowID, int CourseID, string RaceMode) GetRaceRowIndexByLabel(string label)
        {
            var res = ExecuteQuery($"SELECT _rowid_, CourseID, RaceMode FROM RACE WHERE Label = \"{label}\"");
            res.Read();

            return (res.GetInt32(0) - 1, res.GetInt32(1), res.GetString(2));
        }

        public string GetCourseLabelByID(int id)
        {
            var res = ExecuteQuery($"SELECT Label FROM COURSE WHERE RowId = \"{id}\"");
            res.Read();

            if (res.HasRows)
                return res.GetString(0);
            else
                return $"Unknown course {id}";
        }

        public uint? GetVariationRGBOfCarLabel(string label, int varOrder)
        {
            int? variationRowId = GetVariationIdFromCarLabel(label);
            if (variationRowId is null)
                return null;

            var res = ExecuteQuery($"SELECT RGB FROM VARIATIONamerican WHERE RowId = \"{variationRowId}\" AND VarOrder = \"{varOrder + 1}\"");
            res.Read();

            if (res.HasRows)
                return (uint)res.GetInt32(0);
            else
                return null;
        }

        public int? GetVariationIdFromCarLabel(string label)
        {
            var res = ExecuteQuery($"SELECT VariationID FROM CAR_VARIATION_american WHERE Label = \"{label}\"");
            res.Read();

            if (res.HasRows)
            {
                int variationRowId = res.GetInt32(0);
                return variationRowId;
            }
            else
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
            var res = ExecuteQuery($"SELECT Name FROM CAR_NAME_american WHERE RowId = \"{code}\"");
            res.Read();

            if (res.HasRows)
                return res.GetString(0);
            else
                return $"Unknown car {code}";
        }

        public string GetCarLabelByCode(int code)
        {
            var res = ExecuteQuery($"SELECT Label FROM GENERIC_CAR WHERE RowId = \"{code}\"");
            res.Read();

            if (res.HasRows)
                return res.GetString(0);
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

            var res = ExecuteQuery($"SELECT Name, RGB FROM VARIATIONamerican WHERE RowId = \"{variationRowId}\" ORDER BY VarOrder");

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
