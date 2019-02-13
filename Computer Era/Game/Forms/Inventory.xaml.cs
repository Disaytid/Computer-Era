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

        public Inventory(Items items)
        {
            InitializeComponent();
            MaxSize = 16;
            Title.Text = "Кладовка (" + MaxSize + " ячеек)";
            LoadItems(items);
        }

        public void LoadItems(Items items)
        {
            List<ListBoxComponent> items_source = new List<ListBoxComponent>();

            for (int i=0; i <= items.Cases.Count - 1; i++) //CASES
            {items_source.Add(new ListBoxComponent(items.Cases[i], new BitmapImage(new Uri("pack://application:,,,/Resources/coffin.png")), items.Cases[i].ToString()));}

            for (int i = 0; i <= items.Motherboards.Count - 1; i++) //MOTHERBOARDS
            {items_source.Add(new ListBoxComponent(items.Motherboards[i], new BitmapImage(new Uri("pack://application:,,,/Resources/circuitry.png")), items.Motherboards[i].ToString()));}

            for (int i = 0; i <= items.PowerSupplyUnits.Count - 1; i++) //PowerSupplyUnits
            { items_source.Add(new ListBoxComponent(items.PowerSupplyUnits[i], new BitmapImage(new Uri("pack://application:,,,/Resources/plug.png")), items.PowerSupplyUnits[i].ToString())); }

            for (int i = 0; i <= items.CPUs.Count - 1; i++) //CPUs
            { items_source.Add(new ListBoxComponent(items.CPUs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/processor.png")), items.CPUs[i].ToString())); }

            for (int i = 0; i <= items.RAMs.Count - 1; i++) //RAMs
            { items_source.Add(new ListBoxComponent(items.RAMs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/brain.png")), items.RAMs[i].ToString())); }

            for (int i = 0; i <= items.CPUCoolers.Count - 1; i++) //CPUCoolers
            { items_source.Add(new ListBoxComponent(items.CPUCoolers[i], new BitmapImage(new Uri("pack://application:,,,/Resources/computer-fan.png")), items.CPUCoolers[i].ToString())); }

            for (int i = 0; i <= items.HDDs.Count - 1; i++) //HDDs
            { items_source.Add(new ListBoxComponent(items.HDDs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/stone-tablet.png")), items.HDDs[i].ToString())); }

            for (int i = 0; i <= items.Monitors.Count - 1; i++) //Monitors
            { items_source.Add(new ListBoxComponent(items.Monitors[i], new BitmapImage(new Uri("pack://application:,,,/Resources/tv.png")), items.Monitors[i].ToString())); }

            InventoryList.ItemsSource = items_source;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
