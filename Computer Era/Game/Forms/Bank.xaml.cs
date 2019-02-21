using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
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
    }
}
