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
    public class Professions
    {
        public Collection<Profession> PlayerProfessions = new Collection<Profession>();

        public Professions(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM professions;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string sys_name = Convert.ToString(data_reader["sys_name"]);
                    string name = Convert.ToString(data_reader["name"]);
                    double salary = Convert.ToDouble(data_reader["salary_in_ugc"]);
                    double complexity = Convert.ToDouble(data_reader["complexity"]);
                    int working_hours = Convert.ToInt32(data_reader["working_hours"]);
                    int day_period = Convert.ToInt32(data_reader["day_period"]);

                    PlayerProfessions.Add(new Profession(id, sys_name, name, salary, complexity, working_hours, day_period));
                }
            }
        }
    }

    public class Profession
    {
        public int Id { get; }
        public string SystemName { get; }
        public string Name { get; }
        public double Salary { get; }
        public double Complexity { get; } //Огрничить от 0 до 1
        public int WorkingHours { get; } //Написать обработку которая даст записать только занчения от 1 до 24
        public int DayPeriod { get; } //Написать обработку которая даст записать только занчения от 1 до 5 (5 случайный период дня)

        public Profession(int id, string sys_name, string name, double salary, double complexity, int workink_hours, int day_period)
        {
            Id = id;
            SystemName = sys_name;
            Name = name;
            Salary = salary;
            Complexity = complexity;
            WorkingHours = workink_hours;
            DayPeriod = day_period;
        }
    }
}
