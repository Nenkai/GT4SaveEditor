using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GT4SaveEditor
{
    public class GT4Database
    {
        private SQLiteConnection _sConn;

        private string _conString;
        public GT4Database(string fileName)
        {
            _conString = $"Data Source={fileName};Version=3;New=False;Compress=True;";
        }

        public bool CreateConnection()
        {
            _sConn = new SQLiteConnection(_conString);

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

        public SQLiteDataReader ExecuteQuery(string query)
        {
            SQLiteCommand cmd = _sConn.CreateCommand();
            cmd.CommandText = query;

            return cmd.ExecuteReader();
        }
    }
}
