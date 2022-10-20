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
        private string _fileName;

        public GT4Database()
        {

        }

        public bool CreateConnection(string fileName)
        {
            if (fileName == _fileName)
                return true; // Already loaded

            _sConn = new SQLiteConnection($"Data Source={fileName};Version=3;New=False;Compress=True;");

            try
            {
                _sConn.Open();
            }
            catch (Exception e)
            {
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

            return res.GetString(0);
        }

        public uint GetVariationRGBOfCarLabel(string label, int varOrder)
        {
            var res = ExecuteQuery($"SELECT VariationID FROM CAR_VARIATION_american WHERE Label = \"{label}\"");
            res.Read();

            int variationRowId = res.GetInt32(0);

            res = ExecuteQuery($"SELECT RGB FROM VARIATIONamerican WHERE RowId = \"{variationRowId}\" AND VarOrder = \"{varOrder + 1}\"");
            res.Read();

            return (uint)res.GetInt32(0);
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

            return res.GetString(0);
        }

        public string GetCarLabelByCode(int code)
        {
            var res = ExecuteQuery($"SELECT Label FROM GENERIC_CAR WHERE RowId = \"{code}\"");
            res.Read();

            return res.GetString(0);
        }

        public SQLiteDataReader ExecuteQuery(string query)
        {
            SQLiteCommand cmd = _sConn.CreateCommand();
            cmd.CommandText = query;

            return cmd.ExecuteReader();
        }
    }
}
