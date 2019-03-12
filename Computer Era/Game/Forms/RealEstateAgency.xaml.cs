using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для Realty.xaml
    /// </summary>
    public partial class RealEstateAgency : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        public RealEstateAgency(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;
            string path = "Resources/currency/" + GameEnvironment.Money.PlayerCurrency[0].SystemName + ".png";
            Uri uri = new Uri("pack://application:,,,/" + path);

            if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
            {
                uri = new Uri("pack://application:,,,/Resources/currency/coin.png");
            }

            CoinIcon.Source = new BitmapImage(uri);
            CoinIcon.Width = 32;
            CoinIcon.Height = 32;
            CoinCount.Content = GameEnvironment.Money.PlayerCurrency[0].Count.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;

            AddItem(GameEnvironment.Realty.Houses);
        }

        private void AddItemsToItemsSource(Collection<House> items, List<RealtyObject> items_source) //Добавление предметов в ItemsSource
        {
            for (int i = 0; i <= items.Count - 1; i++)
            {
                string path = "Resources/realty/" + items[i].Image + ".png";
                Uri uri = new Uri("pack://application:,,,/" + path);

                if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
                {
                    uri = new Uri("pack://application:,,,/Resources/realty/house.png");
                }
                BitmapImage image = new BitmapImage(uri);

                items_source.Add(new RealtyObject(items[i], image));
            }
        }

        public void AddItem(Collection<House> collection) //Загрузка предметов в ListBox
        {
            List<RealtyObject> items_source = new List<RealtyObject>();
            AddItemsToItemsSource(collection, items_source);

            HousesList.ItemsSource = items_source;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            //TextBlock textBlock = sender as TextBlock;
            // double price = Convert.ToInt32(textBlock.Text) * GameEnvironment.Money.PlayerCurrency[0].Course;
            //textBlock.Text = price.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (!(button.Tag is House)) { return; }
            House house = button.Tag as House;

            if (GameEnvironment.Player.House != null)
            {
                if (GameEnvironment.Player.House.IsRent == true & GameEnvironment.Player.House.IsRentedOut & house.UId == GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Аренда", "Вы уже арендовали данное жилье", GameMessageBox.MessageBoxType.Warning); return; }
                if (GameEnvironment.Player.House.IsPurchase) { if (GameMessageBox.Show("Аренда", "У вас куплено жилье, вы действительно хотите его продать?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
                if (GameEnvironment.Player.House.IsPurchasedOnCredit) { _ = GameMessageBox.Show("Аренда", "Нельзя арендовать купленное в кредит жилье!", GameMessageBox.MessageBoxType.Information); return; }
            }

            Service service = null;
            for (int i = 0; i <= GameEnvironment.Services.AllServices.Count - 1; i++)
            {
                if (GameEnvironment.Services.AllServices[i].SystemName == "rent") { service = GameEnvironment.Services.AllServices[i]; break; }
            }
            if (service == null) { GameMessageBox.Show("Аренда", "Не найдена услуга аренды, убедитесь в целостности базы данных!", GameMessageBox.MessageBoxType.Error); return; }
            PropertyForSale();

            int uid = GameEnvironment.Services.AllServices.Count + 1; //Возможно прийдеться поменять
            PlayerTariff playerTariff = new PlayerTariff(uid, house.Name, GameEnvironment.Money.PlayerCurrency[0], 0, house.Rent, house.Rent, Periodicity.Month, 1, Periodicity.Month, 1, 1, service, house.Rent, 1, GameEnvironment.GameEvents.GameTimer.DateAndTime);
            GameEnvironment.Services.PlayerTariffs.Add(playerTariff);
            GameEnvironment.Player.House = new PlayerHouse(house.UId, house.Name, house.Area, house.StorageSize, house.Rent, house.Price, house.CommunalPayments, house.Location, house.Distance, house.IsPurchase, house.IsRent, house.IsCreditPurchase, house.Image, true, false, false, playerTariff);

            GameEnvironment.GameEvents.Events.Add(new GameEvent(service.UId + ":" + playerTariff.UId + ":" + house.Rent,
                                GameEnvironment.GameEvents.GetDateTimeFromPeriodicity(GameEnvironment.GameEvents.GameTimer.DateAndTime, playerTariff.Periodicity, playerTariff.PeriodicityValue),
                                playerTariff.Periodicity, playerTariff.PeriodicityValue, RentalPayment, true));
            GameEnvironment.Messages.NewMessage("Агенство недвижимости", "Вы арендовали: " + house.Name + ". Поздравляем Вас!", GameMessages.Icon.Info);
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (!(button.Tag is House)) { return; }
            House house = button.Tag as House;

            if (GameEnvironment.Player.House != null)
            {
                if (GameEnvironment.Player.House.IsRent == true & GameEnvironment.Player.House.IsRentedOut & house.UId == GameEnvironment.Player.House.UId) { if (GameMessageBox.Show("Покупка", "Данное жилье арендовано вами, вы действительно хотите расторгнуть аренду?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
                if ((GameEnvironment.Player.House.IsPurchase || GameEnvironment.Player.House.IsCreditPurchase) && house.UId == GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Покупка", "У вас уже куплено данное жилье!", GameMessageBox.MessageBoxType.Information); return; }
                if (GameEnvironment.Player.House.IsCreditPurchase && house.UId != GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Покупка", "Нельзя купить новое жилье пока не погасите кредит за текущее!", GameMessageBox.MessageBoxType.Information); return; }
                if (GameEnvironment.Player.House.IsPurchase && house.UId != GameEnvironment.Player.House.UId) { if (GameMessageBox.Show("Покупка", "У вас есть другое купленно жилье, вы действительно хотите его продать?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
            }
            PropertyForSale();

            double price = house.Price * GameEnvironment.Money.PlayerCurrency[0].Course;
            if (GameEnvironment.Money.PlayerCurrency[0].Withdraw("Покупка " + house.Name, "Агенство недвижимости \"Крыша над головой\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, price))
            {
                CoinCount.Content = GameEnvironment.Money.PlayerCurrency[0].Count.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
                GameEnvironment.Player.House = new PlayerHouse(house.UId, house.Name, house.Area, house.StorageSize, house.Rent, house.Price, house.CommunalPayments, house.Location, house.Distance, house.IsPurchase, house.IsRent, house.IsCreditPurchase, house.Image, false, true, false);
                SellerText.Text = "Благодарим за покупку, с Вами приятно иметь дело!";
                GameEnvironment.Messages.NewMessage("Агенство недвижимости", "Вы купили: " + house.Name + ". Поздравляем Вас с покупкой!", GameMessages.Icon.Info);
            } else {
                SellerText.Text = "К сожалению на вашем счету недостаточно средств!";
            }
        }
        private void PropertyForSale()
        {
            if (GameEnvironment.Player.House != null && GameEnvironment.Player.House.IsRentedOut)
            {
                GameEvent @event;
                for (int i = 0; i >= GameEnvironment.GameEvents.Events.Count - 1; i++)
                {
                    if (GameEnvironment.GameEvents.Events[i].Name == (GameEnvironment.Player.House.PlayerRent.Service.UId + ":" + GameEnvironment.Player.House.PlayerRent.UId + ":" + GameEnvironment.Player.House.Rent))
                    {
                        @event = GameEnvironment.GameEvents.Events[i];
                        double sum = (@event.ResponseTime - GameEnvironment.GameEvents.GameTimer.DateAndTime).TotalDays * (GameEnvironment.Player.House.PlayerRent.Amount * GameEnvironment.Player.House.PlayerRent.Currency.Course);
                        if (GameEnvironment.Player.House.PlayerRent.Currency.Withdraw("Выплата аренды", "Агенство недвижимости \"Крыша над головой\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, sum)) { GameMessageBox.Show("Аренда", "Вам не хватает средств на выплату задолженности по текущей аренде!", GameMessageBox.MessageBoxType.Information); return; }
                        GameEnvironment.GameEvents.Events.Remove(@event);
                        break;
                    }
                }
            }
            else if (GameEnvironment.Player.House != null && GameEnvironment.Player.House.IsPurchased)
            {
                GameEnvironment.Money.PlayerCurrency[0].TopUp("Продажа недвижимости", GameEnvironment.Player.Name, GameEnvironment.GameEvents.GameTimer.DateAndTime, (GameEnvironment.Player.House.Price * 90 / 100) * GameEnvironment.Money.PlayerCurrency[0].Course);
            }
        }

        private void RentalPayment(GameEvent @event)
        {
            string[] keys = @event.Name.Split(new char[] { ':' });
            List<PlayerTariff> tariffs = GameEnvironment.Services.PlayerTariffs.Where(t => t.Service.UId.ToString() == keys[0] & t.UId.ToString() == keys[1] & t.Amount.ToString() == keys[2]).ToList();
            if (tariffs.Count == 1)
            {
                if (tariffs[0].Currency.Withdraw("Выплата аренды", "Агенство недвижимости \"Крыша над головой\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, tariffs[0].Amount * tariffs[0].Currency.Course) == false)
                {
                    GameEnvironment.Player.House = null;
                    GameEnvironment.GameEvents.Events.Remove(@event);
                    GameEnvironment.Messages.NewMessage("Агенство недвижимости \"Крыша над головой\"", "Вы были выселены за неуплату аренды!", GameMessages.Icon.Info);
                }
            } else { GameMessageBox.Show("Обработка выплат и взысканий", "Что-то пошло не так, тариф не найден!", GameMessageBox.MessageBoxType.Error); }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }

    class RealtyObject
    {
        public House House { get; set; }
        public ImageSource Image { get; set; }
        public string Tag { get; set; }

        public RealtyObject(House house, ImageSource imageSource)
        {
            House = house;
            Image = imageSource;
            Tag = house.ToString();
        }
    }
}
