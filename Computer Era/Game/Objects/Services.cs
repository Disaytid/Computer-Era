using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace Computer_Era.Game.Objects
{
    public class Services
    {
        public Collection<Service> AllServices = new Collection<Service>();
        public Collection<Service> PlayerServices = new Collection<Service>();
        public Services(SQLiteConnection connection, Money money)
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
                        double tariff_min_sum = Convert.ToDouble(data_reader2["min_sum"]);
                        double tariff_max_sum = Convert.ToDouble(data_reader2["max_sum"]);
                        Periodicity tariff_periodicity = (Periodicity)Enum.Parse(typeof(Periodicity), Convert.ToString(data_reader2["periodicity"]));
                        int tariff_periodicity_value = Convert.ToInt32(data_reader2["periodicity_value"]);
                        Periodicity tariff_term_unit = (Periodicity)Enum.Parse(typeof(Periodicity), Convert.ToString(data_reader2["term_unit"]));
                        int tariff_min_term = Convert.ToInt32(data_reader2["min_term"]);
                        int tariff_max_term = Convert.ToInt32(data_reader2["max_term"]);
                        bool tariff_special_offer = Convert.ToBoolean(data_reader2["special_offer"]);

                        Currency currency = null;
                        foreach (Currency l_currency in money.PlayerCurrency) { if (l_currency.SystemName == tariff_currency) { currency = l_currency; break; } }
                        if (currency == null) { MessageBox.Show("Валюта не найдена, запись не будет загружена!"); continue; } //Не добавлять в списки записи для которых не найдена валюта, выводить в логи.

                        if (type == TransactionType.TopUp)
                        {
                            if ((total_max_contribution != 0 & (tariff_max_sum / currency.Course) > total_max_contribution) || tariff_max_sum == 0) { tariff_max_sum = total_max_contribution * currency.Course; }
                            if (total_max_contribution != 0 & (tariff_min_sum / currency.Course) > total_max_contribution) { tariff_min_sum = total_max_contribution * currency.Course; }
                        } else if (type == TransactionType.Withdraw) {
                            if ((total_max_debt != 0 & (tariff_max_sum / currency.Course) > total_max_debt) || tariff_max_sum == 0) { tariff_max_sum = total_max_debt * currency.Course; }
                            if (total_max_debt != 0 & (tariff_min_sum / currency.Course) > total_max_debt) { tariff_min_sum = total_max_debt * currency.Course; }
                        }
                        if (tariff_min_term > tariff_max_term) { tariff_min_term = tariff_max_term; }

                        Tariff tariff;
                        if (data_reader2["property_pledged"] != DBNull.Value)
                        {
                            int property_pledged = Convert.ToInt32(data_reader2["property_pledged"]);
                            tariff = new Tariff(tariff_id, tariff_name, currency, tariff_coefficient, tariff_min_sum, tariff_max_sum, tariff_periodicity, tariff_periodicity_value, tariff_term_unit, tariff_min_term, tariff_max_term, property_pledged, tariff_special_offer);
                        } else {
                            tariff = new Tariff(tariff_id, tariff_name, currency, tariff_coefficient, tariff_min_sum, tariff_max_sum, tariff_periodicity, tariff_periodicity_value, tariff_term_unit, tariff_min_term, tariff_max_term, tariff_special_offer);
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
        public Currency Currency { get; set; }
        public int Coefficient { get; set; }
        public double Amount { get; set; }
        public double MinSum { get; set; }
        public double MaxSum { get; set; }
        public Periodicity Periodicity { get; set; }
        public int PeriodicityValue { get; set; }
        public Periodicity TermUnit { get; set; }
        public int MinTerm { get; set; }
        public int MaxTerm { get; set; }
        public bool SpecialOffer { get; set; }
        public int PropertyPledged { get; set; } //Имущество под залог

        public Tariff(int uid, string name, Currency currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, Periodicity term_unit, int min_term, int max_term, bool spec_offer = false, double amount = 0)
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
            TermUnit = term_unit;
            MinTerm = min_term;
            MaxTerm = max_term;
            SpecialOffer = spec_offer;
        }
        public Tariff(int uid, string name, Currency currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, Periodicity term_unit, int min_term, int max_term, int property_pledged, bool spec_offer = false, double amount = 0)
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
            TermUnit = term_unit;
            MinTerm = min_term;
            MaxTerm = max_term;
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
