using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Computer_Era.Game.Objects
{
    public class Companies
    {
        public Collection<Company> GameCompany = new Collection<Company>();

        public Companies(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM game_values WHERE id=1;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);

                    string json;
                    json = Convert.ToString(data_reader["values"]);
                    GameCompany = JsonConvert.DeserializeObject<Collection<Company>>(json);
                }
            }
        }
    }

    public class Company
    {
        public String Name { get; }
        public DateTime OpeningYear { get; }

        public Company(string name, DateTime opening_year) {
            Name = name;
            OpeningYear = opening_year;
        }
    }
}
