using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для HardwareInstallation.xaml
    /// </summary>
    /// 

    public partial class HardwareInstallation : UserControl
    {
        readonly Computers Computers;
        readonly Items Items;
        readonly Money Money;

        public HardwareInstallation(Items items, Computers computers, Money money)
        {
            InitializeComponent();
            Computers = computers;
            Items = items;
            Money = money;
            LoadItems(items);
            LoadComputers(computers);
        }

        public void LoadItems(Items items)
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();

            for (int i = 0; i <= items.Cases.Count - 1; i++) //CASES
            { items_source.Add(new ListBoxObject(items.Cases[i], new BitmapImage(new Uri("pack://application:,,,/Resources/coffin.png")), items.Cases[i], items.Cases[i].ToString())); }

            for (int i = 0; i <= items.Motherboards.Count - 1; i++) //MOTHERBOARDS
            { items_source.Add(new ListBoxObject(items.Motherboards[i], new BitmapImage(new Uri("pack://application:,,,/Resources/circuitry.png")), items.Motherboards[i], items.Motherboards[i].ToString())); }

            for (int i = 0; i <= items.PowerSupplyUnits.Count - 1; i++) //PowerSupplyUnits
            { items_source.Add(new ListBoxObject(items.PowerSupplyUnits[i], new BitmapImage(new Uri("pack://application:,,,/Resources/plug.png")), items.PowerSupplyUnits[i], items.PowerSupplyUnits[i].ToString())); }

            for (int i = 0; i <= items.CPUs.Count - 1; i++) //CPUs
            { items_source.Add(new ListBoxObject(items.CPUs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/processor.png")), items.CPUs[i], items.CPUs[i].ToString())); }

            for (int i = 0; i <= items.RAMs.Count - 1; i++) //RAMs
            { items_source.Add(new ListBoxObject(items.RAMs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/brain.png")), items.RAMs[i], items.RAMs[i].ToString())); }

            for (int i = 0; i <= items.CPUCoolers.Count - 1; i++) //CPUCooler
            { items_source.Add(new ListBoxObject(items.CPUCoolers[i], new BitmapImage(new Uri("pack://application:,,,/Resources/computer-fan.png")), items.CPUCoolers[i], items.CPUCoolers[i].ToString())); }

            for (int i = 0; i <= items.HDDs.Count - 1; i++) //CPUCooler
            { items_source.Add(new ListBoxObject(items.HDDs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/stone-tablet.png")), items.HDDs[i], items.HDDs[i].ToString())); }

            for (int i = 0; i <= items.Monitors.Count - 1; i++) //Monitors
            { items_source.Add(new ListBoxObject(items.Monitors[i], new BitmapImage(new Uri("pack://application:,,,/Resources/tv.png")), items.Monitors[i], items.Monitors[i].ToString())); }

            СomponentsList.ItemsSource = items_source;
        }

        public void LoadComputers(Computers computers)
        {
            foreach (Computer computer in computers.PlayerComputers)
            {
                AssemblyList.Items.Add(computer.Name);
            }
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
                ComputerСomponents.ItemsSource = new Collection<ListBoxObject>();
            }
        }

        //ВАЛИДАЦИЯ

        static void ProblemReport(string message)
        {
            MessageBox.Show(message);
        }

        private enum Operators
        {
            More,           //Больше
            Less,           //Меньше
            Equally,        //Равно
            NotEqual,       //Не равно
            MoreOrEqual,    //Больше или равно
            LessOrEqual,    //Меньше или равно
        }

        private readonly Dictionary<Operators, Func<int, int, bool>> Operations = new Dictionary<Operators, Func<int, int, bool>>
        {
                { Operators.More, (x, y) => x > y },
                { Operators.Less, (x, y) => x < y },
                { Operators.Equally, (x, y) => x == y },
                { Operators.NotEqual, (x, y) => x != y },
                { Operators.MoreOrEqual, (x, y) => x >= y },
                { Operators.LessOrEqual, (x, y) => x <= y },
        };

        private bool IsEquality(int x, int y, Operators @operator, string problem_report)
        {
            if (!Operations.ContainsKey(@operator)) throw new ArgumentException(string.Format("Operation {0} is invalid", @operator), "op");

            bool isValid = true;
            if (!Operations[@operator](x, y)) { isValid = false; ProblemReport(problem_report); }
            return isValid;
        }
        static bool IsСompatibleSocket(Motherboard motherboard, CPU cpu, string problem_report)
        {
            bool isValid = true;
            if (!(motherboard.Properties.Socket == cpu.Properties.Socket)) { isValid = false; ProblemReport(problem_report); }
            return isValid;
        }

        private int GetCount(Collection<ListBoxObject> collection, string type)
        {
            if (collection.Count > 0)
            {
                return collection.Where(i => i.Item.GetTypeValue() == type).Count();
            } else {
                return 0;
            }
        }

        private object GetSingleItem(Collection<ListBoxObject> objects, string type)
        {
            return objects.Single(i => i.Item.GetTypeValue() == type).IObject;
        }

        private Collection<HDD> GetCollectionHDD(Collection<ListBoxObject> collection)
        {
            Collection<HDD> local_collection = new Collection<HDD>();
            
            foreach(ListBoxObject lbo in collection.Where(i => i.Item.GetTypeValue() == "hdd")) { local_collection.Add(lbo.IObject as HDD); }
            return local_collection;
        }

        private void InstallСomponent<T>(Collection<ListBoxObject> components, T obj, Button button)
        {
            components.Add(new ListBoxObject(obj, obj.ToString()));

            ComputerСomponents.Items.Refresh();

            button.Content = "Установлено";
            button.IsEnabled = false;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button & !String.IsNullOrEmpty(AssemblyList.Text))
            {
                if (!(ComputerСomponents.ItemsSource is Collection<ListBoxObject>)) { ComputerСomponents.ItemsSource = new Collection<ListBoxObject>(); }

                Button button = sender as Button;
                Collection<ListBoxObject> items = ComputerСomponents.ItemsSource as Collection<ListBoxObject>;

                if (button.Tag is Case)
                {
                    Case @case = (button.Tag as Case);
                    if (GetCount(items, @case.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "motherboard") == 0 || (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).CheckCompatibility(@case.Properties))
                        {
                            if (@case.CheckСapacity(GetCollectionHDD(items)))
                            {
                                if (GetCount(items, "psu") == 0 || (items.Single(i => i.Item.GetTypeValue() == "psu").IObject as PowerSupplyUnit).CheckCompatibility(@case.Properties))
                                {
                                    items.Add(new ListBoxObject(@case, @case, @case.ToString()));

                                    ComputerСomponents.Items.Refresh();

                                    button.Content = "Установлено";
                                    button.IsEnabled = false;
                                } else {
                                    MessageBox.Show("Блок питания не станет в этот корпус!");
                                }
                            } else {
                                MessageBox.Show("Все диски сюда не влезут!");
                            }
                        } else {
                            MessageBox.Show("Материнская плата не станет в этот корпус!");
                        }
                    } else {
                        MessageBox.Show("У вас уже есть корпус в этой конфигурации!");
                    }
                } else if (button.Tag is Motherboard) {
                    Motherboard motherboard = (button.Tag as Motherboard);
                    if  (GetCount(items, motherboard.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "case") == 0 || motherboard.CheckCompatibility((items.Single(i => i.Item.GetTypeValue() == "case").IObject as Case).Properties))
                        {
                            items.Add(new ListBoxObject(motherboard, motherboard, motherboard.ToString()));

                            ComputerСomponents.Items.Refresh();

                            button.Content = "Установлено";
                            button.IsEnabled = false;
                        } else {
                            MessageBox.Show("Материнская плата не станет в этот корпус!");
                        }
                    } else {
                        MessageBox.Show("У вас уже есть материнская плата в этой конфигурации!");
                    }
                } else if (button.Tag is PowerSupplyUnit) {
                    PowerSupplyUnit psu = (button.Tag as PowerSupplyUnit);
                    if (IsEquality(GetCount(items, psu.GetTypeValue()), 0, Operators.Equally, "У вас уже есть блок питания в этой конфигурации!") &&
                        IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, к чему подключать собрались?"))
                    {
                            if (GetCount(items, "case") == 0 || psu.CheckCompatibility((items.Single(i => i.Item.GetTypeValue() == "case").IObject as Case).Properties))
                            {
                                InstallСomponent<PowerSupplyUnit>(items, psu, button);
                            } else {
                                MessageBox.Show("Блок питания не станет в этот корпус!");
                            }
                    }
                } else if (button.Tag is CPU) {
                    CPU cpu = (button.Tag as CPU);
                    if (IsEquality(GetCount(items, cpu.GetTypeValue()), 0, Operators.Equally, "У вас уже есть процессор в этой конфигурации!") &&
                        IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, и куда вы собрались ставить процессор?") &&
                        IsСompatibleSocket((GetSingleItem(items, "motherboard") as Motherboard), cpu, "Впихнуть невпихуемое? На сокет посмотри!"))
                    { InstallСomponent<CPU>(items, cpu, button); }
                } else if (button.Tag is RAM) {
                    RAM ram = (button.Tag as RAM);
                    if (GetCount(items, "motherboard") == 1)
                    {
                        if (GetCount(items, ram.GetTypeValue()) < (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.RAMSlots)
                        {
                            //Дописать проверку на частоту
                            int volume = 0;

                            foreach (object obj in (items.Where(i => i.Item.GetTypeValue() == "ram")))
                            {
                                volume += (obj as RAM).Properties.Volume;
                            }

                            if (ram.Properties.Volume + volume <= (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.RAMVolume)
                            {
                                if (ram.Properties.RAMTypes == (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.RamType)
                                {
                                    items.Add(new ListBoxObject(ram, ram, ram.ToString()));

                                    ComputerСomponents.Items.Refresh();

                                    button.Content = "Установлено";
                                    button.IsEnabled = false;
                                }  else {
                                    MessageBox.Show("Ну не лезет же, тип памяти другой!");
                                }
                            } else {
                                MessageBox.Show("По объему не подходит!");
                            }
                        } else {
                            MessageBox.Show("Все слоты заняты!");
                        }
                    } else {
                        MessageBox.Show("У вас нет материнской платы, и куда вы собрались вставлять память?");
                    }
                } else if (button.Tag is CPUCooler) { //Дописать проверку на размер
                    CPUCooler cpuCooler = button.Tag as CPUCooler;
                    if (GetCount(items, cpuCooler.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "cpu") == 1)
                        {
                            if (cpuCooler.CheckCompatibility((items.Single(i => i.Item.GetTypeValue() == "cpu").IObject as CPU).Properties))
                            {
                                items.Add(new ListBoxObject(cpuCooler, cpuCooler, cpuCooler.ToString()));

                                ComputerСomponents.Items.Refresh();

                                button.Content = "Установлено";
                                button.IsEnabled = false;
                            } else {
                                MessageBox.Show("Это сюда не встанет!");
                            }
                        } else {
                            MessageBox.Show("Куда же всунуть эту непонятную штуку? Правильно некуда! У вас нет процессора.");
                        }
                    } else  {
                        MessageBox.Show("У вас уже есть куллер для процессора в этой конфигурации!");
                    }
                } else if (button.Tag is HDD) {
                    HDD hdd = button.Tag as HDD;

                    if (GetCount(items, "motherboard") == 1)
                    {
                        if (GetCount(items, hdd.GetTypeValue()) < (items.Single(i => i.Item.GetTypeValue() == "case").IObject as Case).GetCountCompatiblePlaces(hdd.Properties.FormFactor))
                        {
                            if (GetCount(items, hdd.GetTypeValue()) < (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).GetCountCompatibleSlots(hdd.Properties.FormFactor))
                            {
                                items.Add(new ListBoxObject(hdd, hdd, hdd.ToString()));

                                ComputerСomponents.Items.Refresh();

                                button.Content = "Установлено";
                                button.IsEnabled = false;
                            } else {
                                MessageBox.Show("Ничего у вас не выйдет, нет свободных слотов!");
                            }
                        }  else {
                            MessageBox.Show("Увы, но некуда поставить.");
                        }
                    } else {
                        MessageBox.Show("У вас нет материнской платы, и куда вы собрались подключать диск?");
                    }
                } else if (button.Tag is Monitor) {
                    Monitor monitor = button.Tag as Monitor;

                    if (GetCount(items, "motherboard") == 1)
                    {
                        if (GetCount(items, monitor.GetTypeValue()) < (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.VideoInterfaces.Count)
                        {
                            if (GetCount(items, monitor.GetTypeValue()) == 0 & monitor.IsCompatibility((items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.VideoInterfaces))
                            {
                                items.Add(new ListBoxObject(monitor, monitor, monitor.ToString()));

                                ComputerСomponents.Items.Refresh();

                                button.Content = "Установлено";
                                button.IsEnabled = false;
                            } else {
                                MessageBox.Show("Нет совместимы выходов, попробуйте добавить преходник в конфигурацию!");
                            }
                        } else {
                            MessageBox.Show("Нет свободных гнезд для подключения.");
                        }
                    } else {
                        MessageBox.Show("У вас нет материнской платы!");
                    }
                }
            }  
        }

        private void AssemblyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Computer> computer = Computers.PlayerComputers.Where<Computer>(c => c.Name == AssemblyList.Name).ToList();

            if (computer.Count == 1)
            {
                Collection<ListBoxObject> items = new Collection<ListBoxObject>();
                if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].Case, computer[0].Case, computer[0].Case.ToString())); }
                if (computer[0].Motherboard != null) { items.Add(new ListBoxObject(computer[0].Motherboard, computer[0].Motherboard, computer[0].Motherboard.ToString())); }
                if (computer[0].CPU != null) { items.Add(new ListBoxObject(computer[0].CPU, computer[0].CPU, computer[0].CPU.ToString())); }
                if (computer[0].PSU != null) { items.Add(new ListBoxObject(computer[0].PSU, computer[0].PSU, computer[0].PSU.ToString())); }
                if (computer[0].RAMs != null) { foreach (RAM ram in computer[0].RAMs) { items.Add(new ListBoxObject(ram, ram, ram.ToString())); } }

                ComputerСomponents.ItemsSource = items;
            }
            ComputerСomponents.Items.Refresh();
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = (Convert.ToInt32(textBlock.Text) * Money.PlayerCurrency[0].Course).ToString("N3") + " " + Money.PlayerCurrency[0].Abbreviation;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
