using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;

namespace Computer_Era.Game.Objects
{
    public class Realty
    {
        public Collection<House> Houses = new Collection<House>();
        public Realty(SQLiteConnection connection)
        {
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM realty;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string name = Convert.ToString(data_reader["name"]);
                    int area = Convert.ToInt32(data_reader["area"]);
                    int storage_size = Convert.ToInt32(data_reader["storage_size"]);
                    double rent = Convert.ToDouble(data_reader["rent"]);
                    double price = Convert.ToDouble(data_reader["price"]);
                    double communal_payments = Convert.ToDouble(data_reader["communal_payments"]);
                    string location = Convert.ToString(data_reader["location"]);
                    int distance = Convert.ToInt32(data_reader["distance"]);
                    bool is_purchase = Convert.ToBoolean(data_reader["is_purchase"]);
                    bool is_rent = Convert.ToBoolean(data_reader["is_rent"]);
                    bool is_credit_purchase = Convert.ToBoolean(data_reader["is_credit_purchase"]);
                    string image_name = Convert.ToString(data_reader["image_name"]);

                    Houses.Add(new House(id, name, area, storage_size,  rent, price, communal_payments, location, distance, is_purchase, is_rent, is_credit_purchase, image_name));
                }
            }
        }
    }

    public class House
    {
        public int UId { get; set; }
        public string Name { get; set; }
        public int Area { get; set; }
        public int StorageSize { get; set; }
        public double Rent { get; set; }
        public double Price { get; set; }
        public double CommunalPayments { get; set; }
        public string Location { get; set; }
        public int Distance { get; set; } //В метрах
        public bool IsPurchase { get; set; }
        public bool IsRent { get; set; }
        public  bool IsCreditPurchase { get; set; }
        public string Image { get; set; }

        public House(int id, string name, int area, int storage_size, double rent, double price, double communal_payments, string location, int distance, bool is_purchase, bool is_rent, bool is_credit_purchase, string image_name)
        {
            UId = id;
            Name = name;
            Area = area;
            StorageSize = storage_size;
            Rent = rent;
            Price = price;
            CommunalPayments = communal_payments;
            Location = location;
            Distance = distance;
            IsPurchase = is_purchase;
            IsRent = is_rent;
            IsCreditPurchase = is_credit_purchase;
            Image = image_name;
        }

        public override string ToString()
        {
            string str = "Площадь " + Area + " м²" + Environment.NewLine +
                          "Размер кладовки: " + StorageSize + " ячеек" + Environment.NewLine;
            if (IsRent) { str += "Стоимость аренды: " + Rent + Environment.NewLine; }
            if (IsPurchase) { str += "Стоимость покупки: " + Price ; }
            return str;
        }
    }

    public class PlayerHouse : House
    {
        public bool IsRentedOut { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsPurchasedOnCredit { get; set; }
        public PlayerTariff PlayerRent { get; set; }
        public PlayerTariff PlayerCredit { get; set; }
        public PlayerHouse(int id, string name, int area, int storage_size, double rent, double price, double communal_payments, string location, int distance, bool is_purchase, bool is_rent, bool is_credit_purchase, string image_name, bool isRentedOut = false, bool isPurchased = false, bool isPurchasedOnCredit = false, PlayerTariff tariff = null)
                          : base(id, name, area, storage_size, rent, price, communal_payments, location, distance, is_purchase, is_rent, is_credit_purchase, image_name)
        {
            IsRentedOut = isRentedOut;
            IsPurchase = isPurchased;
            IsPurchasedOnCredit = isPurchasedOnCredit;
            if (IsRentedOut) { PlayerRent = tariff; }
            else if (IsCreditPurchase) { PlayerCredit = tariff; }
        }
    }
}
