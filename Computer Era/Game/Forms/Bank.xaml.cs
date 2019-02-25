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
            ListServices.ItemsSource = Services.PlayerTariffs;
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
                { ServiceTariff.SelectedIndex = 0; }
            }
        }

        private void ServiceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceType.SelectedItem != null)
            {
                ServiceTariff.ItemsSource = ((Service)ServiceType.SelectedItem).Tariffs;
                if (ServiceTariff.Items.Count > 0) { ServiceTariff.SelectedIndex = 0; }
            }
            CalculationOfTheTotal();
        }

        private void ServiceTariff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceTariff.SelectedItem != null)
            {
                Tariff tariff = (Tariff)ServiceTariff.SelectedItem;
                TariffDescription.Text = tariff.ToString(); TariffDescription.Visibility = Visibility.Visible;
                LabelPeriod.Content = GameEvents.FromPeriodicityToLocalizedString(tariff.Periodicity) + ": ";
                TariffPeriod.MinValue = tariff.MinTerm;
                TariffPeriod.MaxValue = tariff.MaxTerm;
            } else {
                TariffDescription.Visibility = Visibility.Collapsed;
            }
            CalculationOfTheTotal();
        }

        private void Sum_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculationOfTheTotal();
        }

        private void TariffPeriod_ValueChanged(object sender, ControlLib.ValueChangedEventArgs e)
        {
            CalculationOfTheTotal();
        }

        private void CalculationOfTheTotal()
        {
            if (ServiceTariff.SelectedItem != null && ServiceType.SelectedItem != null)
            {
                Service service = (Service)ServiceType.SelectedItem;
                Tariff tariff = (Tariff)ServiceTariff.SelectedItem;
                if (service.Type == TransactionType.TopUp)
                {
                    if (double.TryParse(Sum.Text, out double sum)) { SummaryInformation.Content = "Итого начисления составят: " + ((sum * tariff.Coefficient / 100) * TariffPeriod.Value); }
                } else if (service.Type == TransactionType.Withdraw) {
                    if (double.TryParse(Sum.Text, out double sum)) { SummaryInformation.Content = "Итоговая сумма выплат составит: " + (sum + ((sum * tariff.Coefficient / 100)) * TariffPeriod.Value); }
                }
            } else { SummaryInformation.Content = ""; }
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
                foreach (PlayerTariff p_tariff in Services.PlayerTariffs)
                {
                    if (p_tariff.Service.UId == service.UId) { sum_tarrifs += p_tariff.Amount; break; }
                }
                if (service.Type == TransactionType.TopUp)
                {
                    if (service.TotalMaxContribution !=0 & service.TotalMaxContribution < ((sum_tarrifs + sum) / tariff.Currency.Course))
                    { CashierText.Text = "Уважаемый, введенная вами сумма превышает максимальную сумму по данному типу услуги на: " + ((sum_tarrifs + sum) - (service.TotalMaxContribution * tariff.Currency.Course)) + " " + tariff.Currency.Abbreviation; return; }
                    if (tariff.Currency.Withdraw(service.Name, Properties.Resources.BankName, GameEvents.GameTimer.DateAndTime, sum) == false) { CashierText.Text = "У вас нет столько денег, и зачем вы только тратите мое время?"; return; }

                }
                if (service.Type == TransactionType.Withdraw)
                {
                    if (service.TotalMaxDebt != 0 & service.TotalMaxDebt < ((sum_tarrifs + sum) / tariff.Currency.Course))
                    { CashierText.Text = "Уважаемый, введенная вами сумма превышает максимальную сумму по данному типу услуги на: " + ((sum_tarrifs + sum) - (service.TotalMaxDebt * tariff.Currency.Course)) + " " + tariff.Currency.Abbreviation; return; }
                    if (tariff.Currency.TopUp(service.Name, Properties.Resources.BankName, GameEvents.GameTimer.DateAndTime, sum) == false) { CashierText.Text = "Компьютер завис, сочувствую но мы не сможем перевести вам деньги"; return; }
                }
                Collection<Tariff> tariffs = new Collection<Tariff>();
                Services.PlayerTariffs.Add(new PlayerTariff(tariff.UId, tariff.Name, tariff.Currency, tariff.Coefficient, tariff.MinSum, tariff.MinSum, tariff.Periodicity, tariff.PeriodicityValue, tariff.TermUnit, tariff.MinTerm, tariff.MaxTerm, service, sum, Convert.ToInt32(TariffPeriod.Value), tariff.SpecialOffer));

                GameEvents.Events.Add(new GameEvent(service.UId + ":" + tariff.UId + ":" + sum, GameEvents.GameTimer.DateAndTime, tariff.Periodicity, tariff.PeriodicityValue, ProcessingServices, true));
                LoadListServices();
                CashierText.Text = "Ваш " + service.Name.ToLower() + " одобрен, ваш тарифный план: \"" + tariff.Name + "\".";
                CoinCount.Content = Money.PlayerCurrency[0].Count.ToString("N3") + " " + Money.PlayerCurrency[0].Abbreviation;
            } else { CashierText.Text = "Хватит баловаться! Введите уже сумму и оформим " + service.Name.ToLower() + "!"; }
        }

        private void ProcessingServices(GameEvent @event)
        {
            string[] keys = @event.Name.Split(new char[] { ':' });
            List<PlayerTariff> tariffs = Services.PlayerTariffs.Where(t => t.Service.UId.ToString() == keys[0] & t.UId.ToString() == keys[1] & t.Amount.ToString() == keys[2]).ToList();
            if (tariffs.Count == 1)
            {
                PlayerTariff tariff = tariffs[0];
                if (tariff.Service.Type == TransactionType.TopUp)
                {
                    tariff.Currency.TopUp("Выплата по услуге \"" + tariff.Service.Name + "\" (" + tariff.Name + ")", Properties.Resources.BankName, GameEvents.GameTimer.DateAndTime, (tariff.Amount * tariff.Coefficient / 100));
                    if (DateTime.Compare(GetDateByPeriod(tariff.StartDateOfService, tariff.TermUnit, tariff.Term), @event.ResponseTime) <= 0) {
                        GameEvents.Events.Remove(@event);
                        tariff.Currency.TopUp("Возврат средств в связи с истечением периода оказания услуги \"" + tariff.Service.Name + "\" (" + tariff.Name + ")", Properties.Resources.BankName, GameEvents.GameTimer.DateAndTime, (tariff.Amount * tariff.Coefficient / 100));
                    }
                } else if (tariff.Service.Type == TransactionType.Withdraw) {
                    if (tariff.Currency.Withdraw("Взыскание по услуге\"" + tariff.Service.Name + "\" (" + tariff.Name + ")", Properties.Resources.BankName, GameEvents.GameTimer.DateAndTime, (tariff.Amount * tariff.Coefficient / 100))) { } //ВЫЗОВ СОБЫТИЯ GAME_OVER если не хватает денег (Игрок банкрот), исключение если есть залог
                    if (DateTime.Compare(GetDateByPeriod(tariff.StartDateOfService, tariff.TermUnit, tariff.Term), @event.ResponseTime) <= 0) { GameEvents.Events.Remove(@event); }
                }
            } else {
                MessageBox.Show("Что-то пошло не так, тариф не найден!");
            }
        }

        private DateTime GetDateByPeriod(DateTime dateTime, Periodicity periodicity, int value)
        {
            if (periodicity == Periodicity.Year) { return dateTime.AddYears(value); }
            else if (periodicity == Periodicity.Month) { return dateTime.AddMonths(value); }
            else if (periodicity == Periodicity.Week) { return dateTime.AddDays(7 * value); }
            else if (periodicity == Periodicity.Day) { return dateTime.AddDays(value); }
            else if (periodicity == Periodicity.Hour) { return dateTime.AddHours(value); }
            else if (periodicity == Periodicity.Minute) { return dateTime.AddMinutes(value); }
            else { return dateTime; }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
