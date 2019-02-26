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
