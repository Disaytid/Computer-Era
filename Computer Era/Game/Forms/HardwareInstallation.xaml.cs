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
    /// Логика взаимодействия для HardwareInstallation.xaml
    /// </summary>
    public partial class HardwareInstallation : UserControl
    {
        public HardwareInstallation(Items items)
        {
            InitializeComponent();

            LoadItems(items);
        }

        public void LoadItems(Items items)
        {
            List<Item> items_source = new List<Item>();

            for (int i = 0; i <= items.Cases.Count - 1; i++)
            {
                items_source.Add(new Item() { Image = new BitmapImage(new Uri("pack://application:,,,/Resources/coffin.png")), Type = items.Cases[i].Type, Name = items.Cases[i].Name, Price = items.Cases[i].Price, ManufacturingDate = items.Cases[i].ManufacturingDate });
            }
            for (int i = 0; i <= items.Motherboards.Count - 1; i++)
            {
                items_source.Add(new Item() { Image = new BitmapImage(new Uri("pack://application:,,,/Resources/circuitry.png")), Type = items.Motherboards[i].Type, Name = items.Motherboards[i].Name, Price = items.Motherboards[i].Price, ManufacturingDate = items.Motherboards[i].ManufacturingDate });
            }

            СomponentsList.ItemsSource = items_source;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void AddAssembly_Click(object sender, RoutedEventArgs e)
        {
            string name = AssemblyList.Text;

            foreach (String item in AssemblyList.Items)
            {
                if (item == name)
                {
                    name = null;
                    MessageBox.Show("Данное имя уже существует!");
                    break;
                }
            }

            if (name != null)
            {
                AssemblyList.Items.Add(name);
            }
        }
    }
}
