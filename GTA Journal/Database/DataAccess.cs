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
                    CREATE TABLE IF NOT EXISTS users (
                        id INTEGER PRIMARY KEY NOT NULL, 
                        usid TEXT NOT NULL,
                        username TEXT NOT NULL,
                        password TEXT NOT NULL,
                        avatar_url TEXT NOT NULL,
                        expires TEXT NOT NULL,
                        is_admin TINYINT NOT NULL
                )";

                new SqliteCommand(tableCommand, db).ExecuteReader();

                Log.Information("Database initialized");
            } catch (Exception ex)
            {
                Log.Fatal(ex, "Failed to initialize database");
                Environment.Exit(1);
            }
        }

        public static List<User> GetUsers()
        {
            var list = new List<User>();

            using (SqliteConnection db = new($"Data Source={GetDbPath()}"))
            {
                db.Open();

                var tableCommand = @"
                    SELECT id, usid, username, password, avatar_url, is_admin, expires FROM users
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

                        list.Add(new User() {
                            Id = reader.GetInt32(0),
                            UsId = reader.GetString(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3),
                            AvatarUrl = reader.GetString(4),
                            IsAdmin = reader.GetInt16(5) != 0,
                            Expires = DateTime.ParseExact(reader.GetString(6), "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            return list;
        }

        public static bool AddUser(int userId, string usId, string username, string password, string avatarUrl, bool isAdmin, DateTime expires)
        {
            try
            {
                using (SqliteConnection db = new($"Data Source={GetDbPath()}"))
                {
                    db.Open();

                    var tableCommand = @"
                        INSERT OR REPLACE INTO users (id, usid, username, password, avatar_url, is_admin, expires)
                        VALUES (@id, @usid, @username, @password, @avatarUrl, @isAdmin, @expires)
                    ";
                    using var command = new SqliteCommand(tableCommand, db);

                    command.Parameters.AddWithValue("@id", userId);
                    command.Parameters.AddWithValue("@usid", usId);
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@avatarUrl", avatarUrl);
                    command.Parameters.AddWithValue("@isAdmin", isAdmin ? 1 : 0);
                    command.Parameters.AddWithValue("@expires", expires.ToString("dd-MM-yyyy HH:mm:ss"));

                    command.ExecuteNonQuery();
                }

                return true;
            } 
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add user into database");
                return false;
            }
        }

        public static bool DeleteUser(int userId)
        {
            try
            {
                using (SqliteConnection db = new($"Data Source={GetDbPath()}"))
                {
                    db.Open();

                    var tableCommand = @"
                        DELETE FROM users WHERE id = @id
                    ";
                    using var command = new SqliteCommand(tableCommand, db);

                    command.Parameters.AddWithValue("@id", userId);

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete user from database");
                return false;
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UsId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Expires { get; set; }
    }
}
