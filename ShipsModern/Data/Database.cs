using Npgsql;
using System;
using System.Configuration;


namespace ShipsForm.Data
{
    public class Database
    {
        private string s_connStr = string.Empty;
        private string? s_server = ConfigurationManager.AppSettings["Server"];
        private string? s_port = ConfigurationManager.AppSettings["Port"];
        private string? s_db_name = ConfigurationManager.AppSettings["Database"];
        private string? s_user = ConfigurationManager.AppSettings["User"];
        private string? s_password = ConfigurationManager.AppSettings["Password"];

        static public Database? Instance;

        private Database()
        {
            s_connStr = $"Server={s_server};Port={s_port};Database={s_db_name};User Id={s_user};Password={s_password}";
            NpgsqlConnection conn = new NpgsqlConnection(s_connStr);
            conn.Open();
            Console.WriteLine($"Database '{conn.Database}' successfully connected to project.");
            conn.Close();
        }

        static public void Init()
        {
            /*if (Instance != null)
                return;
            Instance = new Database();*/
        }
    }
}
