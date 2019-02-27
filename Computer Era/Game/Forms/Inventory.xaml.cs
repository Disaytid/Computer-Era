using Computer_Era.Game.Graphics;
using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Forms
{
    public partial class Inventory : UserControl
    {
        int MaxSize;
        readonly GameEnvironment GameEnvironment;

        public Inventory(GameEnvironment gameEnvironment)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;

            MaxSize = 16;
            LoadItems(GameEnvironment.Items, GameEnvironment.Computers);
        }

        public void LoadItems(Items items, Computers computers)
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();
            ViewItems viewItems = new ViewItems();
            items_source = viewItems.GetSaveItemsSource(items, items_source, computers.PlayerComputers);

            foreach (OpticalDisc opticalDisc in GameEnvironment.Items.OpticalDiscs)
            {
                string path = "Resources/discs/" + opticalDisc.Properties.CoverName + ".png";
                Uri uri = new Uri("pack://application:,,,/" + path);

                if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
                {
                    uri = new Uri(opticalDisc.GetIcon(ItemTypes.optical_disc));
                }
                BitmapImage image = new BitmapImage(uri);
                GameGraphics gameGraphics = new GameGraphics();
                BitmapImage icon = gameGraphics.GlueImages(image, new BitmapImage(new Uri("pack://application:,,,/Resources/discs/disc-case.png")));

                items_source.Add(new ListBoxObject(opticalDisc, icon));
            }

            int currenSize = 0;

            foreach (ListBoxObject obj in items_source)
            {
                if (obj.LabelVisibility == Visibility.Visible) { currenSize++; }
            }
            Title.Text = "Кладовка (" + (items_source.Count - currenSize) + " из " + MaxSize + ")";

            InventoryList.ItemsSource = items_source;
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = (Convert.ToInt32(textBlock.Text) * GameEnvironment.Money.PlayerCurrency[0].Course).ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
