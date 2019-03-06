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
        readonly GameEnvironment GameEnvironment;

        public HardwareInstallation(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;
            LoadItems(GameEnvironment.Items);
            LoadComputers(GameEnvironment.Computers);
        } 

        public void LoadItems(Items items) //Загрузка предметов в ListBox
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();
            ViewItems viewItems = new ViewItems();
            items_source = viewItems.GetSaveItemsSource(items, items_source, GameEnvironment.Computers.PlayerComputers);

            СomponentsList.ItemsSource = items_source;
        }

        public void LoadComputers(Computers computers)
        {
            foreach (Computer computer in computers.PlayerComputers) { AssemblyList.Items.Add(computer.Name); }
        }

        private void AddAssemblyName(string name, bool isCleaningItemsSource = true)
        {
            foreach (String item in AssemblyList.Items)
            { if (item == name) { name = string.Empty; GameMessageBox.Show(Properties.Resources.AddAssembly, Properties.Resources.AssemblyMessage1, GameMessageBox.MessageBoxType.Warning); break; } }

            if (!string.IsNullOrEmpty(name))
            { AssemblyList.Items.Add(name); if (isCleaningItemsSource) { ComputerСomponents.ItemsSource = new Collection<ListBoxObject>(); } }
        }
        private void AddAssembly_Click(object sender, RoutedEventArgs e) => AddAssemblyName(AssemblyList.Text);

        private void DefaultBuild_Click(object sender, RoutedEventArgs e)
        {
            List<Computer> currentComputer = GameEnvironment.Computers.PlayerComputers.Where(n => n.Name == AssemblyList.Text).ToList();
            if (currentComputer.Count == 1)
            {
                GameEnvironment.Computers.CurrentPlayerComputer = currentComputer[0];
                GameMessageBox.Show("Сборка установлена как сборка по умолчанию!");
            } else {
                GameMessageBox.Show("Компьютер с таким именем не существует!");
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

            bool isValid = Operations[@operator](x, y);
            if (!isValid) { ProblemReport(problem_report); } return isValid;
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

            foreach (object obj in (collection.Where(i => i.Item.GetTypeValue() == "ram"))) { volume += ((obj as ListBoxObject).IObject as RAM).Properties.Volume; }
            if (ram.Properties.Volume + volume <= (GetSingleItem(collection, "motherboard") as Motherboard).Properties.RAMVolume) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsFreeSpaceInstallation(Collection<ListBoxObject> collection, HDD hdd, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 0 || GetCount(collection, hdd.GetTypeValue()) < ((GetSingleItem(collection, "case") as Case).GetCountCompatiblePlaces(hdd.Properties.FormFactor))) { isValid = true; } else { ProblemReport(problem_report); }
            return isValid;
        }
        private bool IsFreeSpaceInstallation(Collection<ListBoxObject> collection, OpticalDrive opticalDrive, string problem_report)
        {
            bool isValid = false;
            if (GetCount(collection, "case") == 1) { isValid = GetCount(collection, "optical_drive") < 2 ? true : false; } else { isValid = true; } //2 статическое значение слотов под приводы в систмном блоке, потом может преместиться в параметр
            if (!isValid) { ProblemReport(problem_report); } return isValid;
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
                if (count_interfaces < mouse.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties, (GetSingleItem(collection, "case") as Case).Properties)) { isValid = true; }
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
                if (count_interfaces < keyboard.Compatibility((GetSingleItem(collection, "motherboard") as Motherboard).Properties, (GetSingleItem(collection, "case") as Case).Properties)) { isValid = true; }
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

                if (button.Tag is Case)
                {
                    Case @case = (button.Tag as Case);
                    if (IsEquality(GetCount(items, @case.GetTypeValue()), 0, Operators.Equally, "У вас уже есть корпус в этой конфигурации!") &&
                        IsNullOrCompatibleMotherboard(items, @case, "Материнская плата не станет в этот корпус!") &&
                        IsСapacityHDD(items, @case, "Все диски сюда не влезут!") &&
                        IsNullOrCompatiblePSU(items, @case, "Блок питания не станет в этот корпус!"))
                    { InstallСomponent<Case>(items, @case, button); }
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
        
        private bool IsChangedComputer(Computer computer)
        {
            bool isChangedComputer = false;
            foreach (object obj in ComputerСomponents.Items)
            {
                object iobj = (obj as ListBoxObject).IObject;
                switch (iobj)
                {
                    case Case _:
                        isChangedComputer = computer.Case == (iobj as Case) ? false : true;
                        break;
                    case Motherboard _:
                        isChangedComputer = computer.Motherboard == (iobj as Motherboard) ? false : true;
                        break;
                    case CPU _:
                        isChangedComputer = computer.CPU == (iobj as CPU) ? false : true;
                        break;
                    case PowerSupplyUnit _:
                        isChangedComputer = computer.PSU == (iobj as PowerSupplyUnit) ? false : true;
                        break;
                    case RAM _:
                        isChangedComputer = true;
                        foreach (RAM ram in computer.RAMs) { isChangedComputer = ram == (iobj as RAM) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                    case HDD _:
                        isChangedComputer = true;
                        foreach (HDD hdd in computer.HDDs) { isChangedComputer = hdd == (iobj as HDD) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                    case VideoСard _:
                        isChangedComputer = true;
                        foreach (VideoСard videoСard in computer.VideoСards) { isChangedComputer = videoСard == (iobj as VideoСard) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                    case OpticalDrive _:
                        isChangedComputer = true;
                        foreach (OpticalDrive opticalDrive in computer.OpticalDrives) { isChangedComputer = opticalDrive == (iobj as OpticalDrive) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                    case Monitor _:
                        isChangedComputer = true;
                        foreach (Monitor monitor in computer.Monitors) { isChangedComputer = monitor == (iobj as Monitor) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                    case Mouse _:
                        isChangedComputer = true;
                        foreach (Mouse mouse in computer.Mice) { isChangedComputer = mouse == (iobj as Mouse) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                    case Keyboard _:
                        isChangedComputer = true;
                        foreach (Keyboard keyboard in computer.Keyboards) { isChangedComputer = keyboard == (iobj as Keyboard) ? false : true; if (!isChangedComputer) { break; } }
                        break;
                }

                if (isChangedComputer) { break; }
            }
            return isChangedComputer;
        }

        private Computer AddСomponents(Computer computer)
        {
            foreach (object obj in ComputerСomponents.Items)
            {
                object iobj = (obj as ListBoxObject).IObject;
                switch (iobj)
                {
                    case Case _:
                        computer.Case = iobj as Case;
                        break;
                    case Motherboard _:
                        computer.Motherboard = iobj as Motherboard;
                        break;
                    case CPU _:
                        computer.CPU = iobj as CPU;
                        break;
                    case CPUCooler _:
                        computer.CPUCooler = iobj as CPUCooler;
                        break;
                    case PowerSupplyUnit _:
                        computer.PSU = iobj as PowerSupplyUnit;
                        break;
                    case RAM _:
                        bool isNewRAM = true;
                        foreach (RAM ram in computer.RAMs) { if (iobj as RAM == ram) { isNewRAM = false; } }
                        if (isNewRAM) { computer.RAMs.Add(iobj as RAM); }
                        break;
                    case HDD _:
                        bool isNewHDD = true;
                        foreach (HDD hdd in computer.HDDs) { if (iobj as HDD == hdd) { isNewHDD = false; } }
                        if (isNewHDD) { computer.HDDs.Add(iobj as HDD); }
                        break;
                    case VideoСard _:
                        bool isNewVideoCard = true;
                        foreach (VideoСard videoСard in computer.VideoСards) { if (iobj as VideoСard == videoСard) { isNewVideoCard = false; } }
                        if (isNewVideoCard) { computer.VideoСards.Add(iobj as VideoСard); }
                        break;
                    case OpticalDrive _:
                        bool isNewOpticalDrive = true;
                        foreach (OpticalDrive opticalDrive in computer.OpticalDrives) { if (iobj as OpticalDrive == opticalDrive) { isNewOpticalDrive = false; } }
                        if (isNewOpticalDrive) { computer.OpticalDrives.Add(iobj as OpticalDrive); }
                        break;
                    case Monitor _:
                        bool isNewMonitor = true;
                        foreach (Monitor monitor in computer.Monitors) { if (iobj as Monitor == monitor) { isNewMonitor = false; } }
                        if (isNewMonitor) { computer.Monitors.Add(iobj as Monitor); }
                        break;
                    case Mouse _:
                        bool isNewMouse = true;
                        foreach (Mouse mouse in computer.Mice) { if (iobj as Mouse == mouse) { isNewMouse = false; } }
                        if (isNewMouse) { computer.Mice.Add(iobj as Mouse); }
                        break;
                    case Keyboard _:
                        bool isNewKeyboard = true;
                        foreach (Keyboard keyboard in computer.Keyboards) { if (iobj as Keyboard == keyboard) { isNewKeyboard = false; } }
                        if (isNewKeyboard) { computer.Keyboards.Add(iobj as Keyboard); }
                        break;
                }
            }
            return computer;
        }

        /// <param name="auxiliary_check">Вспомогательная проверка нужна при вызове метода из AssemblyList_SelectionChanged</param>
        private void SaveComputer(bool auxiliary_check = true)
        {
            string name = AssemblyList.Text;
            bool isName = false;
            if (!string.IsNullOrEmpty(name)) { foreach (String item in AssemblyList.Items) { if (item == name) { isName = true; break; } } }

            Computer[] currentComputer = GameEnvironment.Computers.PlayerComputers.Where(n => n.Name == name).ToArray();
            bool isNewComputer = currentComputer.Count() == 1 ? false : true;
            bool isChangedComputer = false;

            if (isNewComputer == false) { isChangedComputer = IsChangedComputer(currentComputer[0]); }
            if (!auxiliary_check) { return; }
            if (!(ComputerСomponents.Items.Count >= 1 && isNewComputer || !isNewComputer && isChangedComputer)) { return; }
            if (!isName) { AddAssemblyName(name, false); }

            Computer newComputer;
            if (!isChangedComputer)
            {
                Case @case = null;
                Motherboard motherboard = null;
                foreach (object obj in ComputerСomponents.Items)
                {
                    object iobj = (obj as ListBoxObject).IObject;
                    if (iobj is Case) { @case = iobj as Case; } else if (iobj is Motherboard) { motherboard = iobj as Motherboard; }
                    if (@case != null && motherboard != null) { break; }
                }

                if (@case == null || motherboard == null)
                {
                    switch (@case)
                    {
                        case null:
                            if (motherboard != null)
                            {
                                newComputer = new Computer(name, motherboard);
                            }
                            else { newComputer = null; }
                            break;
                        default:
                            newComputer = new Computer(name, @case);
                            break;
                    }
                }
                else { newComputer = new Computer(name, @case, motherboard); }
            }
            else { newComputer = currentComputer[0]; currentComputer[0].IsEnable = false; }

            if (newComputer == null) { return; }

            newComputer = AddСomponents(newComputer);
            if (isChangedComputer)
            { _ = GameMessageBox.Show(Properties.Resources.ModifiedAssembly, Properties.Resources.AssemblingChanged, GameMessageBox.MessageBoxType.Information); }
            else
            {
                GameEnvironment.Computers.PlayerComputers.Add(newComputer);
                _ = GameMessageBox.Show(Properties.Resources.AddAssembly, Properties.Resources.AssemblingAdded, GameMessageBox.MessageBoxType.Information);
            }
        }

        private void SaveAssembly_Click(object sender, RoutedEventArgs e)
        {
            string name = AssemblyList.Text;

            Computer[] currentComputer = GameEnvironment.Computers.PlayerComputers.Where(n => n.Name == name).ToArray();
            bool isNewComputer = currentComputer.Count() == 1 ? false : true;
            bool isChangedComputer = false;

            if (isNewComputer == false) { isChangedComputer = IsChangedComputer(currentComputer[0]); }
            SaveComputer();
        }

        string oldName = string.Empty; //Пердыдущее выбранное имя, нужно что бы не срабатывал код при вводе текста в поле, так как в код отдаеться еще не измененный текст
        private void AssemblyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = AssemblyList.Text;
            string selectedName = string.Empty;
            if (AssemblyList.SelectedValue != null) { selectedName = AssemblyList.SelectedValue.ToString(); }
            bool isOldName = false;
            if (!string.IsNullOrEmpty(oldName)) { foreach (string item in AssemblyList.Items) { if (item == oldName) { isOldName = true; break; } } }

            Computer[] currentComputer = GameEnvironment.Computers.PlayerComputers.Where(n => n.Name == name).ToArray();
            bool isNewComputer = currentComputer.Count() == 1 ? false : true;
            bool isChangedComputer = false;

            if (isNewComputer == false) { isChangedComputer = IsChangedComputer(currentComputer[0]); }
            SaveComputer(isOldName);
            ComputerСomponents.ItemsSource = null;
            ComputerСomponents.Items.Clear();

            List<Computer> computer = GameEnvironment.Computers.PlayerComputers.Where(n => n.Name == selectedName).ToList();

            if (computer.Count == 1)
            {
                Collection<ListBoxObject> items = new Collection<ListBoxObject>();
                if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].Case)); }
                if (computer[0].Motherboard != null) { items.Add(new ListBoxObject(computer[0].Motherboard)); }
                if (computer[0].PSU != null) { items.Add(new ListBoxObject(computer[0].PSU)); }
                if (computer[0].CPU != null) { items.Add(new ListBoxObject(computer[0].CPU)); }
                if (computer[0].CPUCooler != null) { items.Add(new ListBoxObject(computer[0].CPUCooler)); }
                if (computer[0].RAMs != null) { foreach (RAM ram in computer[0].RAMs) { items.Add(new ListBoxObject(ram)); } }
                if (computer[0].HDDs != null) { foreach (HDD hdd in computer[0].HDDs) { items.Add(new ListBoxObject(hdd)); } }
                if (computer[0].VideoСards != null) { foreach (VideoСard videoСard in computer[0].VideoСards) { items.Add(new ListBoxObject(videoСard)); } }
                if (computer[0].Monitors != null) { foreach (Monitor monitor in computer[0].Monitors) { items.Add(new ListBoxObject(monitor)); } }
                if (computer[0].OpticalDrives != null) { foreach (OpticalDrive opticalDrive in computer[0].OpticalDrives) { items.Add(new ListBoxObject(opticalDrive)); } }
                if (computer[0].Mice != null) { foreach (Mouse mouse in computer[0].Mice) { items.Add(new ListBoxObject(mouse)); } }
                if (computer[0].Keyboards != null) { foreach (Keyboard keyboard in computer[0].Keyboards) { items.Add(new ListBoxObject(keyboard)); } }

                ComputerСomponents.ItemsSource = items;
            }

            ComputerСomponents.Items.Refresh();
            oldName = name;
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
