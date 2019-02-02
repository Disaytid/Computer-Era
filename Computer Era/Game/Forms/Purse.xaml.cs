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
                  

                    BitmapImage image = new BitmapImage();

                    items_source.Add(new PurseCurrency() { Image = new BitmapImage(uri), Name = currency[i].Name, Count = currency[i].Count.ToString() + " " + currency[i].Abbreviation });
                }
            };

            CurrencyList.ItemsSource = items_source;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }

    class PurseCurrency
    {  
        public ImageSource Image { get; set; }  
        public string Name { get; set; }
        public string Count { get; set; }
    }
}
