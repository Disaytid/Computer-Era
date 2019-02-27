using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Логика взаимодействия для DiscStand.xaml
    /// </summary>
    public partial class DiscStand : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        int StorePercentage = 20; //Процент наценки магазином
        public DiscStand(GameEnvironment gameEnvironment)
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

            AddItem(GameEnvironment.Items);
        }

        private void GluingImages(Uri image, Uri cover)
        {
            
        }

        private void AddItemsToItemsSource(Collection<OpticalDisc> items, List<ListBoxObject> items_source, ItemTypes type) //Добавление предметов в ItemsSource
        {
            BaseItem baseItem = new BaseItem();

            for (int i = 0; i <= items.Count - 1; i++)
            {
                string path = "Resources/discs/" + items[i].Properties.CoverName + ".png";
                Uri uri = new Uri("pack://application:,,,/" + path);

                if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
                {
                    uri = new Uri("pack://application:,,,/Resources/discs/disc-cover_empty.png");
                }
                BitmapImage image = new BitmapImage(uri);

                items_source.Add(new ListBoxObject(items[i], image));
            }
        }

        public void AddItem(Items collection) //Загрузка предметов в ListBox
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();
            AddItemsToItemsSource(collection.AllOpticalDiscs, items_source, ItemTypes.optical_disc);

            DiscsList.ItemsSource = items_source;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            double price = Convert.ToInt32(textBlock.Text) * GameEnvironment.Money.PlayerCurrency[0].Course;
            price += price / 100 * StorePercentage;
            textBlock.Text = price.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Tag is BaseItem)
            {
                double price = (button.Tag as BaseItem).Price * GameEnvironment.Money.PlayerCurrency[0].Course;
                price += price / 100 * StorePercentage;

                if (price <= GameEnvironment.Money.PlayerCurrency[0].Count)
                {
                    GameEnvironment.Money.PlayerCurrency[0].Withdraw("Оплата покупки: " + (button.Tag as BaseItem).Name, "Магазин \"Клепаем сами\"", GameEnvironment.GameEvents.GameTimer.DateAndTime, price);
                    CoinCount.Content = GameEnvironment.Money.PlayerCurrency[0].Count.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;

                    if (button.Tag is OpticalDisc)
                    {
                        OpticalDisc opticalDisc = (button.Tag as OpticalDisc);
                        GameEnvironment.Items.OpticalDiscs.Add(new OpticalDisc(opticalDisc.Uid, opticalDisc.Name, opticalDisc.GetTypeValue(), opticalDisc.Price, opticalDisc.ManufacturingDate, opticalDisc.Properties));
                    }

                    SellerText.Text = "Спасибо за покупку " + (button.Tag as BaseItem).Name + ", держи свой компакт-диск!";
                }
                else
                {
                    SellerText.Text = "Извини, без денег я тебе его не отдам.";
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
