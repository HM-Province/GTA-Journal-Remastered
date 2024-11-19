using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Windows.Storage;
using System.IO;
using System.Diagnostics;
using Serilog;

namespace GTA_Journal.Database
{
    public static class DataAccess
    {
        private static string GetDbPath()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);

            Directory.CreateDirectory(Path.Join(path, "GTA Journal"));

            return Path.Join(path, "GTA Journal/data.db3");
        }

        public static void InitializeDatabase()
        {
            SQLitePCL.Batteries.Init();
            string dbPath = GetDbPath();

            try
            {
                using SqliteConnection db = new($"Data Source={dbPath}");
                db.Open();

                var tableCommand = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        user_id INTEGER PRIMARY KEY NOT NULL, 
                        username TEXT NOT NULL,
                        password TEXT NOT NULL,
                        usid TEXT NOT NULL,
                        expires TEXT NOT NULL
                )";

                new SqliteCommand(tableCommand, db).ExecuteReader();

                Log.Information("Database initialized");
            } catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize database");
            }
        }

        public static List<User> GetUsers()
        {
            var list = new List<User>();

            using (SqliteConnection db = new($"Data Source={GetDbPath()}"))
            {
                db.Open();

                var tableCommand = @"
                    SELECT user_id, username, password, usid, expires FROM Users
                ";

                using (var reader = new SqliteCommand(tableCommand, db).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userId = reader.GetInt32(0);
                        var username = reader.GetString(1);
                        var password = reader.GetString(2);
                        var usid = reader.GetString(3);
                        var expires = reader.GetString(4);

                        Debug.WriteLine($"User with ID: {userId} Username: {username} Password: {password} Usid: {usid} Expires: {expires}");
                    }
                }
            }

            return list;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UsId { get; set; }
        public string Expires { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
