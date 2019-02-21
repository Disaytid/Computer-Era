using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;

namespace Computer_Era.Game.Objects
{
    public class Services
    {
        public Collection<Service> AllServices = new Collection<Service>();
        public Collection<Service> PlayerServices = new Collection<Service>();
        public Services(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM services;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string name = Convert.ToString(data_reader["name"]);
                    TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), Convert.ToString(data_reader["transaction_type"]));
                    double total_max_debt = Convert.ToDouble(data_reader["total_max_debt"]);
                    double total_max_contribution = Convert.ToDouble(data_reader["total_max_contribution"]);
                    Collection<Tariff> tariffs = new Collection<Tariff>();

                    SQLiteCommand command2 = new SQLiteCommand(connection)
                    {
                        CommandText = @"SELECT * FROM v_services_tariffs WHERE id=" + id + ";",
                        CommandType = CommandType.Text
                    };
                    SQLiteDataReader data_reader2 = command2.ExecuteReader();

                    while (data_reader2.Read())
                    {
                        int tariff_id = Convert.ToInt32(data_reader2["tariff_id"]);
                        string tariff_name = Convert.ToString(data_reader2["name"]);
                        string tariff_currency = Convert.ToString(data_reader2["currency"]);
                        int tariff_coefficient = Convert.ToInt32(data_reader2["coefficient"]);
                        int tariff_min_sum = Convert.ToInt32(data_reader2["min_sum"]);
                        int tariff_max_sum = Convert.ToInt32(data_reader2["max_sum"]);
                        Periodicity tariff_periodicity = (Periodicity)Enum.Parse(typeof(Periodicity), Convert.ToString(data_reader2["periodicity"]));
                        int tariff_periodicity_value = Convert.ToInt32(data_reader2["periodicity_value"]);
                        bool tariff_special_offer = Convert.ToBoolean(data_reader2["special_offer"]);

                        Tariff tariff;
                        if (data_reader2["property_pledged"] != DBNull.Value)
                        {
                            int property_pledged = Convert.ToInt32(data_reader2["property_pledged"]);
                            tariff = new Tariff(tariff_id, tariff_name, tariff_currency, tariff_coefficient, tariff_min_sum, tariff_max_sum, tariff_periodicity, tariff_periodicity_value, property_pledged, tariff_special_offer);
                        } else {
                            tariff = new Tariff(tariff_id, tariff_name, tariff_currency, tariff_coefficient, tariff_min_sum, tariff_max_sum, tariff_periodicity, tariff_periodicity_value, tariff_special_offer);
                        }

                        tariffs.Add(tariff);
                    }

                    AllServices.Add(new Service(id, name, type, tariffs, total_max_debt, total_max_contribution));
                }
            }
        }
    }

    public class Tariff
    {
        public int UId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public int Coefficient { get; set; }
        public double Amount { get; set; }
        public double MinSum { get; set; }
        public double MaxSum { get; set; }
        public Periodicity Periodicity { get; set; }
        public int PeriodicityValue { get; set; }
        public bool SpecialOffer { get; set; }
        public int PropertyPledged { get; set; } //Имущество под залог

        public Tariff(int uid, string name, string currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, bool spec_offer = false, double amount = 0)
        {
            UId = uid;
            Name = name;
            Currency = currency;
            Coefficient = coefficient;
            Amount = amount;
            MinSum = min_sum;
            MaxSum = max_sum;
            Periodicity = periodicity;
            PeriodicityValue = periodicity_value;
            SpecialOffer = spec_offer;
        }
        public Tariff(int uid, string name, string currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, int property_pledged, bool spec_offer = false, double amount = 0)
        {
            UId = uid;
            Name = name;
            Currency = currency;
            Coefficient = coefficient;
            Amount = amount;
            MinSum = min_sum;
            MaxSum = max_sum;
            Periodicity = periodicity;
            PeriodicityValue = periodicity_value;
            PropertyPledged = property_pledged;
            SpecialOffer = spec_offer;
        }
    }

    public class Service
    {
        public int UId { get; set; }
        public string Name { get; set; }
        public TransactionType Type { get; set; }
        public Collection<Tariff> Tariffs { get; set; } = new Collection<Tariff>();
        public double TotalMaxDebt { get; set; } //Указывается в UGC(Универсальной игровой валюте)
        public double TotalMaxContribution { get; set; } //Указывается в UGC(Универсальной игровой валюте)

        public Service(int uid, string name, TransactionType type, Collection<Tariff> tariffs, double tmd = 0, double tmc = 0)
        {
            UId = uid;
            Name = name;
            Type = type;
            Tariffs = tariffs;
            TotalMaxDebt = tmd;
            TotalMaxContribution = tmc;
        }
    }
}
