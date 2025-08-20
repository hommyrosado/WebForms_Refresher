using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace WebformsDemonstration
{
    public static class SqlLite
    {
        private static string Conn => ConfigurationManager.ConnectionStrings["HotelDb"].ConnectionString;

        // Resolve |DataDirectory| -> physical path
        private static string GetPhysicalPath()
        {
            var csb = new SQLiteConnectionStringBuilder(Conn);
            var ds = csb.DataSource;

            if (ds.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
            {
                var baseDir = AppDomain.CurrentDomain.GetData("DataDirectory") as string
                              ?? AppDomain.CurrentDomain.BaseDirectory;
                ds = ds.Replace("|DataDirectory|\\", "").Replace("|DataDirectory|", "");
                return Path.Combine(baseDir, ds);
            }
            return ds;
        }

        // Quick validity check (SQLite header starts with "SQLite format 3\0")
        private static bool IsValidSqliteFile(string path)
        {
            if (!File.Exists(path)) return false;
            if (new FileInfo(path).Length < 100) return false;
            try
            {
                using (var fs = File.OpenRead(path))
                {
                    byte[] hdr = new byte[16];
                    int n = fs.Read(hdr, 0, hdr.Length);
                    var header = System.Text.Encoding.ASCII.GetString(hdr, 0, n);
                    return header.StartsWith("SQLite format 3");
                }
            }
            catch { return false; }
        }

        public static void EnsureCreated()
        {
            var physical = GetPhysicalPath();
            var dir = Path.GetDirectoryName(physical);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!IsValidSqliteFile(physical))
            {
                // (Re)create & seed the DB
                if (File.Exists(physical)) File.Delete(physical);
                SQLiteConnection.CreateFile(physical);

                using (var cn = new SQLiteConnection(Conn))
                using (var cmd = new SQLiteCommand(@"
PRAGMA journal_mode=WAL;

CREATE TABLE IF NOT EXISTS Hotels(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  Name  TEXT NOT NULL,
  City  TEXT NOT NULL,
  Rating INTEGER NOT NULL CHECK(Rating BETWEEN 1 AND 5)
);
CREATE TABLE IF NOT EXISTS Amenities(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  HotelId INTEGER NOT NULL,
  Name TEXT NOT NULL,
  FOREIGN KEY(HotelId) REFERENCES Hotels(Id)
);
CREATE TABLE IF NOT EXISTS Rooms(
  Id INTEGER PRIMARY KEY AUTOINCREMENT,
  HotelId INTEGER NOT NULL,
  Number TEXT NOT NULL,
  Type   TEXT NOT NULL,
  Price  REAL NOT NULL,
  FOREIGN KEY(HotelId) REFERENCES Hotels(Id)
);

INSERT INTO Hotels(Name,City,Rating) VALUES
('Riverview Inn','Richmond',4),
('Old Town Suites','Alexandria',5),
('Capital Stay','Arlington',3);

INSERT INTO Rooms (HotelId,Number,Type,Price) VALUES
(1,'101','Standard',119.00),(1,'201','Deluxe',159.00),
(2,'1A','Suite',249.00),(2,'2B','Standard',139.00),
(3,'10','Standard',99.00);

INSERT INTO Amenities(HotelId,Name) VALUES
(1,'Gym'),(1,'Laundry'),(2,'Spa'),(2,'Pool'),(3,'Shuttle');
", cn))
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable Query(string sql, params SQLiteParameter[] p)
        {
            using (var cn = new SQLiteConnection(Conn))
            using (var cmd = new SQLiteCommand(sql, cn))
            using (var da = new SQLiteDataAdapter(cmd))
            {
                if (p != null && p.Length > 0) cmd.Parameters.AddRange(p);
                var dt = new DataTable();
                cn.Open();
                da.Fill(dt); // this fails if the file isn't a real SQLite DB
                return dt;
            }
        }

        public static int Execute(string sql, params SQLiteParameter[] p)
        {
            using (var cn = new SQLiteConnection(Conn))
            using (var cmd = new SQLiteCommand(sql, cn))
            {
                if (p != null && p.Length > 0) cmd.Parameters.AddRange(p);
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
