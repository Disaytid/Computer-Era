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

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

    }

    class RealtyObject
    {
        public House House { get; set; }
        public ImageSource Image { get; set; }

        public RealtyObject(House house, ImageSource imageSource)
        {
            House = house;
            Image = imageSource;
        }
    }
}
