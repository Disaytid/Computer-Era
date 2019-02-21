using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для Bank.xaml
    /// </summary>
    public partial class Bank : UserControl
    {
        Money Money;
        GameEvents GameEvents;
        Services Services;

        public Bank(Money money, Services services, GameEvents events)
        {
            InitializeComponent();

            Money = money;
            Services = services;
            GameEvents = events;

            string path = "Resources/currency/" + Money.PlayerCurrency[0].SystemName + ".png";
            Uri uri = new Uri("pack://application:,,,/" + path);

            if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
            {
                uri = new Uri("pack://application:,,,/Resources/currency/coin.png");
            }

            CoinIcon.Source = new BitmapImage(uri);
            CoinIcon.Width = 32;
            CoinIcon.Height = 32;
            CoinCount.Content = Money.PlayerCurrency[0].Count.ToString("N3") + " " + Money.PlayerCurrency[0].Abbreviation;

            LoadListServices();
        }

        private void LoadListServices()
        {
            ListServices.ItemsSource = Services.PlayerServices;
            ListServices.Items.Refresh();
        }

        private void NewService_Click(object sender, RoutedEventArgs e)
        {
            ServiceForm.Visibility = Visibility.Visible;
            ServiceType.ItemsSource = Services.AllServices;
            if (ServiceType.Items.Count > 0)
            {
                ServiceType.SelectedIndex = 0;
                ServiceTariff.ItemsSource = ((Service)ServiceType.SelectedItem).Tariffs;
                if (ServiceTariff.Items.Count > 0) { ServiceTariff.SelectedIndex = 0; }
            }
        }

        private void ServiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceType.SelectedItem != null)
            {
                ServiceTariff.ItemsSource = ((Service)ServiceType.SelectedItem).Tariffs;
                if (ServiceTariff.Items.Count > 0) { ServiceTariff.SelectedIndex = 0; }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceType.SelectedItem != null) {
                if (ServiceTariff.SelectedItem != null)
                {
                    Service service = (Service)ServiceType.SelectedItem;
                    Tariff tariff = (Tariff)ServiceTariff.SelectedItem;
                    double sum = 0;
                    if (double.TryParse(Sum.Text, out sum))
                    {
                        if ((service.Type == TransactionType.TopUp) && (service.TotalMaxContribution >= tariff.MinSum && service.TotalMaxContribution <= tariff.MaxSum ||
                            service.TotalMaxContribution == 0 && service.TotalMaxContribution <= tariff.MinSum && service.TotalMaxContribution <= tariff.MaxSum) || 
                            (service.Type == TransactionType.Withdraw) && (service.TotalMaxDebt >= tariff.MinSum && service.TotalMaxDebt <= tariff.MaxSum ||
                            service.Type == TransactionType.Withdraw && service.TotalMaxDebt == 0 && service.TotalMaxDebt <= tariff.MaxSum)) 
                        {
                            if (sum >= tariff.MinSum && sum <= tariff.MaxSum || sum >= tariff.MinSum && tariff.MaxSum == 0)
                            {
                                Currency currency = null;
                                foreach(Currency l_currency in Money.PlayerCurrency)
                                {
                                    if (l_currency.SystemName == tariff.Currency) { currency = l_currency; break; }
                                }

                                if (currency != null && (service.Type == TransactionType.TopUp && currency.Count >= sum) || (service.Type == TransactionType.Withdraw && sum >= 0))
                                {
                                    if (service.Type == TransactionType.TopUp) { currency.Withdraw( "Депозит", "Банк \"Возмездие\"", GameEvents.GameTimer.DateAndTime, sum); }
                                    Collection<Tariff> tariffs = new Collection<Tariff>();
                                    tariffs.Add(new Tariff(tariff.UId, tariff.Name, tariff.Currency, tariff.Coefficient, tariff.MinSum, tariff.MinSum, tariff.Periodicity, tariff.PeriodicityValue, tariff.SpecialOffer, sum));
                                    Services.PlayerServices.Add(new Service(service.UId, service.Name, service.Type, tariffs, service.TotalMaxDebt, service.TotalMaxContribution));

                                    GameEvents.Events.Add(new GameEvent(tariff.Name, GameEvents.GameTimer.DateAndTime, tariff.Periodicity, tariff.PeriodicityValue, ProcessingServices, true));
                                    LoadListServices();
                                    CashierText.Text = "Ваш " + service.Name.ToLower() + " одобрен, ваш тарифный план: \"" + tariff.Name + "\".";
                                    CoinCount.Content = Money.PlayerCurrency[0].Count.ToString("N3") + " " + Money.PlayerCurrency[0].Abbreviation;
                                } else { CashierText.Text = "С вашим счетом что-то не то, у вас точно есть столько денег?"; }
                            } else { CashierText.Text = "Разве не видно что не вкладываетесь в рамки тарифа?"; }
                        } else { CashierText.Text = "Компьютер завис, попробуйте ввести сумму поменьше."; }
                    } else { CashierText.Text = "Хватит баловаться! Введите уже сумму и оформим " + service.Name.ToLower() + "!"; }
                } else { CashierText.Text = "Уважаемый не балуйтесь, тариф сам себя не выберет!"; }
            } else { CashierText.Text = "Уважаемый не балуйтесь, выберите уже тип услуги!"; }
        }

        private void ProcessingServices()
        {
            MessageBox.Show("Пора платить по счетам!");
        }
    }
}
