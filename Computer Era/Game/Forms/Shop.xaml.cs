using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Keyboard = Computer_Era.Game.Objects.Keyboard;
using Mouse = Computer_Era.Game.Objects.Mouse;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для Shop.xaml
    /// </summary>
    public partial class Shop : UserControl
    {
        readonly GameEnvironment GameEnvironment;

        int StorePercentage = 20; //Процент наценки магазином
        public Shop(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;
            string path = "Resources/currency/" + GameEnvironment.Money.PlayerCurrency[0].SystemName + ".png";
            Uri uri = new Uri("pack://application:,,,/" + path);

            if (System.IO.File.Exists(System.IO.Path.GetFullPath(path)) == false)
            {
                uri = new Uri("pack://application:,,,/Resources/currency/coin.png");
            }

            CoinIcon.Source = new BitmapImage(uri);
            CoinIcon.Width = 32;
            CoinIcon.Height = 32;
            CoinCount.Content = GameEnvironment.Money.PlayerCurrency[0].Count.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;

            AddItem(GameEnvironment.Items, Properties.Resources.All);
        }

        private void AddItemsToItemsSource<C>(Collection<C> items, List<ListBoxObject> items_source, ItemTypes type) //Добавление предметов в ItemsSource
        {
            BaseItem baseItem = new BaseItem();
            BitmapImage image = new BitmapImage(new Uri(baseItem.GetIcon(type)));

            for (int i = 0; i <= items.Count - 1; i++)
            {
                items_source.Add(new ListBoxObject(items[i], image));
            }
        }

        public void AddItem(Items collection, string type) //Загрузка предметов в ListBox
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();

            if (type == Properties.Resources.All)
            {
                AddItemsToItemsSource(collection.AllCases, items_source, ItemTypes.@case);
                AddItemsToItemsSource(collection.AllMotherboards, items_source, ItemTypes.motherboard);
                AddItemsToItemsSource(collection.AllPowerSupplyUnits, items_source, ItemTypes.psu);
                AddItemsToItemsSource(collection.AllRAMs, items_source, ItemTypes.ram);
                AddItemsToItemsSource(collection.AllCPUs, items_source, ItemTypes.cpu);
                AddItemsToItemsSource(collection.AllCPUCoolers, items_source, ItemTypes.cpu_cooler);
                AddItemsToItemsSource(collection.AllHDDs, items_source, ItemTypes.hdd);
                AddItemsToItemsSource(collection.AllVideoCards, items_source, ItemTypes.video_card);
                AddItemsToItemsSource(collection.AllMonitors, items_source, ItemTypes.monitor);
                AddItemsToItemsSource(collection.AllOpticalDrives, items_source, ItemTypes.optical_drive);
                AddItemsToItemsSource(collection.AllMice, items_source, ItemTypes.mouse);
                AddItemsToItemsSource(collection.AllKeyboards, items_source, ItemTypes.keyboard);
            } else if (type == Properties.Resources.Case) {
                AddItemsToItemsSource(collection.AllCases, items_source, ItemTypes.@case);
            } else if (type == Properties.Resources.Motherboard) {
                AddItemsToItemsSource(collection.AllMotherboards, items_source, ItemTypes.motherboard);
            } else if (type == Properties.Resources.PSU) {
                AddItemsToItemsSource(collection.AllPowerSupplyUnits, items_source, ItemTypes.psu);
            } else if (type == Properties.Resources.RAM) {
                AddItemsToItemsSource(collection.AllRAMs, items_source, ItemTypes.ram);
            } else if (type == Properties.Resources.CPU) {
                AddItemsToItemsSource(collection.AllCPUs, items_source, ItemTypes.cpu);
            } else if (type == Properties.Resources.CPUCooler) {
                AddItemsToItemsSource(collection.AllCPUCoolers, items_source, ItemTypes.cpu_cooler);
            } else if (type == Properties.Resources.HDD) {
                AddItemsToItemsSource(collection.AllHDDs, items_source, ItemTypes.hdd);
            } else if (type == Properties.Resources.VideoCard) {
                AddItemsToItemsSource(collection.AllVideoCards, items_source, ItemTypes.video_card);
            } else if (type == Properties.Resources.Monitor) {
                AddItemsToItemsSource(collection.AllMonitors, items_source, ItemTypes.monitor);
            } else if (type == Properties.Resources.OpticalDrive) {
                AddItemsToItemsSource(collection.AllOpticalDrives, items_source, ItemTypes.optical_drive);
            } else if (type == Properties.Resources.Mouse) {
                AddItemsToItemsSource(collection.AllMice, items_source, ItemTypes.mouse);
            } else if (type == Properties.Resources.Keyboard) {
                AddItemsToItemsSource(collection.AllKeyboards, items_source, ItemTypes.keyboard);
            }

            ComputerСomponents.ItemsSource = items_source;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (GameEnvironment.Player.House == null) { GameMessageBox.Show("Покупка", "Вам негде это хранить, для начала обзаведитесь жильем.", GameMessageBox.MessageBoxType.Information); return; }
            Button button = sender as Button;

            if (button.Tag is BaseItem)
            {
                double price = (button.Tag as BaseItem).Price * GameEnvironment.Money.PlayerCurrency[0].Course;
                price += price / 100 * StorePercentage;

                if (price <= GameEnvironment.Money.PlayerCurrency[0].Count)
                {
                    GameEnvironment.Money.PlayerCurrency[0].Withdraw("Оплата покупки: " + (button.Tag as BaseItem).Name, Properties.Resources.ComponentStoreFullName, GameEnvironment.GameEvents.GameTimer.DateAndTime, price);
                    CoinCount.Content = GameEnvironment.Money.PlayerCurrency[0].Count.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;

                    if (button.Tag is Case)
                    {
                        Case @case = (button.Tag as Case);
                        GameEnvironment.Items.Cases.Add(new Case(@case.Uid, @case.Name, @case.GetTypeValue(), @case.Price, @case.ManufacturingDate, @case.Properties));
                    } else if (button.Tag is Motherboard) {
                        Motherboard motherboard = (button.Tag as Motherboard);
                        GameEnvironment.Items.Motherboards.Add(new Motherboard(motherboard.Uid, motherboard.Name, motherboard.GetTypeValue(), motherboard.Price, motherboard.ManufacturingDate, motherboard.Properties));
                    } else if (button.Tag is PowerSupplyUnit) {
                        PowerSupplyUnit psu = (button.Tag as PowerSupplyUnit);
                        GameEnvironment.Items.PowerSupplyUnits.Add(new PowerSupplyUnit(psu.Uid, psu.Name, psu.GetTypeValue(), psu.Price, psu.ManufacturingDate, psu.Properties));
                    } else if (button.Tag is CPU) {
                        CPU cpu = (button.Tag as CPU);
                        GameEnvironment.Items.CPUs.Add(new CPU(cpu.Uid, cpu.Name, cpu.GetTypeValue(), cpu.Price, cpu.ManufacturingDate, cpu.Properties));
                    } else if (button.Tag is RAM) {
                        RAM ram = (button.Tag as RAM);
                        GameEnvironment.Items.RAMs.Add(new RAM(ram.Uid, ram.Name, ram.GetTypeValue(), ram.Price, ram.ManufacturingDate, ram.Properties));
                    } else if (button.Tag is CPUCooler) {
                        CPUCooler cpuCooler = button.Tag as CPUCooler;
                        GameEnvironment.Items.CPUCoolers.Add(new CPUCooler(cpuCooler.Uid, cpuCooler.Name, cpuCooler.GetTypeValue(), cpuCooler.Price, cpuCooler.ManufacturingDate, cpuCooler.Properties));
                    } else if (button.Tag is HDD) {
                        HDD hdd = button.Tag as HDD;
                        GameEnvironment.Items.HDDs.Add(new HDD(hdd.Uid, hdd.Name, hdd.GetTypeValue(), hdd.Price, hdd.ManufacturingDate, hdd.Properties));
                    } else if (button.Tag is Monitor) {
                        Monitor monitor = button.Tag as Monitor;
                        GameEnvironment.Items.Monitors.Add(new Monitor(monitor.Uid, monitor.Name, monitor.GetTypeValue(), monitor.Price, monitor.ManufacturingDate, monitor.Properties));
                    } else if (button.Tag is VideoCard) {
                        VideoCard videoCard = button.Tag as VideoCard;
                        GameEnvironment.Items.VideoCards.Add(new VideoCard(videoCard.Uid, videoCard.Name, videoCard.GetTypeValue(), videoCard.Price, videoCard.ManufacturingDate, videoCard.Properties));
                    } else if (button.Tag is OpticalDrive) {
                        OpticalDrive opticalDrive = button.Tag as OpticalDrive;
                        GameEnvironment.Items.OpticalDrives.Add(new OpticalDrive(opticalDrive.Uid, opticalDrive.Name, opticalDrive.GetTypeValue(), opticalDrive.Price, opticalDrive.ManufacturingDate, opticalDrive.Properties));
                 } else if (button.Tag is Keyboard) {
                        Keyboard keyboard = button.Tag as Keyboard;
                        GameEnvironment.Items.Keyboards.Add(new Keyboard(keyboard.Uid, keyboard.Name, keyboard.GetTypeValue(), keyboard.Price, keyboard.ManufacturingDate, keyboard.Properties));
                    }

                    SellerText.Text = "Спасибо за покупку " + (button.Tag as BaseItem).Name + ", хороший выбор!";
                } else {
                    SellerText.Text = "Извини дружище, нет денег нет товара.";
                }           } else if (button.Tag is Mouse) {
                        Mouse mouse = button.Tag as Mouse;
                        GameEnvironment.Items.Mice.Add(new Mouse(mouse.Uid, mouse.Name, mouse.GetTypeValue(), mouse.Price, mouse.ManufacturingDate, mouse.Properties));
            
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            double price = Convert.ToInt32(textBlock.Text) * GameEnvironment.Money.PlayerCurrency[0].Course;
            price += price / 100 * StorePercentage;
            textBlock.Text = price.ToString("N3") + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation;
        }
        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Description.Text = (string)(sender as Grid).Tag;
        }
        private void SelectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionType.SelectedItem != null && GameEnvironment != null)
            {
                ComboBoxItem item = (ComboBoxItem)SelectionType.SelectedItem;
                AddItem(GameEnvironment.Items, item.Content.ToString());
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
