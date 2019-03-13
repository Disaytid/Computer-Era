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
        public Collection<PlayerTariff> PlayerTariffs = new Collection<PlayerTariff>();
        public Services(SQLiteConnection connection, GameEnviromentMoney money)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM services;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string system_name = Convert.ToString(data_reader["system_name"]);
                    string name = Convert.ToString(data_reader["name"]);
                    TransactionType type = (TransactionType)Enum.Parse(typeof(TransactionType), Convert.ToString(data_reader["transaction_type"]));
                    double total_max_debt = Convert.ToDouble(data_reader["total_max_debt"]);
                    double total_max_contribution = Convert.ToDouble(data_reader["total_max_contribution"]);
                    bool is_system = Convert.ToBoolean(data_reader["is_system"]);
                    Collection<Tariff> tariffs = new Collection<Tariff>();

                    SQLiteCommand command2 = new SQLiteCommand(connection)
                    {
                        CommandText = @"SELECT * FROM v_services_tariffs WHERE id=" + id + ";",
                        CommandType = CommandType.Text
                    };
                    SQLiteDataReader data_reader2 = command2.ExecuteReader();

                    //Возможно стоит написать проверку от халтуры на Withdraw, сейчас ничто не мешает создать тариф где по кредиту сбор будет меньше выданной банком суммы
                    while (data_reader2.Read()) //написать проверку Periodicity меньше или равно TermUnit или условие на уровне БД
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

                    AllServices.Add(new Service(id, system_name, name, type, tariffs, is_system, total_max_debt, total_max_contribution));
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
        public double MinSum { get; set; }
        public double MaxSum { get; set; }
        public Periodicity Periodicity { get; set; }
        public int PeriodicityValue { get; set; }
        public Periodicity TermUnit { get; set; }
        public int MinTerm { get; set; }
        public int MaxTerm { get; set; }
        public bool SpecialOffer { get; set; }
        public object PropertyPledged { get; set; } //Имущество под залог

        public Tariff(int uid, string name, Currency currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, Periodicity term_unit, int min_term, int max_term, bool spec_offer = false)
        {
            UId = uid;
            Name = name;
            Currency = currency;
            Coefficient = coefficient;
            MinSum = min_sum;
            MaxSum = max_sum;
            Periodicity = periodicity;
            PeriodicityValue = periodicity_value;
            TermUnit = term_unit;
            MinTerm = min_term;
            MaxTerm = max_term;
            SpecialOffer = spec_offer;
        }
        public Tariff(int uid, string name, Currency currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, Periodicity term_unit, int min_term, int max_term, object property_pledged, bool spec_offer = false)
        {
            UId = uid;
            Name = name;
            Currency = currency;
            Coefficient = coefficient;
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

        public override string ToString()
        {
            string str = Name + Environment.NewLine +
            "Валюта: " + Currency.Name + Environment.NewLine +
            "Под: " + Coefficient + "%" + Environment.NewLine +
            "Сумма: ";
            if (MinSum == 0 & MaxSum == 0) { str += "не ограничена"; }
            else if (MinSum == 0 & MaxSum > 0) { str += "до " + MaxSum.ToString("N3") + " " + Currency.Abbreviation; }
            else if (MinSum > 0 & MaxSum == 0) { str += "от " + MinSum.ToString("N3") + " " + Currency.Abbreviation; }
            else { str += "от " + MinSum.ToString("N3") + " " + Currency.Abbreviation + " до " + MaxSum.ToString("N3") + " " + Currency.Abbreviation;  }
            return str;
        }
    }

    public class PlayerTariff : Tariff
    {
        public Service Service { get; set; }
        public double Amount { get; set; }
        public int Term { get; set; }
        public DateTime StartDateOfService { get; set; }
        public PlayerTariff(int uid, string name, Currency currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, Periodicity term_unit, int min_term, int max_term, Service service, double amount, int term, DateTime start_date, bool spec_offer = false) 
                     : base(uid, name, currency, coefficient, min_sum, max_sum, periodicity, periodicity_value, term_unit, min_term, max_term, spec_offer)
        {
            Service = service;
            Amount = amount;
            Term = term;
            StartDateOfService = start_date;
        }

        public PlayerTariff(int uid, string name, Currency currency, int coefficient, double min_sum, double max_sum, Periodicity periodicity, int periodicity_value, Periodicity term_unit, int min_term, int max_term, Service service, double amount, int term, DateTime start_date, object property_pledged, bool spec_offer = false)
                        : base (uid, name, currency, coefficient, min_sum, max_sum, periodicity, periodicity_value, term_unit, min_term, max_term, property_pledged, spec_offer)
        {
            Service = service;
            Amount = amount;
            Term = term;
            StartDateOfService = start_date;
        }

        public override string ToString()
        {
            string str = Name + Environment.NewLine +
            "Валюта: " + Currency.Name + Environment.NewLine +
            "Под: " + Coefficient + "%" + Environment.NewLine;
            if (Service.Type == TransactionType.TopUp)
            { str += "Вложенно: " + Amount.ToString("N3") + " " + Currency.Abbreviation;
            } else if (Service.Type == TransactionType.Withdraw) { str += "Получено: " + Amount.ToString("N3") + " " + Currency.Abbreviation; }
            str += Environment.NewLine +
            "Дата заключения услуги: " + StartDateOfService.ToString("dd.MM.yyyy HH:mm");
            return str;
        }
    }

    public class Service
    {
        public int UId { get; set; }
        public string SystemName { get; set; }
        public string Name { get; set; }
        public TransactionType Type { get; set; }
        public Collection<Tariff> Tariffs { get; set; } = new Collection<Tariff>();
        public double TotalMaxDebt { get; set; } //Указывается в UGC(Универсальной игровой валюте)
        public double TotalMaxContribution { get; set; } //Указывается в UGC(Универсальной игровой валюте)
        public bool IsSystem { get; set; }

        public Service(int uid, string system_name, string name, TransactionType type, Collection<Tariff> tariffs, bool isSystem , double tmd = 0, double tmc = 0)
        {
            UId = uid;
            SystemName = system_name;
            Name = name;
            Type = type;
            Tariffs = tariffs;
            TotalMaxDebt = tmd;
            TotalMaxContribution = tmc;
            IsSystem = isSystem;
        }
    }
}
