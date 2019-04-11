using Computer_Era.Game;
using Computer_Era.Game.Objects;
using Newtonsoft.Json;
using System;
using System.Data.SQLite;
using System.Windows;
using Size = Computer_Era.Game.Objects.Size;

namespace Computer_Era_Tools
{
    public partial class MainWindow : Window
    {
        SQLiteConnection connection;
        Items items;
        public MainWindow()
        {
            InitializeComponent();
            DataBase dataBase = new DataBase("ComputerEra.db3");
            connection = dataBase.ConnectDB();
            items = new Items(connection, 1);

            Cases.ItemsSource = items.AllCases;
            CaseType.ItemsSource = Enum.GetValues(typeof(CaseTypes));
            CaseFormFactorMotherboard.ItemsSource = Enum.GetValues(typeof(MotherboardTypes));
            CaseFormFactorPSU.ItemsSource = Enum.GetValues(typeof(PSUTypes));

            Motherboards.ItemsSource = items.AllMotherboards;
            MotherboardType.ItemsSource = Enum.GetValues(typeof(MotherboardTypes));
            MotherboardCPUSocket.ItemsSource = Enum.GetValues(typeof(Sockets));
            MotherboardBIOS.ItemsSource = Enum.GetValues(typeof(MotherboardBIOS));
            MotherboardRamType.ItemsSource = Enum.GetValues(typeof(RAMTypes));
            MotherboardVideoInterfaces.ItemsSource = Enum.GetValues(typeof(VideoInterface));

            PSUs.ItemsSource = items.AllPowerSupplyUnits;
            PSUType.ItemsSource = Enum.GetValues(typeof(PSUTypes));
            PSUTypeCM.ItemsSource = Enum.GetValues(typeof(TypeConnectorMotherboard));

            CPUs.ItemsSource = items.AllCPUs;
            CPUSocket.ItemsSource = Enum.GetValues(typeof(Sockets));

            RAMs.ItemsSource = items.AllRAMs;
            RAMType.ItemsSource = Enum.GetValues(typeof(RAMTypes));

            CPUCoolers.ItemsSource = items.AllCPUCoolers;
            CPUCoolerSockets.ItemsSource = Enum.GetValues(typeof(Sockets));

            HDDs.ItemsSource = items.AllHDDs;
            HDDFormFactor.ItemsSource = Enum.GetValues(typeof(HDDFormFactor));
            HDDInterface.ItemsSource = Enum.GetValues(typeof(HDDInterface));

            VideoCards.ItemsSource = items.AllVideoCards;
            VideoCardInterface.ItemsSource = Enum.GetValues(typeof(Interface));
            VideoCardTypeVideoMemory.ItemsSource = Enum.GetValues(typeof(TypeVideoMemory));
            VideoCardVideoInterfaces.ItemsSource = Enum.GetValues(typeof(VideoInterface));

            Monitors.ItemsSource = items.AllMonitors;
            MonitorVideoInterfaces.ItemsSource = Enum.GetValues(typeof(VideoInterface));

            OpticalDrives.ItemsSource = items.AllOpticalDrives;
            OpticalDriveInterface.ItemsSource = Enum.GetValues(typeof(OpticalDriveInterface));
        }

        private int InsertToDB<T>(Item<T> item)
        {
            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO items (name, type, price, manufacturing_date, properties) VALUES ($name, $type, $price, $manufacturing_date, $properties)", connection);
            cmd.Parameters.Add("$name", System.Data.DbType.String).Value = item.Name;
            cmd.Parameters.Add("$type", System.Data.DbType.String).Value = item.GetTypeValue();
            cmd.Parameters.Add("$price", System.Data.DbType.Int32).Value = item.Price;
            cmd.Parameters.Add("$manufacturing_date", System.Data.DbType.Date).Value = item.ManufacturingDate;
            cmd.Parameters.Add("$properties", System.Data.DbType.String).Value = JsonConvert.SerializeObject(item.Properties);
            cmd.ExecuteNonQuery();

            //Получаем ID и добавляем запись в локальный спаравочник
            cmd = new SQLiteCommand("SELECT last_insert_rowid()", connection);
            object id = cmd.ExecuteScalar();
            cmd.CommandText = "SELECT id FROM items WHERE rowid=" + id.ToString();
            id = cmd.ExecuteScalar();
            return Convert.ToInt32(id);
        }
        private void SaveCase_Click(object sender, RoutedEventArgs e)
        {
           try
           {
                CaseProperties caseProperties = new CaseProperties
                {
                    CaseType = (CaseTypes)Enum.Parse(typeof(CaseTypes), CaseType.SelectedItem.ToString()),
                    CoolerHeight = Convert.ToInt32(CaseCoolerHeight.Text),
                    VideocardLength = Convert.ToInt32(CaseVideocardLength.Text),
                    Sections3_5 = Convert.ToInt32(CaseSections3_5.Text),
                    Sections2_5 = Convert.ToInt32(CaseSections2_5.Text),
                    ExpansionSlots = Convert.ToInt32(CaseExpansionSlots.Text),
                    BuiltinFans = Convert.ToInt32(CaseBuiltinFans.Text),
                    PlacesFans = Convert.ToInt32(CasePlacesFans.Text),
                    LiquidCooling = CaseLiquidCooling.IsChecked.Value,
                    USB2_0 = Convert.ToInt32(CaseUSB2_0.Text),
                    USB3_0 = Convert.ToInt32(CaseUSB3_0.Text),
                    HeadphoneJack = CaseHeadphoneJack.IsChecked.Value,
                    MicrophoneJack = CaseMicrophoneJack.IsChecked.Value,
                };

                for (int i = 0; CaseFormFactorMotherboard.SelectedItems.Count > i; i++)
                {
                    caseProperties.FormFactor.Add((MotherboardTypes)Enum.Parse(typeof(MotherboardTypes), CaseFormFactorMotherboard.SelectedItems[i].ToString()));
                }

                for (int i = 0; CaseFormFactorPSU.SelectedItems.Count > i; i++)
                {
                    caseProperties.FormFactorPSU.Add((PSUTypes)Enum.Parse(typeof(PSUTypes), CaseFormFactorPSU.SelectedItems[i].ToString()));
                }
                Case @case = new Case(0, CaseName.Text, "case", Convert.ToInt32(CasePrice.Text), CaseManufacturingDate.SelectedDate.Value, caseProperties);

                int id = InsertToDB(@case);
                @case.Uid = Convert.ToInt32(id);
                items.AllCases.Add(@case);
                Cases.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
           }
        }

        private void SaveMotherboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MotherboardProperties motherboardProperties = new MotherboardProperties()
                {
                    MotherboardType = (MotherboardTypes)Enum.Parse(typeof(MotherboardTypes), MotherboardType.SelectedItem.ToString()),
                    Socket = (Sockets)Enum.Parse(typeof(Sockets), MotherboardCPUSocket.SelectedItem.ToString()),
                    MultiCoreProcessor = MotherboardMultiCoreProcessor.IsChecked.Value,
                    Chipset = MotherboardChipset.Text,
                    BIOS = (MotherboardBIOS)Enum.Parse(typeof(MotherboardBIOS), MotherboardBIOS.SelectedItem.ToString()),
                    EFI = MotherboardEFI.IsChecked.Value,
                    RamType = (RAMTypes)Enum.Parse(typeof(RAMTypes), MotherboardRamType.SelectedItem.ToString()),
                    RAMSlots = Convert.ToInt32(MotherboardRAMSlots.Text),
                    MinFrequency = Convert.ToInt32(MotherboardRamMinFrequency.Text),
                    MaxFrequency = Convert.ToInt32(MotherboardRamMaxFrequency.Text),
                    RAMVolume = Convert.ToInt32(MotherboardRAMVolume.Text),
                    IDE = Convert.ToInt32(MotherboardIDE.Text),
                    PCI = Convert.ToInt32(MotherboardPCI.Text),
                    PCI_Ex1 = Convert.ToInt32(MotherboardPCI_Ex1.Text),
                    PCI_Ex4 = Convert.ToInt32(MotherboardPCI_Ex4.Text),
                    PCI_Ex8 = Convert.ToInt32(MotherboardPCI_Ex8.Text),
                    PCI_Ex16 = Convert.ToInt32(MotherboardPCI_Ex16.Text),
                    PCIE2_0 = MotherboardPCIE2_0.IsChecked.Value,
                    PCIE3_0 = MotherboardPCIE3_0.IsChecked.Value,
                    EmbeddedGraphics = MotherboardEmbeddedGraphics.IsChecked.Value,
                    Sound = MotherboardSound.IsChecked.Value,
                    EthernetSpeed = Convert.ToInt32(MotherboardEthernetSpeed.Text),
                    PS2Keyboard = MotherboardPS2Keyboard.IsChecked.Value,
                    PS2Mouse = MotherboardPS2Mouse.IsChecked.Value,
                    USB2_0 = Convert.ToInt32(MotherboardUSB2_0.Text),
                    USB3_0 = Convert.ToInt32(MotherboardUSB3_0.Text),
                };

                for (int i = 0; CaseFormFactorMotherboard.SelectedItems.Count > i; i++)
                {
                    motherboardProperties.VideoInterfaces.Add((VideoInterface)Enum.Parse(typeof(VideoInterface), MotherboardVideoInterfaces.SelectedItems[i].ToString()));
                }

                Motherboard motherboard = new Motherboard(0, MotherboardName.Text, "motherboard", Convert.ToInt32(MotherboardPrice.Text), MotherboardManufacturingDate.SelectedDate.Value, motherboardProperties);
                int id = InsertToDB(motherboard);
                motherboard.Uid = id;
                items.AllMotherboards.Add(motherboard);
                Motherboards.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch { 
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SavePSU_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PowerSupplyUnitProperties psuProperties = new PowerSupplyUnitProperties()
                {
                    PSUType = (PSUTypes)Enum.Parse(typeof(PSUTypes), PSUType.SelectedItem.ToString()),
                    TypeCM = (TypeConnectorMotherboard)Enum.Parse(typeof(TypeConnectorMotherboard), PSUTypeCM.SelectedItem.ToString()),
                    Pin4plus4CPU = Convert.ToInt32(PSUPin4plus4CPU.Text),
                    Pin6plus2PCIE = Convert.ToInt32(PSUPin6plus2PCIE.Text),
                    Pin15SATA = Convert.ToInt32(PSUPin15SATA.Text),
                    Pin4IDE = Convert.ToInt32(PSUPin4IDE.Text),
                    MinNoiseLevel = Convert.ToInt32(PSUMinNoiseLevel.Text),
                    MaxNoiseLevel = Convert.ToInt32(PSUMaxNoiseLevel.Text),
                    OvervoltageProtection = PSUOvervoltageProtection.IsChecked.Value,
                    OverloadProtection = PSUOverloadProtection.IsChecked.Value,
                    ShortCircuitProtection = PSUShortCircuitProtection.IsChecked.Value,
                };
                PowerSupplyUnit psu = new PowerSupplyUnit(0, PSUName.Text, "psu", Convert.ToInt32(PSUPrice.Text), PSUManufacturingDate.SelectedDate.Value, psuProperties);

                int id = InsertToDB(psu);
                psu.Uid = id;
                items.AllPowerSupplyUnits.Add(psu);
                PSUs.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SaveCPU_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CPUProperties cpuProperties = new CPUProperties()
                {
                    Socket = (Sockets)Enum.Parse(typeof(Sockets), CPUSocket.SelectedItem.ToString()),
                    NumberCores = Convert.ToInt32(CPUNumberCores.Text),
                    MinCPUFrequency = Convert.ToInt32(CPUMinCPUFrequency.Text),
                    MaxCPUFrequency = Convert.ToInt32(CPUMaxCPUFrequency.Text),
                    MinHeatDissipation = Convert.ToInt32(CPUMinHeatDissipation.Text),
                    MaxHeatDissipation = Convert.ToInt32(CPUMaxHeatDissipation.Text),
                    MaximumTemperature = Convert.ToInt32(CPUMaximumTemperature.Text),
                };
                CPU cpu = new CPU(0, CPUName.Text, "cpu", Convert.ToInt32(CPUPrice.Text), CPUManufacturingDate.SelectedDate.Value, cpuProperties);

                int id = InsertToDB(cpu);
                cpu.Uid = id;
                items.AllCPUs.Add(cpu);
                CPUs.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SaveRAM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RAMProperties ramProperties = new RAMProperties()
                {
                    RAMType = (RAMTypes)Enum.Parse(typeof(RAMTypes), RAMType.SelectedItem.ToString()),
                    ClockFrequency = Convert.ToInt32(RAMClockFrequency.Text),
                    Volume = Convert.ToInt32(RAMVolume.Text),
                    SupplyVoltage = Convert.ToInt32(RAMSupplyVoltage.Text),
                };
                RAM ram = new RAM(0, RAMName.Text, "ram", Convert.ToInt32(RAMPrice.Text), RAMManufacturingDate.SelectedDate.Value, ramProperties);

                int id = InsertToDB(ram);
                ram.Uid = id;
                items.AllRAMs.Add(ram);
                RAMs.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }
        private void SaveCPUCooler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CPUCoolerProperties cpuCoolerProperties = new CPUCoolerProperties()
                {
                    MinRotationalSpeed = Convert.ToInt32(CPUCoolerMinRotationalSpeed.Text),
                    MaxRotationalSpeed = Convert.ToInt32(CPUCoolerMaxRotationalSpeed.Text),
                    AirFlow = Convert.ToInt32(CPUCoolerAirFlow.Text),
                    MinNoiseLevel = Convert.ToInt32(CPUCoolerMinNoiseLevel.Text),
                    MaxNoiseLevel = Convert.ToInt32(CPUCoolerMaxNoiseLevel.Text),
                    SpeedController = CPUCoolerSpeedController.IsChecked.Value,
                    Size = new Size(Convert.ToInt32(CPUCoolerWidth.Text),
                                    Convert.ToInt32(CPUCoolerHeight.Text),
                                    Convert.ToInt32(CPUCoolerDepth.Text)),
                };
                for (int i = 0; CPUCoolerSockets.SelectedItems.Count > i; i++)
                {
                    cpuCoolerProperties.Sockets.Add((Sockets)Enum.Parse(typeof(Sockets), CPUCoolerSockets.SelectedItems[i].ToString()));
                }
                CPUCooler cpuCooler = new CPUCooler(0, CPUCoolerName.Text, "cpu_cooler", Convert.ToInt32(CPUCoolerPrice.Text), CPUCoolerManufacturingDate.SelectedDate.Value, cpuCoolerProperties);

                int id = InsertToDB(cpuCooler);
                cpuCooler.Uid = id;
                items.AllCPUCoolers.Add(cpuCooler);
                CPUCoolers.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SaveHDD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HDDProperties hddProperties = new HDDProperties()
                {
                    FormFactor = (HDDFormFactor)Enum.Parse(typeof(HDDFormFactor), HDDFormFactor.SelectedItem.ToString()),
                    Volume = Convert.ToInt32(HDDVolume.Text),
                    WriteSpeed = Convert.ToInt32(HDDWriteSpeed.Text),
                    ReadSpeed = Convert.ToInt32(HDDReadSpeed.Text),
                    BufferCapacity = Convert.ToInt32(HDDBufferCapacity.Text),
                    Interface = (HDDInterface)Enum.Parse(typeof(HDDInterface), HDDInterface.SelectedItem.ToString()),
                    MaximumTemperature = Convert.ToInt32(HDDMaximumTemperature.Text),
                };
                HDD hdd = new HDD(0, HDDName.Text, "hdd", Convert.ToInt32(HDDPrice.Text), HDDManufacturingDate.SelectedDate.Value, hddProperties);

                int id = InsertToDB(hdd);
                hdd.Uid = id;
                items.AllHDDs.Add(hdd);
                HDDs.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SaveMonitor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MonitorProperties monitorProperties = new MonitorProperties()
                {
                    Size = Convert.ToInt32(MonitorSize.Text),
                    Resolution = new Resolution(Convert.ToInt32(MonitorResolutionWidth.Text), Convert.ToInt32(MonitorResolutionHeight.Text)),
                    MaxFrameRefreshRate = Convert.ToInt32(MonitorMaxFrameRefreshRate.Text),
                };
                for (int i = 0; MonitorVideoInterfaces.SelectedItems.Count > i; i++)
                {
                    monitorProperties.VideoInterfaces.Add((VideoInterface)Enum.Parse(typeof(VideoInterface), MonitorVideoInterfaces.SelectedItems[i].ToString()));
                }
                Monitor monitor = new Monitor(0, MonitorName.Text, "monitor", Convert.ToInt32(MonitorPrice.Text), MonitorManufacturingDate.SelectedDate.Value, monitorProperties);

                int id = InsertToDB(monitor);
                monitor.Uid = id;
                items.AllMonitors.Add(monitor);
                Monitors.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SaveOpticalDrive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpticalDriveProperties opticalDriveProperties = new OpticalDriveProperties()
                {
                    Interface = (OpticalDriveInterface)Enum.Parse(typeof(OpticalDriveInterface), OpticalDriveInterface.SelectedItem.ToString()),
                    Size = new Size(Convert.ToInt32(OpticalDriveWidth.Text), Convert.ToInt32(OpticalDriveHeight.Text), Convert.ToInt32(OpticalDriveDepth.Text)),
                    MaxWritingSpeed = new int[] { Convert.ToInt32(OpticalDriveMaxWritingSpeedCD_R.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedCD_RW.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVD_R.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVDplusR_DL.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVD_RW.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVDplusR.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVDplusR_DL.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVDplusRW.Text),
                                              Convert.ToInt32(OpticalDriveMaxWritingSpeedDVD_RAM.Text) },
                    MaxReadSpeedCD = Convert.ToInt32(OpticalDriveMaxReadSpeedCD.Text),
                    MaxReadSpeedDVD = Convert.ToInt32(OpticalDriveMaxReadSpeedDVD.Text),
                    ReadAccessTimeCD = Convert.ToInt32(OpticalDriveReadAccessTimeCD.Text),
                    ReadAccessTimeDVD = Convert.ToInt32(OpticalDriveReadAccessTimeDVD.Text),
                };
                OpticalDrive opticalDrive = new OpticalDrive(0, OpticalDriveName.Text, "optical_drive", Convert.ToInt32(OpticalDrivePrice.Text), OpticalDriveManufacturingDate.SelectedDate.Value, opticalDriveProperties);

                int id = InsertToDB(opticalDrive);
                opticalDrive.Uid = id;
                items.AllOpticalDrives.Add(opticalDrive);
                OpticalDrives.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            } catch {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }

        private void SaveVideoСard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VideoCardProperties videoCardProperties = new VideoCardProperties()
                {
                    GraphicsProcessor = VideoCardGraphicsProcessor.Text,
                    Interface = (Interface)Enum.Parse(typeof(Interface), VideoCardInterface.SelectedItem.ToString()),
                    MaxResolution = new Resolution(Convert.ToInt32(VideoCardMaxResolutionWidth.Text), Convert.ToInt32(VideoCardMaxResolutionHeight.Text)),
                    GPUFrequency = Convert.ToInt32(VideoCardGPUFrequency.Text),
                    VideoMemory = Convert.ToInt32(VideoCardVideoMemory.Text),
                    TypeVideoMemory = (TypeVideoMemory)Enum.Parse(typeof(TypeVideoMemory), VideoCardTypeVideoMemory.SelectedItem.ToString()),
                    VideoMemoryFrequency = Convert.ToInt32(VideoCardVideoMemoryFrequency.Text),
                };
                for (int i = 0; VideoCardVideoInterfaces.SelectedItems.Count > i; i++)
                {
                    videoCardProperties.VideoInterfaces.Add((VideoInterface)Enum.Parse(typeof(VideoInterface), VideoCardVideoInterfaces.SelectedItems[i].ToString()));
                }
                VideoCard videoCard = new VideoCard(0, VideoСardName.Text, "video_card", Convert.ToInt32(VideoCardPrice.Text), VideoCardManufacturingDate.SelectedDate.Value, videoCardProperties);

                int id = InsertToDB(videoCard);
                videoCard.Uid = id;
                items.AllVideoCards.Add(videoCard);
                Monitors.Items.Refresh();
                MessageBox.Show("Запись добавлена!");
            }
            catch
            {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }
    }
}
