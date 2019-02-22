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
                if (ServiceTariff.Items.Count > 0)
                {
                    ServiceTariff.SelectedIndex = 0;
                    TariffPeriod.MinValue = ((Tariff)ServiceTariff.SelectedItem).MinTerm;
                    TariffPeriod.MaxValue = ((Tariff)ServiceTariff.SelectedItem).MaxTerm;
                    TariffPeriod.Value = ((Tariff)ServiceTariff.SelectedItem).MinTerm;
                }
            }
        }

        private void ServiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceType.SelectedItem != null)
            {
                ServiceTariff.ItemsSource = ((Service)ServiceType.SelectedItem).Tariffs;
                if (ServiceTariff.Items.Count > 0)
                {
                    ServiceTariff.SelectedIndex = 0;
                    TariffPeriod.MinValue = ((Tariff)ServiceTariff.SelectedItem).MinTerm;
                    TariffPeriod.MaxValue = ((Tariff)ServiceTariff.SelectedItem).MaxTerm;
                    TariffPeriod.Value = ((Tariff)ServiceTariff.SelectedItem).MinTerm;
                }
            }
        }

        private void ServiceTariff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceType.SelectedItem != null)
            {
                TariffPeriod.MinValue = ((Tariff)ServiceTariff.SelectedItem).MinTerm;
                TariffPeriod.MaxValue = ((Tariff)ServiceTariff.SelectedItem).MaxTerm;
                TariffPeriod.Value = ((Tariff)ServiceTariff.SelectedItem).MinTerm;
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceType.SelectedItem == null) { CashierText.Text = "Уважаемый не балуйтесь, выберите уже тип услуги!"; return; }
            if (ServiceType.SelectedItem == null) { CashierText.Text = "Уважаемый не балуйтесь, тариф сам себя не выберет!"; return; }

            Service service = (Service)ServiceType.SelectedItem;
            Tariff tariff = (Tariff)ServiceTariff.SelectedItem;
            if (double.TryParse(Sum.Text, out double sum)) {
                if (tariff.MinSum != 0 & sum < tariff.MinSum && tariff.MaxSum!=0 & sum > tariff.MaxSum) { CashierText.Text = "Разве не видно что вы не вкладываетесь в рамки тарифа?"; return; }
                double sum_tarrifs = 0;
                foreach (Service p_service in Services.PlayerServices)
                {
                    if (p_service.UId == service.UId) { foreach (Tariff p_tariff in p_service.Tariffs) { sum_tarrifs += p_tariff.Amount; } break; }
                }
                if (service.Type == TransactionType.TopUp)
                {
                    if (service.TotalMaxContribution !=0 & service.TotalMaxContribution < ((sum_tarrifs + sum) / tariff.Currency.Course))
                    { CashierText.Text = "Уважаемый, введенная вами сумма превышает максимальную сумму по данному типу услуги на: " + ((sum_tarrifs + sum) - (service.TotalMaxContribution * tariff.Currency.Course)) + " " + tariff.Currency.Abbreviation; return; }
                    if (!tariff.Currency.Withdraw(service.Name, "Банк \"Возмездие\"", GameEvents.GameTimer.DateAndTime, sum)) { CashierText.Text = "У вас нет столько денег, и зачем вы только тратите мое время?"; return; }

                }
                if (service.Type == TransactionType.Withdraw)
                {
                    if (service.TotalMaxDebt != 0 & service.TotalMaxDebt < ((sum_tarrifs + sum) / tariff.Currency.Course))
                    { CashierText.Text = "Уважаемый, введенная вами сумма превышает максимальную сумму по данному типу услуги на: " + ((sum_tarrifs + sum) - (service.TotalMaxDebt * tariff.Currency.Course)) + " " + tariff.Currency.Abbreviation; return; }
                    if (!tariff.Currency.TopUp(service.Name, "Банк \"Возмездие\"", GameEvents.GameTimer.DateAndTime, sum)) { CashierText.Text = "Компьютер завис, сочувствую но мы не сможем перевести вам деньги"; return; }
                }
                Collection<Tariff> tariffs = new Collection<Tariff>();
                tariffs.Add(new Tariff(tariff.UId, tariff.Name, tariff.Currency, tariff.Coefficient, tariff.MinSum, tariff.MinSum, tariff.Periodicity, tariff.PeriodicityValue, tariff.TermUnit, tariff.MinTerm, tariff.MaxTerm, tariff.SpecialOffer, sum));
                Services.PlayerServices.Add(new Service(service.UId, service.Name, service.Type, tariffs, service.TotalMaxDebt, service.TotalMaxContribution));

                GameEvents.Events.Add(new GameEvent(tariff.UId + ":" + sum, GameEvents.GameTimer.DateAndTime, tariff.Periodicity, tariff.PeriodicityValue, ProcessingServices, true));
                LoadListServices();
                CashierText.Text = "Ваш " + service.Name.ToLower() + " одобрен, ваш тарифный план: \"" + tariff.Name + "\".";
                CoinCount.Content = Money.PlayerCurrency[0].Count.ToString("N3") + " " + Money.PlayerCurrency[0].Abbreviation;
            } else { CashierText.Text = "Хватит баловаться! Введите уже сумму и оформим " + service.Name.ToLower() + "!"; }
        }

        private void ProcessingServices(GameEvent @event)
        {
            MessageBox.Show("Пора платить по счетам за: " + @event.Name);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
