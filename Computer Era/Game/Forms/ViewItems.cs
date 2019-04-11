using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Forms
{
    public class ViewItems
    {
        private void AddItemsToItemsSource<C>(Collection<C> items, List<ListBoxObject> items_source, ItemTypes type, Collection<Computer> computers) //Добавление предметов в ItemsSource
        {
            BaseItem baseItem = new BaseItem();
            BitmapImage image = new BitmapImage(new Uri(baseItem.GetIcon(type)));

            for (int i = 0; i <= items.Count - 1; i++)
            {
                bool isInstalled = false;
                foreach (Computer computer in computers)
                {
                    if (computer.Case != null && computer.Case.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.Case); }
                    if (computer.Motherboard != null && computer.Motherboard.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.Motherboard); }
                    if (computer.PSU != null && computer.PSU.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.PSU); }
                    foreach (RAM ram in computer.RAMs) { if (ram != null && ram.GetType() == typeof(C)) { if (items[i].Equals(ram)) { isInstalled = true; break; } } }
                    if (computer.CPU != null && computer.CPU.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.CPU); }
                    if (computer.CPUCooler != null && computer.CPUCooler.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.CPUCooler); }
                    foreach (HDD hdd in computer.HDDs) { if (hdd != null && hdd.GetType() == typeof(C)) { if (items[i].Equals(hdd)) { isInstalled = true; break; } } }
                    foreach (VideoCard videoСard in computer.VideoCards) { if (videoСard != null && videoСard.GetType() == typeof(C)) { if (items[i].Equals(videoСard)) { isInstalled = true; break; } } }
                    foreach (Monitor monitor in computer.Monitors) { if (monitor != null && monitor.GetType() == typeof(C)) { if (items[i].Equals(monitor)) { isInstalled = true; break; } } }
                    foreach (OpticalDrive opticalDrive in computer.OpticalDrives) { if (opticalDrive != null && opticalDrive.GetType() == typeof(C)) { if (items[i].Equals(opticalDrive)) { isInstalled = true; break; } } }
                    foreach (Mouse mouse in computer.Mice) { if (mouse != null && mouse.GetType() == typeof(C)) { if (items[i].Equals(mouse)) { isInstalled = true; break; } } }
                    foreach (Keyboard keyboard in computer.Keyboards) { if (keyboard != null && keyboard.GetType() == typeof(C)) { if (items[i].Equals(keyboard)) { isInstalled = true; break; } } }
                }

                items_source.Add(new ListBoxObject(items[i], image, !isInstalled));
            }
        }
        public List<ListBoxObject> GetSaveItemsSource(Items collection, List<ListBoxObject> list, Collection<Computer> computers) //Загрузка предметов в ListBox
        {
            List<ListBoxObject> items_source = list;

            AddItemsToItemsSource(collection.Cases, items_source, ItemTypes.@case, computers);
            AddItemsToItemsSource(collection.Motherboards, items_source, ItemTypes.motherboard, computers);
            AddItemsToItemsSource(collection.PowerSupplyUnits, items_source, ItemTypes.psu, computers);
            AddItemsToItemsSource(collection.RAMs, items_source, ItemTypes.ram, computers);
            AddItemsToItemsSource(collection.CPUs, items_source, ItemTypes.cpu, computers);
            AddItemsToItemsSource(collection.CPUCoolers, items_source, ItemTypes.cpu_cooler, computers);
            AddItemsToItemsSource(collection.HDDs, items_source, ItemTypes.hdd, computers);
            AddItemsToItemsSource(collection.VideoCards, items_source, ItemTypes.video_card, computers);
            AddItemsToItemsSource(collection.Monitors, items_source, ItemTypes.monitor, computers);
            AddItemsToItemsSource(collection.OpticalDrives, items_source, ItemTypes.optical_drive, computers);
            AddItemsToItemsSource(collection.Mice, items_source, ItemTypes.mouse, computers);
            AddItemsToItemsSource(collection.Keyboards, items_source, ItemTypes.keyboard, computers);

            return items_source;
        }
    }
    public class ListBoxObject
    {
        public BaseItem Item { get; set; }
        public object IObject { get; set; }
        public string Tag { get; set; }
        public bool IsEnabled { get; set; }
        public Visibility LabelVisibility { get; set; }

        public ListBoxObject(object obj, BitmapImage image)
        {
            Item = obj as BaseItem;
            Item.Image = image;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = true;
            LabelVisibility = Visibility.Visible;
        }
        public ListBoxObject(object obj, BitmapImage image, Visibility visibility)
        {
            Item = obj as BaseItem;
            Item.Image = image;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = true;
            LabelVisibility = visibility;
        }
        public ListBoxObject(object obj, BitmapImage image, bool isEnabled)
        {
            Item = obj as BaseItem;
            Item.Image = image;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = isEnabled;
            LabelVisibility = IsEnabled ? Visibility.Hidden : Visibility.Visible;
        }

        public ListBoxObject(object obj)
        {
            Item = obj as BaseItem;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = false;
            LabelVisibility = Visibility.Hidden;
        }
    }
}
