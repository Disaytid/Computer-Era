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
    public class GameEnviromentMoney
    {
        public Collection<Currency> PlayerCurrency = new Collection<Currency>();

        public GameEnviromentMoney(SQLiteConnection connection, int save_id)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * from sv_currency WHERE save_id=" + save_id + ";";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string sys_name = Convert.ToString(data_reader["sys_name"]);
                    string name = Convert.ToString(data_reader["name"]);
                    string abbreviation = Convert.ToString(data_reader["abbreviation"]);
                    DateTime date_appearance = Convert.ToDateTime(data_reader["date_appearance"]);
                    double course = Convert.ToDouble(data_reader["course_to_ugc"]);
                    double count = Convert.ToDouble(data_reader["count"]);

                    PlayerCurrency.Add(new Currency(id, sys_name, name, abbreviation, date_appearance, course, count));
                }
            }
        }
    }

    public enum TransactionType
    {
        TopUp,
        Withdraw
    }
    public class Transaction
    {
        public string Name { get; set; }
        public string Initiator { get; set; }
        public DateTime DateTime { get; set; } //Дата и время проведения транзакции
        public double Sum { get; set; }
        public TransactionType Type { get; set; }
        public Transaction(string name, string initiator, DateTime dateTime, double sum, TransactionType type)
        {
            Name = name;
            Initiator = initiator;
            DateTime = dateTime;
            Sum = sum;
            Type = type;
        }
    }

    public class Currency
    {
        int Id { get; }
        public string SystemName { get;}
        public string Name { get; }
        public string Abbreviation { get;}
        public DateTime DateAppearance { get; }
        public double Course { get; } //Написать обработчик
        private double count = 0;
        public Collection<Transaction> TransactionHistory { get; } = new Collection<Transaction>();

        public double Count
        {
            get { return count;}
            private set { count = value; }
        }

        public bool Withdraw(string name, string initiator, DateTime dateTime, double amount)
        {
            if (amount <= count & amount > 0)
            {
                count -= amount;
                TransactionHistory.Add(new Transaction(name, initiator, dateTime, amount, TransactionType.Withdraw));
                return true;
            } else {
                return false;
            }
        }

        public bool TopUp(string name, string initiator, DateTime dateTime, double amount)
        {
            if (amount > 0)
            {
                count += amount;
                TransactionHistory.Add(new Transaction(name, initiator, dateTime, amount, TransactionType.TopUp));
                return true;
            } else {
                return false;
            }
        }

        public Currency(int id, string system_name, string name, string abbreviation, DateTime date_appearance, double course, double count)
        {
            Id = id;
            SystemName = system_name;
            Name = name;
            Abbreviation = abbreviation;
            DateAppearance = date_appearance;
            Course = course;
            Count = count;
        }
    }
}
