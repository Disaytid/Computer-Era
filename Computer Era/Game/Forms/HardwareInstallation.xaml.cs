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

        private bool IsNullOrCompatibleMotherboard(Collection<ListBoxObject> collection, Motherboard motherboard, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 0 || motherboard.CheckCompatibility((GetSingleItem(collection, "case") as Case).Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsNullOrCompatibleMotherboard(Collection<ListBoxObject> collection, Case @case, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "motherboard") == 0 || (GetSingleItem(collection, "motherboard") as Motherboard).CheckCompatibility(@case.Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsNullOrCompatibleCase(Collection<ListBoxObject> collection, PowerSupplyUnit psu, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 0 || psu.CheckCompatibility((GetSingleItem(collection, "case") as Case).Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsNullOrCompatiblePSU(Collection<ListBoxObject> collection, Case @case, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "psu") == 0 || (GetSingleItem(collection, "psu") as PowerSupplyUnit).CheckCompatibility(@case.Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        static bool IsСompatibleSocket(Motherboard motherboard, CPU cpu, string problem_report)
        {
            bool isValid = false;
            if (motherboard.Properties.Socket == cpu.Properties.Socket) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsСompatibleRAMSlots(Collection<ListBoxObject> collection, RAM ram, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, ram.GetTypeValue()) < (GetSingleItem(collection, "motherboard") as Motherboard).Properties.RAMSlots) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsСompatibleRAMType(Collection<ListBoxObject> collection, RAM ram, string problem_report)
        {
            bool isValid = false;
            if (ram.Properties.RAMTypes == (GetSingleItem(collection, "motherboard") as Motherboard).Properties.RamType) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsСompatibleCPU(Collection<ListBoxObject> collection, CPUCooler cpuCooler, string problem_report)
        {
            bool isValid = false;
            if (cpuCooler.CheckCompatibility((GetSingleItem(collection, "cpu") as CPU).Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsСapacityHDD(Collection<ListBoxObject> collection, Case @case, string problem_report)
        {
            bool isValid = false;
            Collection<HDD> hdd_collection = new Collection<HDD>();
            foreach (ListBoxObject lbo in collection.Where(i => i.Item.GetTypeValue() == "hdd")) { hdd_collection.Add(lbo.IObject as HDD); }
            if (@case.CheckСapacity(hdd_collection)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsEnteringRangeFrequency(RAM ram, Motherboard motherboard, string problem_report)
        {
            bool isValid = false;
            if (motherboard.Properties.MinFrequency <= ram.Properties.ClockFrequency & ram.Properties.ClockFrequency <= motherboard.Properties.MaxFrequency) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsEnteringRangeVolume(Collection<ListBoxObject> collection, RAM ram, string problem_report)
        {
            bool isValid = false;
            int volume = 0;

            foreach (object obj in (collection.Where(i => i.Item.GetTypeValue() == "ram"))) { volume += (obj as RAM).Properties.Volume; }
            if (ram.Properties.Volume + volume <= (GetSingleItem(collection, "motherboard") as Motherboard).Properties.RAMVolume) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsFreeSpaceInstallation(Collection<ListBoxObject> collection, HDD hdd, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, hdd.GetTypeValue()) < ((GetSingleItem(collection, "case") as Case).GetCountCompatiblePlaces(hdd.Properties.FormFactor))) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsFreeSlotsInstallation(Collection<ListBoxObject> collection, HDD hdd, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, hdd.GetTypeValue()) < ((GetSingleItem(collection, "motherboard") as Motherboard).GetCountCompatibleSlots(hdd.Properties.FormFactor))) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }

        private bool IsFreeVideoInterfaces(Collection<ListBoxObject> collection, Monitor monitor, string problem_report)
        {
            bool isValid = false;
            if ((GetCount(collection, monitor.GetTypeValue()) < ((GetSingleItem(collection, "motherboard") as Motherboard).Properties.VideoInterfaces.Count)))
            {
                if (GetCount(collection, monitor.GetTypeValue()) > 0)
                {
                    for (int i = 0; i > (collection.Where(m => m.Item.GetTypeValue() == "monitor").Count()); i++)
                    {
                        Monitor mon = collection[i].IObject as Monitor;

                        Collection<VideoInterface> mon_videoInterfaces = monitor.Compatibility(mon.Properties.VideoInterfaces);
                        if (mon_videoInterfaces.Count() > 1)
                        {
                            if (monitor.IsCompatibility(mon_videoInterfaces)) { isValid = true; break; }
                        }
                    }
                } else { isValid = monitor.IsCompatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties.VideoInterfaces); }
            }
            if (!isValid) { ProblemReport(problem_report); }
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
                    if (IsEquality(GetCount(items, @case.GetTypeValue()), 0, Operators.Equally,"У вас уже есть корпус в этой конфигурации!") &&
                        IsNullOrCompatibleMotherboard(items, @case, "Материнская плата не станет в этот корпус!") &&
                        IsСapacityHDD(items, @case, "Все диски сюда не влезут!") &&
                        IsNullOrCompatiblePSU(items, @case, "Блок питания не станет в этот корпус!"))
                    { InstallСomponent<Case>(items, @case, button); }
                } else if (button.Tag is Motherboard) {
                    Motherboard motherboard = (button.Tag as Motherboard);
                    if  (IsEquality(GetCount(items, motherboard.GetTypeValue()), 0, Operators.Equally, "У вас уже есть материнская плата в этой конфигурации!") &&
                        IsNullOrCompatibleMotherboard(items, motherboard, "Материнская плата не станет в этот корпус!"))
                    {   InstallСomponent<Motherboard>(items, motherboard, button); }
                } else if (button.Tag is PowerSupplyUnit) {
                    PowerSupplyUnit psu = (button.Tag as PowerSupplyUnit);
                    if (IsEquality(GetCount(items, psu.GetTypeValue()), 0, Operators.Equally, "У вас уже есть блок питания в этой конфигурации!") &&
                        IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, к чему подключать собрались?") &&
                        IsNullOrCompatibleCase(items, psu, "Блок питания не станет в этот корпус!"))
                    { InstallСomponent<PowerSupplyUnit>(items, psu, button);}
                } else if (button.Tag is CPU) {
                    CPU cpu = (button.Tag as CPU);
                    if (IsEquality(GetCount(items, cpu.GetTypeValue()), 0, Operators.Equally, "У вас уже есть процессор в этой конфигурации!") &&
                        IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, и куда вы собрались ставить процессор?") &&
                        IsСompatibleSocket((GetSingleItem(items, "motherboard") as Motherboard), cpu, "Впихнуть невпихуемое? На сокет посмотри!"))
                    { InstallСomponent<CPU>(items, cpu, button); }
                } else if (button.Tag is RAM) {
                    RAM ram = (button.Tag as RAM);
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, и куда вы собрались вставлять память?") &&
                        IsСompatibleRAMSlots(items, ram, "Все слоты заняты!") &&
                        IsEnteringRangeFrequency(ram, GetSingleItem(items, "motherboard") as Motherboard, "Материнской платой не поддерживаеться память с такой частатой.") &&
                        IsEnteringRangeVolume(items, ram, "По объему не подходит!") &&
                        IsСompatibleRAMType(items, ram, "Ну не лезет же, тип памяти другой!"))
                    { InstallСomponent<RAM>(items, ram, button); }
                } else if (button.Tag is CPUCooler) { //Дописать проверку на размер
                    CPUCooler cpuCooler = button.Tag as CPUCooler;
                    if (IsEquality(GetCount(items, cpuCooler.GetTypeValue()), 0, Operators.Equally, "У вас уже есть куллер для процессора в этой конфигурации!") &&
                        IsEquality(GetCount(items, "cpu"), 1, Operators.Equally, "Хмм... куда же всунуть эту непонятную штуку? Правильно некуда! У вас нет процессора.") &&
                        IsСompatibleCPU(items, cpuCooler, "Это сюда не встанет!"))
                    { InstallСomponent<CPUCooler>(items, cpuCooler, button); }
                } else if (button.Tag is HDD) {
                    HDD hdd = button.Tag as HDD;
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, и куда вы собрались подключать диск?") &&
                        IsFreeSpaceInstallation(items, hdd, "Увы, но некуда поставить.") &&
                        IsFreeSlotsInstallation(items, hdd, "Ничего у вас не выйдет, нет свободных слотов!"))
                    { InstallСomponent<HDD>(items, hdd, button); }
                } else if (button.Tag is Monitor) {
                    Monitor monitor = button.Tag as Monitor;
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы!") &&
                        IsFreeVideoInterfaces(items, monitor, "Нет свободных гнезд для подключения."))
                    { InstallСomponent<Monitor>(items, monitor, button); }
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
