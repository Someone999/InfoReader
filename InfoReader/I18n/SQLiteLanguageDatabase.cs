using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace InfoReaderPlugin.I18n
{
    public class DatabaseLastUpdateAttribute:System.Attribute
    {
        public string UpdateTime { get; }
        public DatabaseLastUpdateAttribute(string time)
        {
            UpdateTime = time;
        }
    }
    [DatabaseLastUpdate("2021/8/25T18:36:39+8")]
    public class SqliteLanguageDatabase:ILanguageFileReader,ILanguageFileUpdater
    {
        private readonly string _fileName;
        private bool _updated;

        public static DateTime? LastUpdateTime
        {
            get
            {
                Type t = typeof(SqliteLanguageDatabase);
                DatabaseLastUpdateAttribute attr = t.GetCustomAttribute(typeof(DatabaseLastUpdateAttribute)) as DatabaseLastUpdateAttribute;
                if (attr is null)
                    return null;
                if (DateTime.TryParse(attr.UpdateTime, out var retDateTime))
                    return retDateTime;
                return null;
            }
        }

        public static DateTime? LastUpdateTimeInDatabase
        {
            get
            {
                SQLiteConnection connection = new SQLiteConnection("DataSource=InfoReader.db");
                string query = "select UpdateTime from LanguageList where LanguageId = \"zh-cn\"";
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = query;
                var reader = cmd.ExecuteReader();
                
                if (reader.HasRows && reader.Read())
                {
                    string datetimeString = reader["UpdateTime"].ToString();
                    bool convertSuc = DateTime.TryParse(datetimeString, out var ret);
                    reader.Close();
                    connection.Close();
                    return convertSuc ? (DateTime?)ret : null;
                }
                reader.Close();
                connection.Close();
                return null;
            }
        }
        public SqliteLanguageDatabase(string databaseFile = "InfoReader.db")
        {
            _fileName = databaseFile;
            if(!File.Exists("InfoReader.db"))
                File.WriteAllBytes("InfoReader.db",Resource1.Database);
        }

        public void Write(string languageId, string key, string value)
        {
            var connection = CreateAndOpenConnection();
            var reader = ExecuteReader($"select exists (select * from \"{languageId}\" where \"Name\" = '{key}') == 1 as good", connection);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if(reader["good"].ToString() == "0")
                        ExecuteNonQuery($"insert into '{languageId}' values('{key}','{value}')", connection);
                }
            }
            
            CloseConnection(connection);
        }

        public void Append(string languageId, string key, string value) => Write(languageId, key, value);

        public void Delete(string languageId, string key, string value)
        {
            var connection = CreateAndOpenConnection();
            ExecuteNonQuery($"delete from \"{languageId}\" where Name = '{key}' and Content = '{value}'",
                connection);
            CloseConnection(connection);
        }

        public void Clear(string languageId)
        {
            var connection = CreateAndOpenConnection();
            ExecuteNonQuery($"delete from \"{languageId}\"", connection);
            CloseConnection(connection);
        }

        public string Read(string languageId, string key)
        {
            var connection = CreateAndOpenConnection();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = $"select Content from \"{languageId}\" where Name = '{key}'";
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
                return "null";
            while (reader.Read())
            {
                return reader[key].ToString();
            }
            CloseConnection(connection);
            return "null";
        }

        public bool NeedUpdate(string languageId)
        {
            if (LastUpdateTime.HasValue && LastUpdateTimeInDatabase.HasValue)
            {
                TimeSpan differ = LastUpdateTime.Value.ToUniversalTime() - LastUpdateTimeInDatabase.Value.ToUniversalTime();
                if (Math.Abs(differ.Days) > 1)
                    return true;
            }
            
            if (_updated)
                return false;
            if (!_updated)
                _updated = true;
            var connection = CreateAndOpenConnection();
            var reader = ExecuteReader($"select COUNT(Name) == (select COUNT(*) from LanguageKeys) as NeedUpdate from \"{languageId}\"",connection);
            if (reader is null)
                return false;
            while (reader.Read())
            {
                if (reader["NeedUpdate"].ToString() == "1")
                {
                    return false;
                }
            }
            CloseConnection(connection);
            return true;
        }
        public LanguageElement[] ReadAll(string languageId)
        {
            List<LanguageElement> elements = new List<LanguageElement>();
            var connection = CreateAndOpenConnection();
            var reader = ExecuteReader($"select * from \"{languageId}\"",connection);
            if (reader is null)
                return new LanguageElement[0];
            if (!reader.HasRows)
                return new LanguageElement[0];
            while (reader.Read())
               elements.Add(new LanguageElement(reader["Name"].ToString(),reader["Content"].ToString()));
            CloseConnection(connection);
            return elements.ToArray();
        }
        /// <summary>
        /// 创建并打开一个新连接
        /// </summary>
        protected virtual SQLiteConnection CreateAndOpenConnection()
        {
            var connection = new SQLiteConnection($"DataSource={_fileName}");
            connection.Open();
            return connection;
        }

        protected virtual void CloseConnection(SQLiteConnection connection) => connection.Close();
        
        static SQLiteConnection _fallbackConnection = new SQLiteConnection();
        public SQLiteDataReader ExecuteReader(string command,SQLiteConnection connection)
        {
            try
            {
                var sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = command;
                var rslt = sqlCommand.ExecuteReader();
                return rslt;
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public int ExecuteNonQuery(string command, SQLiteConnection connection)
        {
            try
            {
               
                var sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = command;
                var i = sqlCommand.ExecuteNonQuery();
                return i;
            }
            catch (Exception)
            {
                return -1;
                //Ignored
            }
            
        }

    }
}
