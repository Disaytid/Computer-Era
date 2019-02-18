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

        public readonly Dictionary<ItemTypes, string> DefaulItemIcon = new Dictionary<ItemTypes, string>
        {
            { Objects.ItemTypes.@case, "pack://application:,,,/Resources/coffin.png" },
            { Objects.ItemTypes.motherboard, "pack://application:,,,/Resources/circuitry.png" },
            { Objects.ItemTypes.psu, "pack://application:,,,/Resources/plug.png" },
            { Objects.ItemTypes.ram, "pack://application:,,,/Resources/brain.png" },
            { Objects.ItemTypes.cpu, "pack://application:,,,/Resources/processor.png" },
            { Objects.ItemTypes.cpu_cooler, "pack://application:,,,/Resources/computer-fan.png" },
            { Objects.ItemTypes.hdd, "pack://application:,,,/Resources/stone-tablet.png" },
            { Objects.ItemTypes.video_card, "pack://application:,,,/Resources/cyber-eye.png" },
            { Objects.ItemTypes.monitor, "pack://application:,,,/Resources/tv.png" },
            { Objects.ItemTypes.optical_drive, "pack://application:,,,/Resources/compact-disc.png" },
            { Objects.ItemTypes.mouse, "pack://application:,,,/Resources/mouse.png" },
            { Objects.ItemTypes.keyboard, "pack://application:,,,/Resources/keyboard.png" },
        };

        private void AddItemsToItemsSource<C>(Collection<C> items, List<ListBoxObject> items_source, ItemTypes type) //Добавление предметов в ItemsSource
        {
            if (!DefaulItemIcon.ContainsKey(type)) throw new ArgumentException(string.Format("Operation {0} is invalid", type), "op");
            string path = (string)DefaulItemIcon[type];
            BitmapImage image = new BitmapImage(new Uri(path));
            //<C> lItems = List(typeof(C))(Computers.PlayerComputers.Where(item => item.GetType() == typeof(C)).ToList());


            for (int i = 0; i <= items.Count - 1; i++) {
                bool isInstalled = false;
                foreach (Computer computer in Computers.PlayerComputers)
                {
                    if (computer.Case != null && computer.Case.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.Case); }
                    if (computer.Motherboard != null && computer.Motherboard.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.Motherboard); }
                    if (computer.PSU != null && computer.PSU.GetType() == typeof(C)) { isInstalled = items[i].Equals(computer.PSU); }
                }

                items_source.Add(new ListBoxObject(items[i], image, !isInstalled));
            }
        }   

        public void LoadItems(Items items) //Загрузка предметов в ListBox
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();

            AddItemsToItemsSource(items.Cases, items_source, ItemTypes.@case);
            AddItemsToItemsSource(items.Motherboards, items_source, ItemTypes.motherboard);
            AddItemsToItemsSource(items.RAMs, items_source, ItemTypes.ram);
            AddItemsToItemsSource(items.PowerSupplyUnits, items_source, ItemTypes.psu);
            AddItemsToItemsSource(items.CPUs, items_source, ItemTypes.cpu);
            AddItemsToItemsSource(items.CPUCoolers, items_source, ItemTypes.cpu_cooler);
            AddItemsToItemsSource(items.HDDs, items_source, ItemTypes.hdd);
            AddItemsToItemsSource(items.VideoСards, items_source, ItemTypes.video_card);
            AddItemsToItemsSource(items.Monitors, items_source, ItemTypes.monitor);
            AddItemsToItemsSource(items.OpticalDrives, items_source, ItemTypes.optical_drive);
            AddItemsToItemsSource(items.Mice, items_source, ItemTypes.mouse);
            AddItemsToItemsSource(items.Keyboards, items_source, ItemTypes.keyboard);

            СomponentsList.ItemsSource = items_source;
        }

        public void LoadComputers(Computers computers)
        {
            foreach (Computer computer in computers.PlayerComputers) { AssemblyList.Items.Add(computer.Name); }
        }

        private void AddAssembly_Click(object sender, RoutedEventArgs e)
        {
            string name = AssemblyList.Text;

            foreach (String item in AssemblyList.Items)
            { if (item == name) {  name = string.Empty; MessageBox.Show("Данное имя уже существует!"); break; } }

            if (!string.IsNullOrEmpty(name))
            { AssemblyList.Items.Add(name); ComputerСomponents.ItemsSource = new Collection<ListBoxObject>(); }
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

            bool isValid = Operations[@operator](x, y);
            if (!isValid) { ProblemReport(problem_report); } return isValid;
        }
        private bool IsNullOrCompatibleMotherboard(Collection<ListBoxObject> collection, Motherboard motherboard, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 0 || motherboard.CheckCompatibility((GetSingleItem(collection, "case") as C).Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsNullOrCompatibleMotherboard(Collection<ListBoxObject> collection, C @case, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "motherboard") == 0 || (GetSingleItem(collection, "motherboard") as Motherboard).CheckCompatibility(@case.Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsNullOrCompatibleCase(Collection<ListBoxObject> collection, PowerSupplyUnit psu, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 0 || psu.CheckCompatibility((GetSingleItem(collection, "case") as C).Properties)) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsNullOrCompatiblePSU(Collection<ListBoxObject> collection, C @case, string problem_report)
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
        private bool IsСompatibleInterface(Collection<ListBoxObject> collection, VideoСard videoСard, string problem_report)
        {
            bool isValid = false;

            MotherboardProperties motherboard = (GetSingleItem(collection, "motherboard") as Motherboard).Properties;
            if (GetCount(collection, "video_card") == 0)
            {
                if (videoСard.IsCompatibility(motherboard)) { isValid = true; }
            } else {
                Collection<VideoСard> videoСards = new Collection<VideoСard>();

                for (int i = 0; i > (collection.Where(m => m.Item.GetTypeValue() == "video_card").Count()); i++)
                {
                    VideoСard cVideoСard = collection[i].IObject as VideoСard;
                    int mon_videoInterfaces = cVideoСard.Compatibility(((GetSingleItem(collection, "motherboard") as Motherboard).Properties));

                    if (mon_videoInterfaces > 1) { if (cVideoСard.IsCompatibility(motherboard)) { isValid = true; break; } }
                }
            }

            if (!isValid) { ProblemReport(problem_report); } return isValid;
        }
        private bool IsСapacityHDD(Collection<ListBoxObject> collection, C @case, string problem_report)
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
            if (GetCount(collection, "case") == 0 || GetCount(collection, hdd.GetTypeValue()) < ((GetSingleItem(collection, "case") as C).GetCountCompatiblePlaces(hdd.Properties.FormFactor))) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsFreeSpaceInstallation(Collection<ListBoxObject> collection, OpticalDrive opticalDrive, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 1) { isValid = GetCount(collection, "optical_drive") < 2 ? true : false; } else { isValid = true; } //2 статическое значение слотов под приводы в систмном блоке, потом может преместиться в параметр
            if (!isValid) { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsFreeSlotsInstallation(Collection<ListBoxObject> collection, HDD hdd, string problem_report)
        {
            bool isValid = false;
            int count_interfaces = 0;
            for (int i = 0; i > (collection.Where(m => m.Item.GetTypeValue() == "optical_drive" || m.Item.GetTypeValue() == "hdd").Count()); i++)
            {
                if (collection[i].Item.Type == "optical_drive")
                {
                    OpticalDrive od = collection[i].IObject as OpticalDrive;
                    if (hdd.Properties.Interface == HDDInterface.sata_20 || hdd.Properties.Interface == HDDInterface.sata_30 && od.Properties.Interface == OpticalDriveInterface.SATA)
                    { count_interfaces++; }
                    else if (hdd.Properties.Interface == HDDInterface.IDE && od.Properties.Interface == OpticalDriveInterface.IDE)
                    { count_interfaces++; }
                }
                else if (collection[i].Item.Type == "hdd")
                {
                    HDD lhdd = collection[i].IObject as HDD;
                    if (hdd.Properties.Interface == HDDInterface.sata_20 || hdd.Properties.Interface == HDDInterface.sata_30 && lhdd.Properties.Interface == HDDInterface.sata_20 || lhdd.Properties.Interface == HDDInterface.sata_30)
                    { count_interfaces++; }
                    else if (hdd.Properties.Interface == HDDInterface.IDE && lhdd.Properties.Interface == HDDInterface.IDE)
                    { count_interfaces++; }
                }
            }
            if (count_interfaces < hdd.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties)) { isValid = true; }
            if (!isValid) { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsFreeSlotsInstallation(Collection<ListBoxObject> collection, OpticalDrive opticalDrive, string problem_report)
        {
            bool isValid = false;
            int count_interfaces = 0;
            for (int i = 0; i > (collection.Where(m => m.Item.GetTypeValue() == "optical_drive" || m.Item.GetTypeValue() == "hdd").Count()); i++)
            {
                if (collection[i].Item.Type == "optical_drive")
                {
                    OpticalDrive od = collection[i].IObject as OpticalDrive;
                    if (opticalDrive.Properties.Interface == OpticalDriveInterface.SATA && od.Properties.Interface == OpticalDriveInterface.SATA) { count_interfaces++; }
                    else if (opticalDrive.Properties.Interface == OpticalDriveInterface.IDE && od.Properties.Interface == OpticalDriveInterface.IDE) { count_interfaces++; }
                } else if (collection[i].Item.Type == "hdd") {
                    HDD hdd = collection[i].IObject as HDD;
                    if (opticalDrive.Properties.Interface == OpticalDriveInterface.SATA && hdd.Properties.Interface == HDDInterface.sata_20 || hdd.Properties.Interface == HDDInterface.sata_30)
                    { count_interfaces++; }
                    else if (opticalDrive.Properties.Interface == OpticalDriveInterface.IDE && hdd.Properties.Interface == HDDInterface.IDE) { count_interfaces++; }
                }
            }
            if (count_interfaces < opticalDrive.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties)) { isValid = true; }
            if (!isValid) { ProblemReport(problem_report); } return isValid;
        }
        private bool IsFreeVideoInterfaces(Collection<ListBoxObject> collection, Monitor monitor, string problem_report) //Дописать с учетом видеокарт
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
                        { if (monitor.IsCompatibility(mon_videoInterfaces)) { isValid = true; break; } }
                    }
                } else { isValid = monitor.IsCompatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties.VideoInterfaces); }
            }
            if (!isValid) { ProblemReport(problem_report); } return isValid;
        }

        private bool IsFreeInterfaces(Collection<ListBoxObject> collection, Mouse mouse, string problem_report)
        {
            bool isValid = false;
            int count_interfaces = 0;
            for (int i = 0; i > (collection.Where(m => m.Item.GetTypeValue() == "mouse" || m.Item.GetTypeValue() == "keyboard").Count()); i++)
            {
                if (collection[i].Item.Type == "mouse")
                {
                    Mouse lmouse = collection[i].IObject as Mouse;
                    if (mouse.Properties.Interface == InputInterfaces.USB && lmouse.Properties.Interface == InputInterfaces.USB) { count_interfaces++; }
                    else if (mouse.Properties.Interface == InputInterfaces.PSby2 && lmouse.Properties.Interface == InputInterfaces.PSby2) { count_interfaces++; }
                } else if (collection[i].Item.Type == "keyboard") {
                    Keyboard lkeyboard = collection[i].IObject as Keyboard;
                    if (mouse.Properties.Interface == InputInterfaces.USB && lkeyboard.Properties.Interface == InputInterfaces.USB) { count_interfaces++; }
                    else if (mouse.Properties.Interface == InputInterfaces.PSby2 && lkeyboard.Properties.Interface == InputInterfaces.PSby2) { count_interfaces++; }
                }
            }

            if (GetCount(collection, "case") == 1)
            {
                if (count_interfaces < mouse.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties, (GetSingleItem(collection, "case") as C).Properties)) { isValid = true; }
            } else {
                if (count_interfaces < mouse.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties)) { isValid = true; }
            }
            
            if (!isValid) { ProblemReport(problem_report); } return isValid;
        }
        private bool IsFreeInterfaces(Collection<ListBoxObject> collection, Keyboard keyboard, string problem_report)
        {
            bool isValid = false;
            int count_interfaces = 0;
            for (int i = 0; i > (collection.Where(m => m.Item.GetTypeValue() == "mouse" || m.Item.GetTypeValue() == "keyboard").Count()); i++)
            {
                if (collection[i].Item.Type == "mouse")
                {
                    Mouse lmouse = collection[i].IObject as Mouse;
                    if (keyboard.Properties.Interface == InputInterfaces.USB && lmouse.Properties.Interface == InputInterfaces.USB) { count_interfaces++; }
                    else if (keyboard.Properties.Interface == InputInterfaces.PSby2 && lmouse.Properties.Interface == InputInterfaces.PSby2) { count_interfaces++; }
                }
                else if (collection[i].Item.Type == "keyboard")
                {
                    Keyboard lkeyboard = collection[i].IObject as Keyboard;
                    if (keyboard.Properties.Interface == InputInterfaces.USB && lkeyboard.Properties.Interface == InputInterfaces.USB) { count_interfaces++; }
                    else if (keyboard.Properties.Interface == InputInterfaces.PSby2 && lkeyboard.Properties.Interface == InputInterfaces.PSby2) { count_interfaces++; }
                }
            }
            if (GetCount(collection, "case") == 1)
            {
                if (count_interfaces < keyboard.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties, (GetSingleItem(collection, "case") as C).Properties)) { isValid = true; }
            } else {
                if (count_interfaces < keyboard.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties)) { isValid = true; }
            }

            if (!isValid) { ProblemReport(problem_report); } return isValid;
        }
        private int GetCount(Collection<ListBoxObject> collection, string type)
        {
            if (collection.Count > 0)
            { return collection.Where(i => i.Item.GetTypeValue() == type).Count(); }
            else { return 0; }
        }
        private object GetSingleItem(Collection<ListBoxObject> objects, string type)
        {
            return objects.Single(i => i.Item.GetTypeValue() == type).IObject;
        }
        private void InstallСomponent<T>(Collection<ListBoxObject> components, T obj, Button button)
        {
            components.Add(new ListBoxObject(obj));
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

                if (button.Tag is C)
                {
                    C @case = (button.Tag as C);
                    if (IsEquality(GetCount(items, @case.GetTypeValue()), 0, Operators.Equally, "У вас уже есть корпус в этой конфигурации!") &&
                        IsNullOrCompatibleMotherboard(items, @case, "Материнская плата не станет в этот корпус!") &&
                        IsСapacityHDD(items, @case, "Все диски сюда не влезут!") &&
                        IsNullOrCompatiblePSU(items, @case, "Блок питания не станет в этот корпус!"))
                    { InstallСomponent<C>(items, @case, button); }
                } else if (button.Tag is Motherboard) {
                    Motherboard motherboard = (button.Tag as Motherboard);
                    if (IsEquality(GetCount(items, motherboard.GetTypeValue()), 0, Operators.Equally, "У вас уже есть материнская плата в этой конфигурации!") &&
                        IsNullOrCompatibleMotherboard(items, motherboard, "Материнская плата не станет в этот корпус!"))
                    { InstallСomponent<Motherboard>(items, motherboard, button); }
                } else if (button.Tag is PowerSupplyUnit) {
                    PowerSupplyUnit psu = (button.Tag as PowerSupplyUnit);
                    if (IsEquality(GetCount(items, psu.GetTypeValue()), 0, Operators.Equally, "У вас уже есть блок питания в этой конфигурации!") &&
                        IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, к чему подключать собрались?") &&
                        IsNullOrCompatibleCase(items, psu, "Блок питания не станет в этот корпус!"))
                    { InstallСomponent<PowerSupplyUnit>(items, psu, button); }
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
                } else if (button.Tag is VideoСard) {
                    VideoСard videoCard = button.Tag as VideoСard;
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, видеокарту куда вставлять прикажете?") &&
                        IsСompatibleInterface(items, videoCard, "Нет подходящего интерфейса!"))
                    { InstallСomponent<VideoСard>(items, videoCard, button); }
                } else if (button.Tag is OpticalDrive) {
                    OpticalDrive opticalDrive = button.Tag as OpticalDrive;
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, куда прикажете привод подключить?!") &&
                        IsFreeSpaceInstallation(items, opticalDrive, "Нет свободных мест для установки оптического привода.") &&
                        IsFreeSlotsInstallation(items, opticalDrive, "Нет свободных интерфейсов для подключения оптического привода."))
                    { InstallСomponent<OpticalDrive>(items, opticalDrive, button); }
                } else if (button.Tag is Mouse) {
                    Mouse mouse = button.Tag as Mouse;
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, грызуна некуда воткнуть!") &&
                        IsFreeInterfaces(items, mouse, "Нет свободных гнезд для подключения мышки."))
                    { InstallСomponent<Mouse>(items, mouse, button); }
                } else if (button.Tag is Keyboard) {
                    Keyboard keyboard = button.Tag as Keyboard;
                    if (IsEquality(GetCount(items, "motherboard"), 1, Operators.Equally, "У вас нет материнской платы, клаву некуда воткнуть!") &&
                        IsFreeInterfaces(items, keyboard, "Нет свободных гнезд для подключения клавиатуры."))
                    { InstallСomponent<Keyboard>(items, keyboard, button); }
                }
            }  
        }

        private bool AssemblyListHandleSelection = true;
        string oldName = string.Empty; //Пердыдущее выбранное имя, нужно что бы не срабатывал код при вводе текста в поле, так как в код отдаеться еще не измененный текст
        private void AssemblyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = AssemblyList.Text;
            bool isName = false;
            bool isOldName = false;
            string selectedName = string.Empty;
            if (AssemblyList.SelectedValue != null) { selectedName = AssemblyList.SelectedValue.ToString(); }
            if (!string.IsNullOrEmpty(name)) { foreach (String item in AssemblyList.Items) { if (item == name) { isName = true; break; } } } //Проверяет было ли имя добавлено в имена сборки
            if (!string.IsNullOrEmpty(oldName)) { foreach (String item in AssemblyList.Items) { if (item == oldName) { isOldName = true; break; } } } //Заменить на переменную присваемую в конце

            List<Computer> currentComputer = Computers.PlayerComputers.Where(n => n.Name == name).ToList();
            bool isNewComputer = currentComputer.Count() == 1 ? false : true; //Добавить возможность модификации
            bool isChangedComputer = false;

            if (isNewComputer == false)
            {
                foreach (object obj in ComputerСomponents.Items)
                {
                    object iobj = (obj as ListBoxObject).IObject;
                    if (iobj is C)
                    {
                        isChangedComputer = currentComputer[0].Case == iobj as C ? false : true;
                    } else if (iobj is Motherboard) {
                        isChangedComputer = currentComputer[0].Motherboard == iobj as Motherboard ? false : true;
                    } else if (iobj is CPU) {
                        isChangedComputer = currentComputer[0].CPU == iobj as CPU ? false : true;
                    } else if (iobj is PowerSupplyUnit) {
                        isChangedComputer = currentComputer[0].PSU == iobj as PowerSupplyUnit ? false : true;
                    }  else if (iobj is RAM) {
                        foreach (RAM ram in currentComputer[0].RAMs) { isChangedComputer = ram == iobj as RAM ? false : true; }
                    } else if (iobj is HDD) {
                        foreach (HDD hdd in currentComputer[0].HDDs) { isChangedComputer = hdd == iobj as HDD ? false : true; }
                    } else if (iobj is VideoСard) {
                        foreach (VideoСard videoСard in currentComputer[0].VideoСards) { isChangedComputer = videoСard == iobj as VideoСard ? false : true; }
                    } else if (iobj is OpticalDrive) {
                        foreach (OpticalDrive opticalDrive in currentComputer[0].OpticalDrives) { isChangedComputer = opticalDrive == iobj as OpticalDrive ? false : true; }
                    } else if (iobj is Monitor) {
                        foreach (Monitor monitor in currentComputer[0].Monitors) { isChangedComputer = monitor == iobj as Monitor ? false : true; }
                    } else if (iobj is Mouse) {
                        foreach (Mouse mouse in currentComputer[0].Mice) { isChangedComputer = mouse == iobj as Mouse ? false : true; }
                    } else if (iobj is Keyboard) {
                        foreach (Keyboard keyboard in currentComputer[0].Keyboards) { isChangedComputer = keyboard == iobj as Keyboard ? false : true; }
                    }

                    if (isChangedComputer) { MessageBox.Show("Измененный компьютер!"); break; }
                }
            }

            if (AssemblyListHandleSelection & name != oldName)
            {
                if (ComputerСomponents.Items.Count >= 1 && isNewComputer || !isNewComputer && isChangedComputer)
                {
                    if (isName && isOldName)
                    {
                        Computer newComputer;

                        if (!isChangedComputer)
                        {
                            C @case = null;
                            Motherboard motherboard = null;
                            foreach (object obj in ComputerСomponents.Items)
                            {
                                object iobj = (obj as ListBoxObject).IObject;
                                if (iobj is C) { @case = iobj as C; } else if (iobj is Motherboard) { motherboard = iobj as Motherboard; }
                                if (@case != null && motherboard != null) { break; }
                            }

                            if (@case != null && motherboard != null)
                            {
                                newComputer = new Computer(name, @case, motherboard);
                            }
                            else if (@case != null)
                            {
                                newComputer = new Computer(name, @case);
                            }
                            else if (motherboard != null)
                            {
                                newComputer = new Computer(name, motherboard);
                            }
                            else
                            {
                                newComputer = null;
                                MessageBoxResult result = MessageBox.Show("В списке компонентов нет ни материнской платы не корпуса, сохранение не возможно!" +
                                    "Вы действитьльно хотите перейти к другому списку потяряв изменения в этом?", "", MessageBoxButton.YesNo);
                                if (result == MessageBoxResult.No)
                                {
                                    ComboBox combo = (ComboBox)sender;
                                    AssemblyListHandleSelection = false;
                                    combo.SelectedItem = e.RemovedItems[0];
                                    return;
                                }
                            }
                        } else {
                            newComputer = currentComputer[0];
                        }

                        if (newComputer != null) {
                            foreach (object obj in ComputerСomponents.Items)
                            {
                                object iobj = (obj as ListBoxObject).IObject;
                                if (iobj is C)
                                {
                                    newComputer.Case = iobj as C;
                                } else if (iobj is Motherboard) {
                                    newComputer.Motherboard = iobj as Motherboard;
                                } else if (iobj is CPU) {
                                    newComputer.CPU = iobj as CPU;
                                } else if (iobj is PowerSupplyUnit) {
                                    newComputer.PSU = iobj as PowerSupplyUnit;
                                } else if (iobj is RAM) {
                                    newComputer.RAMs.Add(iobj as RAM);
                                } else if (iobj is HDD) {
                                    newComputer.HDDs.Add(iobj as HDD);
                                } else if (iobj is VideoСard) {
                                    newComputer.VideoСards.Add(iobj as VideoСard);
                                } else if (iobj is OpticalDrive) {
                                    newComputer.OpticalDrives.Add(iobj as OpticalDrive);
                                } else if (iobj is Monitor) {
                                    newComputer.Monitors.Add(iobj as Monitor);
                                } else if (iobj is Mouse) {
                                    newComputer.Mice.Add(iobj as Mouse);
                                } else if (iobj is Keyboard) {
                                    newComputer.Keyboards.Add(iobj as Keyboard);
                                }
                            }

                            if (!isChangedComputer)
                            {
                                Computers.PlayerComputers.Add(newComputer);
                                MessageBox.Show("Добавлен компьютер");
                            } else {
                                MessageBox.Show("Компьютер изменен!");
                            }

                            ComputerСomponents.ItemsSource = null;
                            ComputerСomponents.Items.Clear();
                        }


                    } else {
                        MessageBox.Show("Имя этой сборки не существует, добавьте его в список!");
                    }
                }

                ComputerСomponents.ItemsSource = null;
                ComputerСomponents.Items.Clear();
            }

            if (true) //!isNewComputer
            { 
                List<Computer> computer = Computers.PlayerComputers.Where(n => n.Name == selectedName).ToList();

                if (computer.Count == 1)
                {
                    Collection<ListBoxObject> items = new Collection<ListBoxObject>();
                    if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].Case)); }
                    if (computer[0].Motherboard != null) { items.Add(new ListBoxObject(computer[0].Motherboard)); }
                    if (computer[0].CPU != null) { items.Add(new ListBoxObject(computer[0].CPU)); }
                    if (computer[0].PSU != null) { items.Add(new ListBoxObject(computer[0].PSU)); }
                    if (computer[0].RAMs != null) { foreach (RAM ram in computer[0].RAMs) { items.Add(new ListBoxObject(ram)); } }
                    if (computer[0].HDDs != null) { foreach (HDD hdd in computer[0].HDDs) { items.Add(new ListBoxObject(hdd)); } }
                    if (computer[0].VideoСards != null) { foreach (VideoСard videoСard in computer[0].VideoСards) { items.Add(new ListBoxObject(videoСard)); } }
                    if (computer[0].OpticalDrives != null) { foreach (OpticalDrive opticalDrive in computer[0].OpticalDrives) { items.Add(new ListBoxObject(opticalDrive)); } }
                    if (computer[0].Monitors != null) { foreach (Monitor monitor in computer[0].Monitors) { items.Add(new ListBoxObject(monitor)); } }
                    if (computer[0].Mice != null) { foreach (Mouse mouse in computer[0].Mice) { items.Add(new ListBoxObject(mouse)); } }
                    if (computer[0].Keyboards != null) { foreach (Keyboard keyboard in computer[0].Keyboards) { items.Add(new ListBoxObject(keyboard)); } }

                    ComputerСomponents.ItemsSource = items;
                }
            }

            AssemblyListHandleSelection = true;
            ComputerСomponents.Items.Refresh();
            oldName = name;
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
