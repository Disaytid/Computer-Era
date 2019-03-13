using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (!(button.Tag is House)) { return; }
            House house = button.Tag as House;

            if (GameEnvironment.Player.House != null)
            {
                if (GameEnvironment.Player.House.IsRent == true & GameEnvironment.Player.House.IsRentedOut & house.UId == GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Аренда", "Вы уже арендовали данное жилье", GameMessageBox.MessageBoxType.Warning); return; }
                if (GameEnvironment.Player.House.IsPurchased) { if (GameMessageBox.Show("Аренда", "У вас куплено жилье, вы действительно хотите его продать?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
                if (GameEnvironment.Player.House.IsPurchasedOnCredit) { _ = GameMessageBox.Show("Аренда", "Нельзя арендовать если есть купленное в кредит жилье!", GameMessageBox.MessageBoxType.Information); return; }
            }

            Service service = null;
            Service communal_service = null;
            for (int i = 0; i <= GameEnvironment.Services.AllServices.Count - 1; i++)
            {
                if (GameEnvironment.Services.AllServices[i].SystemName == "rent") { service = GameEnvironment.Services.AllServices[i];  }
                else if (GameEnvironment.Services.AllServices[i].SystemName == "communal_payments") { communal_service = GameEnvironment.Services.AllServices[i]; }
                if (service != null && communal_service != null) { break; }
            }
            if (service == null) { GameMessageBox.Show("Аренда", "Не найдена услуга аренды, убедитесь в целостности базы данных!", GameMessageBox.MessageBoxType.Error); return; }
            if (communal_service == null) { GameMessageBox.Show("Аренда", "Не найдена услуга коммунальных платежей, убедитесь в целостности базы данных!", GameMessageBox.MessageBoxType.Error); return; }
            PropertyForSale();

            int uid = GameEnvironment.Services.AllServices.Count + 1; //Возможно прийдеться поменять
            PlayerTariff playerTariff = new PlayerTariff(uid, house.Name, GameEnvironment.Money.PlayerCurrency[0], 0, house.Rent, house.Rent, Periodicity.Month, 1, Periodicity.Month, 1, 1, service, house.Rent * GameEnvironment.Money.PlayerCurrency[0].Course, 1, GameEnvironment.GameEvents.GameTimer.DateAndTime);
            PlayerTariff playerCommunalTariff = new PlayerTariff(uid + 1, house.Name, GameEnvironment.Money.PlayerCurrency[0], 0, house.Rent, house.Rent, Periodicity.Month, 1, Periodicity.Month, 1, 1, communal_service, house.CommunalPayments * GameEnvironment.Money.PlayerCurrency[0].Course, 1, GameEnvironment.GameEvents.GameTimer.DateAndTime);
            GameEnvironment.Services.PlayerTariffs.Add(playerTariff);
            GameEnvironment.Services.PlayerTariffs.Add(playerCommunalTariff);
            GameEnvironment.Player.House = new PlayerHouse(house.UId, house.Name, house.Area, house.StorageSize, house.Rent, house.Price, house.CommunalPayments, house.Location, house.Distance, house.IsPurchase, house.IsRent, house.IsCreditPurchase, house.Image, playerCommunalTariff, true, false, false, playerTariff);

            GameEnvironment.GameEvents.Events.Add(new GameEvent(service.UId + ":" + playerTariff.UId + ":" + (house.Rent * playerTariff.Currency.Course),
                                GameEnvironment.GameEvents.GetDateTimeFromPeriodicity(GameEnvironment.GameEvents.GameTimer.DateAndTime, playerTariff.Periodicity, playerTariff.PeriodicityValue),
                                playerTariff.Periodicity, playerTariff.PeriodicityValue, RentalPayment, true));
            DateTime date = GameEnvironment.GameEvents.GameTimer.DateAndTime; date = new DateTime(date.Year, date.Month, 15, date.Hour, date.Minute, date.Second);
            GameEnvironment.GameEvents.Events.Add(new GameEvent(communal_service.UId + ":" + playerCommunalTariff.UId + ":" + (house.CommunalPayments * playerCommunalTariff.Currency.Course),
                    GameEnvironment.GameEvents.GetDateTimeFromPeriodicity(date, playerCommunalTariff.Periodicity, playerCommunalTariff.PeriodicityValue),
                    playerCommunalTariff.Periodicity, playerCommunalTariff.PeriodicityValue, CommunalPayment, true));
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
                if ((GameEnvironment.Player.House.IsPurchased || GameEnvironment.Player.House.IsPurchasedOnCredit) && house.UId == GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Покупка", "У вас уже куплено данное жилье!", GameMessageBox.MessageBoxType.Information); return; }
                if (GameEnvironment.Player.House.IsPurchasedOnCredit && house.UId != GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Покупка", "Нельзя купить новое жилье пока не погасите кредит за текущее!", GameMessageBox.MessageBoxType.Information); return; }
                if (GameEnvironment.Player.House.IsPurchased && house.UId != GameEnvironment.Player.House.UId) { if (GameMessageBox.Show("Покупка", "У вас есть другое купленно жилье, вы действительно хотите его продать?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
            }

            Service communal_service = null;
            for (int i = 0; i <= GameEnvironment.Services.AllServices.Count - 1; i++)
            {
                if (GameEnvironment.Services.AllServices[i].SystemName == "communal_payments") { communal_service = GameEnvironment.Services.AllServices[i]; break;  }
            }
            if (communal_service == null) { GameMessageBox.Show("Покупка", "Не найдена услуга коммунальных платежей, убедитесь в целостности базы данных!", GameMessageBox.MessageBoxType.Error); return; }
            PropertyForSale();

            double price = house.Price * GameEnvironment.Money.PlayerCurrency[0].Course;
            if (GameEnvironment.Money.PlayerCurrency[0].Withdraw("Покупка " + house.Name, "Агенство недвижимости \"Крыша над головой\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, price))
            {
                CoinCount.Content = GameEnvironment.Money.PlayerCurrency[0].Count.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
                int uid = GameEnvironment.Services.AllServices.Count + 1; //Возможно прийдеться поменять
                PlayerTariff playerCommunalTariff = new PlayerTariff(uid + 1, house.Name, GameEnvironment.Money.PlayerCurrency[0], 0, house.Rent, house.Rent, Periodicity.Month, 1, Periodicity.Month, 1, 1, communal_service, house.CommunalPayments * GameEnvironment.Money.PlayerCurrency[0].Course, 1, GameEnvironment.GameEvents.GameTimer.DateAndTime);
                GameEnvironment.Services.PlayerTariffs.Add(playerCommunalTariff);

                DateTime date = GameEnvironment.GameEvents.GameTimer.DateAndTime; date = new DateTime(date.Year, date.Month, 15, date.Hour, date.Minute, date.Second);
                GameEnvironment.GameEvents.Events.Add(new GameEvent(communal_service.UId + ":" + playerCommunalTariff.UId + ":" + (house.CommunalPayments * playerCommunalTariff.Currency.Course),
                                      GameEnvironment.GameEvents.GetDateTimeFromPeriodicity(date, playerCommunalTariff.Periodicity, playerCommunalTariff.PeriodicityValue),
                                      playerCommunalTariff.Periodicity, playerCommunalTariff.PeriodicityValue, CommunalPayment, true));
                GameEnvironment.Player.House = new PlayerHouse(house.UId, house.Name, house.Area, house.StorageSize, house.Rent, house.Price, house.CommunalPayments, house.Location, house.Distance, house.IsPurchase, house.IsRent, house.IsCreditPurchase, house.Image, playerCommunalTariff, false, true, false);
                SellerText.Text = "Благодарим за покупку, с Вами приятно иметь дело!";
                GameEnvironment.Messages.NewMessage("Агенство недвижимости", "Вы купили: " + house.Name + ". Поздравляем Вас с покупкой!", GameMessages.Icon.Info);
            } else {
                SellerText.Text = "К сожалению на вашем счету недостаточно средств!";
            }
        }

        House house;
        Bank bank;
        Service communal_service = null;
        private void BuyCreditButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (!(button.Tag is House)) { return; }
            house = button.Tag as House;

            if (GameEnvironment.Player.House != null)
            {
                if (GameEnvironment.Player.House.IsRent == true & GameEnvironment.Player.House.IsRentedOut & house.UId == GameEnvironment.Player.House.UId) { if (GameMessageBox.Show("Покупка в кредит", "Данное жилье арендовано вами, вы действительно хотите расторгнуть аренду?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
                if ((GameEnvironment.Player.House.IsPurchased || GameEnvironment.Player.House.IsPurchasedOnCredit) && house.UId == GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Покупка в кредит", "У вас уже куплено данное жилье!", GameMessageBox.MessageBoxType.Information); return; }
                if (GameEnvironment.Player.House.IsPurchasedOnCredit && house.UId != GameEnvironment.Player.House.UId) { _ = GameMessageBox.Show("Покупка в кредит", "Нельзя купить новое жилье пока не погасите кредит за текущее!", GameMessageBox.MessageBoxType.Information); return; }
                if (GameEnvironment.Player.House.IsPurchased && house.UId != GameEnvironment.Player.House.UId) { if (GameMessageBox.Show("Покупка в кредит", "У вас есть другое купленно жилье, вы действительно хотите его продать?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.No) { return; } }
            }

            Service service = null;
            for (int i = 0; i <= GameEnvironment.Services.AllServices.Count - 1; i++)
            {
                if (GameEnvironment.Services.AllServices[i].SystemName == "rent") { service = GameEnvironment.Services.AllServices[i]; }
                else if (GameEnvironment.Services.AllServices[i].SystemName == "communal_payments") { communal_service = GameEnvironment.Services.AllServices[i]; }
                if (service != null && communal_service != null) { break; }
            }
            if (service == null) { GameMessageBox.Show("Покупка в кредит", "Не найдена услуга аренды, убедитесь в целостности базы данных!", GameMessageBox.MessageBoxType.Error); return; }
            if (communal_service == null) { GameMessageBox.Show("Покупка в кредит", "Не найдена услуга коммунальных платежей, убедитесь в целостности базы данных!", GameMessageBox.MessageBoxType.Error); return; }
            PropertyForSale();

            bank = new Bank(GameEnvironment);
            bank.ServiceGrid.Children.Remove(bank.ServiceForm);
            CreditPanel.Children.Add(bank.ServiceForm);
            bank.ServiceForm.Visibility = Visibility.Visible;
            bank.Sum.Text = house.Price.ToString();
            bank.Sum.Tag = house.Price;
            bank.Sum.IsEnabled = false;
            bank.LoadCredits();
            bank.Accept.Click -= bank.Accept_Click;
            bank.Accept.Click += Accept_Click;
            bank.CloseServiceForm.Click -= bank.CloseServiceForm_Click;
            bank.CloseServiceForm.Click += CloseServiceForm_Click;

            RentPanel.Visibility = Visibility.Collapsed;
            CreditPanel.Visibility = Visibility.Visible;
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (bank == null) { return; }

            if (bank.ServiceType.SelectedItem == null) { SellerText.Text = "Уважаемый не балуйтесь, выберите уже тип услуги!"; return; }
            if (bank.ServiceTariff.SelectedItem == null) { SellerText.Text = "Уважаемый не балуйтесь, тариф сам себя не выберет!"; return; }

            Service service = (Service)bank.ServiceType.SelectedItem;
            Tariff tariff = (Tariff)bank.ServiceTariff.SelectedItem;
            if (double.TryParse(bank.Sum.Text, out double sum))
            {
                if (tariff.MinSum != 0 & sum < tariff.MinSum || tariff.MaxSum != 0 & sum > tariff.MaxSum) { SellerText.Text = "Разве не видно что вы не вкладываетесь в рамки тарифа?"; return; }
                double sum_tarrifs = 0;
                foreach (PlayerTariff p_tariff in GameEnvironment.Services.PlayerTariffs)
                {
                    if (p_tariff.Service.UId == service.UId) { sum_tarrifs += p_tariff.Amount; }
                }

                if (service.Type == TransactionType.Withdraw)
                {
                    if (service.TotalMaxDebt != 0 & service.TotalMaxDebt < ((sum_tarrifs + sum) / tariff.Currency.Course))
                    { SellerText.Text = "Уважаемый, введенная вами сумма превышает максимальную сумму по данному типу услуги на: " + ((sum_tarrifs + sum) - (service.TotalMaxDebt * tariff.Currency.Course)) + " " + tariff.Currency.Abbreviation; return; }
                }
                PlayerTariff playerTariff = new PlayerTariff(tariff.UId, tariff.Name, tariff.Currency, tariff.Coefficient,
                                                            tariff.MinSum, tariff.MinSum, tariff.Periodicity,
                                                            tariff.PeriodicityValue, tariff.TermUnit, tariff.MinTerm,
                                                            tariff.MaxTerm, service, sum,
                                                            Convert.ToInt32(bank.TariffPeriod.Value),
                                                            GameEnvironment.GameEvents.GameTimer.DateAndTime, house, tariff.SpecialOffer);
                int uid = GameEnvironment.Services.AllServices.Count + 1; //Возможно прийдеться поменять
                PlayerTariff playerCommunalTariff = new PlayerTariff(uid, house.Name, GameEnvironment.Money.PlayerCurrency[0], 0, house.Price, house.Price, Periodicity.Month, 1, Periodicity.Month, 1, 1, communal_service, house.CommunalPayments  * GameEnvironment.Money.PlayerCurrency[0].Course, 1, GameEnvironment.GameEvents.GameTimer.DateAndTime);
                GameEnvironment.Services.PlayerTariffs.Add(playerCommunalTariff);
                GameEnvironment.Services.PlayerTariffs.Add(playerTariff);

                GameEnvironment.Player.House = new PlayerHouse(house.UId, house.Name, house.Area, house.StorageSize, house.Rent, house.Price, house.CommunalPayments, house.Location, house.Distance, house.IsPurchase, house.IsRent, house.IsCreditPurchase, house.Image, playerCommunalTariff, false, false, true, playerTariff);
                GameEnvironment.GameEvents.Events.Add(new GameEvent(service.UId + ":" + tariff.UId + ":" + sum,
                                                    GameEnvironment.GameEvents.GetDateTimeFromPeriodicity(GameEnvironment.GameEvents.GameTimer.DateAndTime, tariff.Periodicity, tariff.PeriodicityValue),
                                                    tariff.Periodicity, tariff.PeriodicityValue, bank.ProcessingServices, true));
                DateTime date = GameEnvironment.GameEvents.GameTimer.DateAndTime; date = new DateTime(date.Year, date.Month, 15, date.Hour, date.Minute, date.Second);
                GameEnvironment.GameEvents.Events.Add(new GameEvent(communal_service.UId + ":" + playerCommunalTariff.UId + ":" + (house.CommunalPayments * playerCommunalTariff.Currency.Course),
                      GameEnvironment.GameEvents.GetDateTimeFromPeriodicity(date, playerCommunalTariff.Periodicity, playerCommunalTariff.PeriodicityValue),
                      playerCommunalTariff.Periodicity, playerCommunalTariff.PeriodicityValue, CommunalPayment, true));
                SellerText.Text = "Благодарим за покупку, с Вами приятно иметь дело!";
                GameEnvironment.Messages.NewMessage("Агенство недвижимости", "Вы купили: " + house.Name + ". Поздравляем Вас с покупкой!", GameMessages.Icon.Info);

                RentPanel.Visibility = Visibility.Visible;
                CreditPanel.Visibility = Visibility.Hidden;
                CreditPanel.Children.Clear();
            }
        }

        private bool PropertyForSale()
        {
            if (GameEnvironment.Player.House != null)
            {
                GameEvent @event;
                bool isBreak = false;
                for (int i = 0; i >= GameEnvironment.GameEvents.Events.Count - 1; i++)
                {
                    if (GameEnvironment.Player.House.IsRentedOut)
                    {
                        if (GameEnvironment.GameEvents.Events[i].Name == (GameEnvironment.Player.House.PlayerRent.Service.UId + ":" + GameEnvironment.Player.House.PlayerRent.UId + ":" + GameEnvironment.Player.House.Rent))
                        {
                            @event = GameEnvironment.GameEvents.Events[i];
                            double sum = (@event.ResponseTime - GameEnvironment.GameEvents.GameTimer.DateAndTime).TotalDays * GameEnvironment.Player.House.PlayerRent.Amount;
                            if (GameEnvironment.Player.House.PlayerRent.Currency.Withdraw("Выплата аренды", "Агенство недвижимости \"Крыша над головой\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, sum)) { GameMessageBox.Show("Аренда", "Вам не хватает средств на выплату задолженности по текущей аренде!", GameMessageBox.MessageBoxType.Information); return false; }
                            GameEnvironment.GameEvents.Events.Remove(@event);
                            if (isBreak) { break; }
                            isBreak = true;
                        }
                    }
                    else if (GameEnvironment.GameEvents.Events[i].Name == (GameEnvironment.Player.House.PlayerCommunalPayments.Service.UId + ":" + GameEnvironment.Player.House.PlayerCommunalPayments.UId + ":" + GameEnvironment.Player.House.CommunalPayments))
                    {
                        @event = GameEnvironment.GameEvents.Events[i];
                        double sum = (@event.ResponseTime - GameEnvironment.GameEvents.GameTimer.DateAndTime).TotalDays * GameEnvironment.Player.House.PlayerCommunalPayments.Amount;
                        if (GameEnvironment.Player.House.PlayerCommunalPayments.Currency.Withdraw("Оплата коммунальных платежей", "Жилищно-комуунальное хозяйство", GameEnvironment.GameEvents.GameTimer.DateAndTime, sum)) { GameMessageBox.Show("Аренда", "Вам не хватает средств на выплату задолженности по коммунальным платежам!", GameMessageBox.MessageBoxType.Information); return false; }
                        GameEnvironment.GameEvents.Events.Remove(@event);
                        if (isBreak) { break; }
                        isBreak = true;
                    }
                }

                if (GameEnvironment.Player.House.IsPurchased)
                {
                    GameEnvironment.Money.PlayerCurrency[0].TopUp("Продажа недвижимости", GameEnvironment.Player.Name, GameEnvironment.GameEvents.GameTimer.DateAndTime, (GameEnvironment.Player.House.Price * 90 / 100) * GameEnvironment.Money.PlayerCurrency[0].Course);
                }
            }
            return true;
        }

        private void CloseServiceForm_Click(object sender, RoutedEventArgs e)
        {
            RentPanel.Visibility = Visibility.Visible;
            CreditPanel.Visibility = Visibility.Collapsed;
            CreditPanel.Children.Clear();
        }

        private void RentalPayment(GameEvent @event)

        {
            string[] keys = @event.Name.Split(new char[] { ':' });
            List<PlayerTariff> tariffs = GameEnvironment.Services.PlayerTariffs.Where(t => t.Service.UId.ToString() == keys[0] & t.UId.ToString() == keys[1] & t.Amount.ToString() == keys[2]).ToList();
            if (tariffs.Count == 1)
            {
                DateTime dateTime;
                if (tariffs[0].StartDateOfService.Year == GameEnvironment.GameEvents.GameTimer.DateAndTime.Year && tariffs[0].StartDateOfService.Month + 1 == GameEnvironment.GameEvents.GameTimer.DateAndTime.Month)
                {
                    dateTime = tariffs[0].StartDateOfService;
                } else { dateTime = @event.ResponseTime.AddMonths(-1); }
                double sum = (GameEnvironment.GameEvents.GameTimer.DateAndTime - dateTime).TotalDays * tariffs[0].Amount;
                if (tariffs[0].Currency.Withdraw("Выплата аренды", "Агенство недвижимости \"Крыша над головой\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, sum) == false)
                {
                    GameEnvironment.Player.House = null;
                    GameEnvironment.GameEvents.Events.Remove(@event);
                    GameEnvironment.Messages.NewMessage("Агенство недвижимости \"Крыша над головой\"", "Вы были выселены за неуплату аренды!", GameMessages.Icon.Info);
                }
            } else { GameMessageBox.Show("Обработка выплат и взысканий", "Что-то пошло не так, тариф не найден!", GameMessageBox.MessageBoxType.Error); }
        }
        private void CommunalPayment(GameEvent @event)
        {
            string[] keys = @event.Name.Split(new char[] { ':' });
            List<PlayerTariff> tariffs = GameEnvironment.Services.PlayerTariffs.Where(t => t.Service.UId.ToString() == keys[0] & t.UId.ToString() == keys[1] & t.Amount.ToString() == keys[2]).ToList();
            if (tariffs.Count == 1)
            {
                DateTime dateTime;
                if (tariffs[0].StartDateOfService.Year == GameEnvironment.GameEvents.GameTimer.DateAndTime.Year && tariffs[0].StartDateOfService.Month + 1 == GameEnvironment.GameEvents.GameTimer.DateAndTime.Month)
                {
                    dateTime = tariffs[0].StartDateOfService;
                } else { dateTime = @event.ResponseTime.AddMonths(-1); }
                double sum = (GameEnvironment.GameEvents.GameTimer.DateAndTime - dateTime).TotalDays * tariffs[0].Amount;
                if (tariffs[0].Currency.Withdraw("Оплата коммунальных платежей", "Жилищно-коммунальное хозяйство", GameEnvironment.GameEvents.GameTimer.DateAndTime, sum) == false)
                {
                    GameEnvironment.Scenario.GameOver("Вы не оплатили коммунальные платежи");
                }
            }
            else { GameMessageBox.Show("Обработка выплат и взысканий", "Что-то пошло не так, тариф не найден!", GameMessageBox.MessageBoxType.Error); }
        }

        private void RentTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (string.IsNullOrEmpty(textBlock.Text)) { return; }
            double price = Convert.ToDouble(textBlock.Text) * GameEnvironment.Money.PlayerCurrency[0].Course;
            textBlock.Text = Properties.Resources.RentPrice + ": " + price.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation + " (за день)";
        }
        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (string.IsNullOrEmpty(textBlock.Text)) { return; }
            double price = Convert.ToDouble(textBlock.Text) * GameEnvironment.Money.PlayerCurrency[0].Course;
            textBlock.Text = Properties.Resources.Cost + ": " + price.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }

    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((House)value).IsPurchase || ((House)value).IsCreditPurchase;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
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
