using Computer_Era.Game;
using Computer_Era.Game.Objects;
using Newtonsoft.Json;
using System;
using System.Data.SQLite;
using System.Windows;

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
            }
            catch
            {
                MessageBox.Show("Запись не была добавлена в базу, вероятно не заполнено одно или несколько полей, либо заполнены неверно.");
            }
        }
    }
}
