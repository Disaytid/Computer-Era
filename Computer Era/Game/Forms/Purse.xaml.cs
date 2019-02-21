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
    /// Логика взаимодействия для Purse.xaml
    /// </summary>
    public partial class Purse : UserControl
    {

        public Purse(Collection<Currency> player_currency, DateTime game_data)
        {
            InitializeComponent();

            LoadICurrency(player_currency, game_data);
        }

        public void LoadICurrency(Collection<Currency> currency, DateTime game_data)
        {
            List<PurseCurrency> items_source = new List<PurseCurrency>();

            for (int i = 0; i <= currency.Count - 1; i++)
            {
                if (currency[i].DateAppearance <= game_data) {
                    string path = "Resources/currency/" + currency[i].SystemName + ".png";
                    Uri uri = new Uri("pack://application:,,,/" + path);

                    if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
                    {
                        uri = new Uri("pack://application:,,,/Resources/currency/coin.png");
                    }

                    items_source.Add(new PurseCurrency() { Currency = currency[i], Image = new BitmapImage(uri), Name = currency[i].Name, Count = currency[i].Count.ToString() + " " + currency[i].Abbreviation });
                }
            };

            CurrencyList.ItemsSource = items_source;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void CurrencyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrencyList.SelectedItem != null)
            {
                PurseCurrency item = CurrencyList.SelectedItem as PurseCurrency;
                HistoryList.Items.Clear();

                foreach (Transaction transaction in item.Currency.TransactionHistory)
                {
                    HistoryList.Items.Add(transaction);
                }
            }
        }
    }

    public class DataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DateTime)value).ToString("dd.MM.yy \r\n HH:mm");
         }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    class PurseCurrency
    {  
        public Currency Currency { get; set; }
        public ImageSource Image { get; set; }
        public string Name { get; set; }
        public string Count { get; set; }
    }
}
