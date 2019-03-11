﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Computer_Era.Game.Objects
{
    public enum ItemTypes
    {
        @case,
        motherboard,
        ram,
        psu,
        cpu,
        cpu_cooler,
        hdd,
        monitor,
        video_card,
        optical_drive,
        mouse,
        keyboard,
        optical_disc,
        os,
        program,
    }
    public class Items
    {
        SQLiteConnection Connection;

        public Collection<Case> AllCases = new Collection<Case>();
        public Collection<Motherboard> AllMotherboards = new Collection<Motherboard>();
        public Collection<RAM> AllRAMs = new Collection<RAM>();
        public Collection<PowerSupplyUnit> AllPowerSupplyUnits = new Collection<PowerSupplyUnit>();
        public Collection<CPU> AllCPUs = new Collection<CPU>();
        public Collection<CPUCooler> AllCPUCoolers = new Collection<CPUCooler>();
        public Collection<HDD> AllHDDs = new Collection<HDD>();
        public Collection<Monitor> AllMonitors = new Collection<Monitor>();
        public Collection<VideoСard> AllVideoСards = new Collection<VideoСard>();
        public Collection<OpticalDrive> AllOpticalDrives = new Collection<OpticalDrive>();
        public Collection<Mouse> AllMice = new Collection<Mouse>();
        public Collection<Keyboard> AllKeyboards = new Collection<Keyboard>();
        public Collection<OpticalDisc> AllOpticalDiscs = new Collection<OpticalDisc>();
        public Collection<OperatingSystem> AllOperatingSystems = new Collection<OperatingSystem>();
        public Collection<Program> AllPrograms = new Collection<Program>();

        public Collection<Case> Cases = new Collection<Case>();
        public Collection<Motherboard> Motherboards = new Collection<Motherboard>();
        public Collection<RAM> RAMs = new Collection<RAM>();
        public Collection<PowerSupplyUnit> PowerSupplyUnits = new Collection<PowerSupplyUnit>();
        public Collection<CPU> CPUs = new Collection<CPU>();
        public Collection<CPUCooler> CPUCoolers = new Collection<CPUCooler>();
        public Collection<HDD> HDDs = new Collection<HDD>();
        public Collection<Monitor> Monitors = new Collection<Monitor>();
        public Collection<VideoСard> VideoСards = new Collection<VideoСard>();
        public Collection<OpticalDrive> OpticalDrives = new Collection<OpticalDrive>();
        public Collection<Mouse> Mice = new Collection<Mouse>();
        public Collection<Keyboard> Keyboards = new Collection<Keyboard>();
        public Collection<OpticalDisc> OpticalDiscs = new Collection<OpticalDisc>();
        public Collection<OperatingSystem> OperatingSystems = new Collection<OperatingSystem>();
        public Collection<Program> Programs = new Collection<Program>();

        public Items(SQLiteConnection connection, int save_id)
        {
            Connection = connection;

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM sv_inventorys WHERE save_id=" + save_id + ";";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string name = Convert.ToString(data_reader["name"]);
                    string type = Convert.ToString(data_reader["type"]);
                    int price = Convert.ToInt32(data_reader["price"]);
                    DateTime manufacturing_date = Convert.ToDateTime(data_reader["manufacturing_date"]);

                    string json = Convert.ToString(data_reader["properties"]);
                    AddItemsToSaveCollection(id, name, type, price, manufacturing_date, json,
                                             Cases, Motherboards, RAMs, PowerSupplyUnits, CPUs, CPUCoolers, HDDs, Monitors, VideoСards, OpticalDrives, Mice, Keyboards, OpticalDiscs, OperatingSystems, Programs);
                }
            }

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * FROM items;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string name = Convert.ToString(data_reader["name"]);
                    string type = Convert.ToString(data_reader["type"]);
                    int price = Convert.ToInt32(data_reader["price"]);
                    DateTime manufacturing_date = Convert.ToDateTime(data_reader["manufacturing_date"]);

                    string json = Convert.ToString(data_reader["properties"]);
                    AddItemsToSaveCollection(id, name, type, price, manufacturing_date, json,
                                             AllCases, AllMotherboards, AllRAMs, AllPowerSupplyUnits, AllCPUs, AllCPUCoolers, AllHDDs, AllMonitors, AllVideoСards, AllOpticalDrives, AllMice,
                                             AllKeyboards, AllOpticalDiscs, AllOperatingSystems, AllPrograms);
                }
            }
        }

        private void AddItemsToSaveCollection(int id, string name, string type, int price, DateTime manufacturing_date, string json,
                                              Collection<Case> cases, Collection<Motherboard> motherboards, Collection<RAM> rams, Collection<PowerSupplyUnit> powerSupplyUnits,
                                              Collection<CPU> cpus, Collection<CPUCooler> cpu_coolers, Collection<HDD> hdds, Collection<Monitor> monitors, Collection<VideoСard> videoСards,
                                              Collection<OpticalDrive> opticalDrives, Collection<Mouse> mice, Collection<Keyboard> keyboards, Collection<OpticalDisc> discs,
                                              Collection<OperatingSystem> operatingSystems, Collection<Program> programs)
        {
            ItemTypes itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), type);

            if (itemType == ItemTypes.@case)
            {
                cases.Add(new Case(id, name, type, price, manufacturing_date, AddItem<CaseProperties>(json)));
            }  else if (itemType == ItemTypes.motherboard) {
                motherboards.Add(new Motherboard(id, name, type, price, manufacturing_date, AddItem<MotherboardProperties>(json)));
            } else if (itemType == ItemTypes.ram) {
                rams.Add(new RAM(id, name, type, price, manufacturing_date, AddItem<RAMProperties>(json)));
            }  else if (itemType == ItemTypes.psu) {
                powerSupplyUnits.Add(new PowerSupplyUnit(id, name, type, price, manufacturing_date, AddItem<PowerSupplyUnitProperties>(json)));
            } else if (itemType == ItemTypes.cpu) {
                cpus.Add(new CPU(id, name, type, price, manufacturing_date, AddItem<CPUProperties>(json)));
            } else if (itemType == ItemTypes.cpu_cooler) {
                cpu_coolers.Add(new CPUCooler(id, name, type, price, manufacturing_date, AddItem<CPUCoolerProperties>(json)));
            } else if (itemType == ItemTypes.hdd) {
                hdds.Add(new HDD(id, name, type, price, manufacturing_date, AddItem<HDDProperties>(json)));
            } else if (itemType == ItemTypes.monitor) {
                monitors.Add(new Monitor(id, name, type, price, manufacturing_date, AddItem<MonitorProperties>(json)));
            } else if (itemType == ItemTypes.video_card) {
                videoСards.Add(new VideoСard(id, name, type, price, manufacturing_date, AddItem<VideoСardProperties>(json)));
            } else if (itemType == ItemTypes.optical_drive) {
                opticalDrives.Add(new OpticalDrive(id, name, type, price, manufacturing_date, AddItem<OpticalDriveProperties>(json)));
            } else if (itemType == ItemTypes.mouse) {
                mice.Add(new Mouse(id, name, type, price, manufacturing_date, AddItem<MouseProperties>(json)));
            } else if (itemType == ItemTypes.keyboard) {
                keyboards.Add(new Keyboard(id, name, type, price, manufacturing_date, AddItem<KeyboardProperties>(json)));
            } else if (itemType == ItemTypes.optical_disc) {
                discs.Add(new OpticalDisc(id, name, type, price, manufacturing_date, AddItem<OpticalDiscProperties>(json)));
            } else if(itemType == ItemTypes.os) {
                operatingSystems.Add(new OperatingSystem(id, name, type, price, manufacturing_date, AddItem<OperatingSystemProperties>(json)));
            } else if(itemType == ItemTypes.program) {
                programs.Add(new Program(id, name, type, price, manufacturing_date, AddItem<ProgramProperties>(json)));
            }
        }

        public T AddItem<T>(string json)
        {
            T properties = JsonConvert.DeserializeObject<T>(json);
            return properties;
        }
    }

    public class Size //В милиметрах
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }

        public Size(int width, int height, int depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }
    }

    public class BaseItem
    {
        public int Uid { get; set; }
        public ImageSource Image { get; set; }
        public string Name { get; set; }

        private string _type;
        public int Price { get; set; }
        public DateTime ManufacturingDate { get; set; }

        public readonly Dictionary<ItemTypes, string> ItemIcon = new Dictionary<ItemTypes, string>
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
            { Objects.ItemTypes.optical_disc, "pack://application:,,,/Resources/compact-disc.png" },
            { Objects.ItemTypes.os, "pack://application:,,,/Resources/compact-disc.png" },
            { Objects.ItemTypes.program, "pack://application:,,,/Resources/compact-disc.png" },
        };
        public string GetIcon(ItemTypes type)
        {
            if (!ItemIcon.ContainsKey(type)) throw new ArgumentException(string.Format("Operation {0} is invalid", type), "op");
            return (string)ItemIcon[type];
        }

        private readonly Dictionary<ItemTypes, string> ItemTypes = new Dictionary<ItemTypes, string>
        {
            { Objects.ItemTypes.@case, Properties.Resources.Case },
            { Objects.ItemTypes.motherboard, Properties.Resources.Motherboard },
            { Objects.ItemTypes.ram, Properties.Resources.RAM },
            { Objects.ItemTypes.psu, Properties.Resources.PSU },
            { Objects.ItemTypes.cpu, Properties.Resources.CPU },
            { Objects.ItemTypes.cpu_cooler, Properties.Resources.CPUCooler },
            { Objects.ItemTypes.hdd, Properties.Resources.HDD },
            { Objects.ItemTypes.monitor, Properties.Resources.Monitor },
            { Objects.ItemTypes.video_card, Properties.Resources.VideoCard },
            { Objects.ItemTypes.optical_drive, Properties.Resources.OpticalDrive },
            { Objects.ItemTypes.mouse, Properties.Resources.Mouse },
            { Objects.ItemTypes.keyboard, Properties.Resources.Keyboard },
            { Objects.ItemTypes.optical_disc, Properties.Resources.OpticalDisc },
            { Objects.ItemTypes.program, Properties.Resources.Program },
        };
        public string Type
        {
            get
            {
                ItemTypes itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), _type);
                if (!ItemTypes.ContainsKey(itemType)) throw new ArgumentException(string.Format("Operation {0} is invalid", itemType), "op");
                return (string)ItemTypes[itemType];
            }
            set { _type = value; }
        }
        public string GetTypeValue() { return _type; }
    }
    public class Item<P> : BaseItem
    {
        public P Properties { get; set; }
        public Item(int uid, string name, string type, int price, DateTime man_date, P properties)
        {
            Uid = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;
            Properties = properties;
        }

        public override string ToString()
        {
            return Name;
        }

        public string getManufacturingYear()
        {
            return ManufacturingDate.Year.ToString();
        }
    }

    // = CASES ======================================================================== //

    public enum CaseTypes
    {
        AT,
        ATX,
        Slim,
        MiniTower,
        MidiTower,
        BigTower,
        Barebone,
        Rack
    }

    public class CaseProperties
    {
        public CaseTypes CaseType;  //Тип корпуса
        public Collection<MotherboardTypes> FormFactor;
        public Collection<PSUTypes> FormFactorPSU;
        public int CoolerHeight;    //Максимальная высота куллера на процессоре до крышки
        public int VideocardLength; //Максимальная длинна видеокарты до крышки
        public int Sections3_5;     //Секций 3.5
        public int Sections2_5;     //Секций 2.5
        public int ExpansionSlots;  //Слоты расширений
        public int Builtin_Fans;    //Количество встроенных вентиляторов
        public int PlacesFans;      //Свободных мест для установки вентиляторов
        public bool LiquidCooling;  //Возможность установки жидкостного охлождения]
        public int USB2_0;          //Количество USB гнезд 2.0
        public int USB3_0;          //Количество USB гнезд 3.0
        public bool HeadphoneJack;  //Наличие гнезда для наушников
        public bool MicrophoneJack; //Наличие гнезда для микрофона
    }

    public class Case : Item<CaseProperties>
    {
        public Case(int uid, string name, string type, int price, DateTime man_date, CaseProperties properties) : base(uid, name, type, price, man_date, properties) { }
        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Тип корпуса: " + Properties.CaseType + Environment.NewLine;
            info += "Секций 3.5: " + Properties.Sections3_5 + Environment.NewLine;
            info += "Секций 2.5: " + Properties.Sections2_5 + Environment.NewLine;
            info += "Встроенных вентиляторов: " + Properties.Builtin_Fans + Environment.NewLine;
            info += "Мест для вентиляторов: " + Properties.PlacesFans + Environment.NewLine;
            info += "Поддержка жидкостного охлождения: " + (Properties.LiquidCooling ? "Да" : "Нет") + Environment.NewLine;
            info += "USB гнезд 2.0: " + Properties.USB2_0 + Environment.NewLine;
            info += "USB гнезд 3.0: " + Properties.USB3_0 + Environment.NewLine;
            info += "Гнездо для наушников: " + (Properties.HeadphoneJack ? "Да" : "Нет") + Environment.NewLine;
            info += "Гнездо для микрофона: " + (Properties.MicrophoneJack ? "Да" : "Нет");
            return info;
        }

        public int GetCountCompatiblePlaces(HDDFormFactor formFactor)
        {
            switch (formFactor)
            {
                case HDDFormFactor.three_five:
                    return Properties.Sections3_5;
                case HDDFormFactor.two_five:
                    return Properties.Sections2_5;
                default:
                    return 0;
            }
        }

        public bool CheckСapacity(Collection<HDD> hdds)
        {
            int hdd_35 = 0;
            int hdd_25 = 0;
            
            foreach (HDD hdd in hdds)
            {
                if (hdd.Properties.FormFactor == HDDFormFactor.three_five) { hdd_35 += 1; }
                if (hdd.Properties.FormFactor == HDDFormFactor.two_five) { hdd_25 += 1; }
            }
            if (hdd_35 <= Properties.Sections3_5 & hdd_25 <= Properties.Sections2_5) { return true; } else { return false; }
        }
    }

    // = BIOS ========================================================================= //

    public enum MotherboardBIOS
    {
        AMI,
    }

    public class BIOS
    {
        public readonly Dictionary<MotherboardBIOS, string> BiosText = new Dictionary<MotherboardBIOS, string>
        {
            { Objects.MotherboardBIOS.AMI, "AMIBIOS American Megatrends, Inc." },
        };

        public string GetBIOSText(MotherboardBIOS type)
        {
            if (!BiosText.ContainsKey(type)) throw new ArgumentException(string.Format("Operation {0} is invalid", type), "op");
            return (string)BiosText[type];
        }
    }


    // = MOTHERBOARDS ================================================================= //

    public enum MotherboardTypes
    {
        ATX,
        MiniATX,
        NLX,
        FlexATX,
        MicroATX,
        CEB,
        WTX,
        PicoITX,
        NanoITX,
        MiniITX,
        PicoBTX,
        MicroBTX,
        BTX,
        LPX,
        BabyAT
    }

    public enum Sockets
    {
        AM4,
        LGA1151,
        LGA775,
        LGA1150,
        LGA1155,
        LGA1156,
        AM2,
        AM2plus,
        AM3,
        AM3plus,
        FM1,
        FM2,
        FM2plus
    }

    public enum RAMTypes
    {
        SIMM,
        FPM,
        DIMM,
        RIMM,
        DDR,
        DDR2,
        DDR3,
        DDR4
    }

    public enum VideoInterface
    {
        VGA,
        DVI,
        HDMI,
        DisplayPort,
        DVI_D
    }

    public class MotherboardProperties
    {
        public MotherboardTypes MotherboardType;            //Тип корпуса
        public Sockets Socket;                              //Сокет
        public bool MultiCoreProcessor;                     //Поддержка многоядерных процессоров
        public string Chipset;                              //Чипсет
        public MotherboardBIOS BIOS;                        //Биос
        public bool EFI;                                    //Поддержка EFI

        public RAMTypes RamType;                            //Тип поддерживаемой памяти
        public int RAMSlots;                                //Сколько слотов памяти
        public int MinFrequency;                            //Минимальная частота
        public int MaxFrequency;                            //Максимальная частота
        public int RAMVolume;                               //Максимально поддерживаемый объем памяти

        public int IDE;                                     //Количество гнезд IDE
        public int SATA2_0;                                 //Количество гнезд SATA 2.0
        public int SATA3_0;                                 //Количество гнезд SATA 3.0
        public int PCI;                                     //Количество шин PCI
        public int PCI_Ex1;                                 //Количество шин PCI-Ex1
        public int PCI_Ex4;                                 //Количество шин PCI-Ex4
        public int PCI_Ex8;                                 //Количество шин PCI-Ex8
        public int PCI_Ex16;                                //Количество шин PCI-Ex16
        public bool PCIE2_0;                                //Поддержка PCI-Express 2.0
        public bool PCIE3_0;                                //Поддержка PCI-Express 3.0
        public bool EmbeddedGraphics;                       //Поддержка встроенной графики
        public Collection<VideoInterface> VideoInterfaces;  //Гнезда для вывода на монитор     
        public bool Sound;                                  //Наличие встроенной звуковой карты
        public int EthernetSpeed;                           //Скорость сетевой карты (если 0 карта отсутствует или вышла из строя)
        public bool PS2Keyboard;                            //PS/2 для клавиатуры
        public bool PS2Mouse;                               //PS/2 для мышки
        public int USB2_0;                                  //Количество USB гнезд 2.0
        public int USB3_0;                                  //Количество USB гнезд 3.0
    }

    public class Motherboard : Item<MotherboardProperties>
    {
        public Motherboard(int uid, string name, string type, int price, DateTime man_date, MotherboardProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Тип материнской платы: " + Properties.MotherboardType + Environment.NewLine;
            info += "Сокет: " + Properties.Socket + Environment.NewLine;
            info += "Поддержка многоядерных процессоров: " + (Properties.MultiCoreProcessor ? "Да" : "Нет") + Environment.NewLine;
            info += "Чипсет: " + Properties.Chipset + Environment.NewLine;
            info += "Биос: " + Properties.BIOS + Environment.NewLine;
            info += "Оддержка EFI: " + (Properties.EFI ? "Да" : "Нет") + Environment.NewLine;

            info += Environment.NewLine + "Память" + Environment.NewLine;
            info += "Тип: " + Properties.RamType + Environment.NewLine;
            info += "Количество слотов: " + Properties.RAMSlots + Environment.NewLine;
            info += "Частота: " + Properties.MinFrequency + " - " + Properties.MaxFrequency + Environment.NewLine;
            info += "Максимальный объем: " + Properties.RAMVolume + Environment.NewLine + Environment.NewLine;

            info += "Количество гнезд IDE: " + Properties.IDE + Environment.NewLine;
            info += "Количество гнезд SATA 2.0: " + Properties.SATA2_0 + Environment.NewLine;
            info += "Количество гнезд SATA 3.0: " + Properties.SATA3_0 + Environment.NewLine + Environment.NewLine;

            info += "Количество шин PCI: " + Properties.PCI + Environment.NewLine;
            info += "Количество шин PCI-Express x1: " + Properties.PCI_Ex1 + Environment.NewLine;
            info += "Количество шин PCI-Express x4: " + Properties.PCI_Ex4 + Environment.NewLine;
            info += "Количество шин PCI-Express x8: " + Properties.PCI_Ex8 + Environment.NewLine;
            info += "Количество шин PCI-Express x16: " + Properties.PCI_Ex16 + Environment.NewLine + Environment.NewLine;

            info += "Поддержка PCI-Express 2.0: " + (Properties.PCIE2_0 ? "Да" : "Нет") + Environment.NewLine;
            info += "Поддержка PCI-Express 3.0: " + (Properties.PCIE3_0 ? "Да" : "Нет") + Environment.NewLine;
            info += "Наличие звуковой карты: " + (Properties.PCIE3_0 ? "Да" : "Нет") + Environment.NewLine;
            info += "Скорость сетевой карты: " + Properties.EthernetSpeed + Environment.NewLine + Environment.NewLine;

            info += "PS/2 для клавиатуры: " + (Properties.PS2Keyboard ? "Да" : "Нет") + Environment.NewLine;
            info += "PS/2 для мышки: " + (Properties.PS2Mouse ? "Да" : "Нет") + Environment.NewLine;
            info += "USB гнезд 2.0: " + Properties.USB2_0 + Environment.NewLine;
            info += "USB гнезд 3.0: " + Properties.USB3_0;
            return info;
        }

        public bool CheckCompatibility(CaseProperties @case)
        {
            foreach (MotherboardTypes type in @case.FormFactor)
            {
                if (type == Properties.MotherboardType) { return true; }
            }

            return false;
        }

        public int GetCountCompatibleSlots(HDDFormFactor formFactor)
        {
            if (formFactor == HDDFormFactor.two_five || formFactor == HDDFormFactor.three_five)
            {
                return Properties.SATA2_0 + Properties.SATA3_0;
            }

            return 0;
        }
    }

    // = CPU ========================================================================== //
    public class CPUProperties
    {
        public Sockets Socket { get; set; }         //Сокет
        public int NumberCores { get; set; }        //Количество ядер
        public int MinCPUFrequency { get; set; }    //Минимальная частота МГц MHz
        public int MaxCPUFrequency { get; set; }    //Максимальная частота Мгц MHz

        public int MinHeatDissipation { get; set; } //Минимальное тепловыделение
        public int MaxHeatDissipation { get; set; } //Максимальное тепловыделение
        public int MaximumTemperature { get; set; } //Максимальная рабочая температура (градусы по цельсию)
    }

    public class CPU : Item<CPUProperties>
    {
        public CPU(int uid, string name, string type, int price, DateTime man_date, CPUProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Сокет: " + Properties.Socket + Environment.NewLine;
            info += "Количество ядер: " + Properties.NumberCores + Environment.NewLine;
            info += "Частота процессора: " + Properties.MinCPUFrequency + " - " + Properties.MaxCPUFrequency + Environment.NewLine;
            info += "Тепловыделение: " + Properties.MinHeatDissipation + " - " + Properties.MaxHeatDissipation + Environment.NewLine;
            info += "Максимальная рабочая температура: " + Properties.MaximumTemperature;
            return info;
        }
    }

    // = RAMS ========================================================================= //

    public class RAMProperties
    {
        public RAMTypes RAMTypes { get; set; }      //Тип памяти
        public int ClockFrequency { get; set; }     //Частота
        public int Volume { get; set; }             //Объем в мб.
        public double SupplyVoltage { get; set; }   //Напряжение питания
    }

    public class RAM : Item<RAMProperties>
    {
        public RAM(int uid, string name, string type, int price, DateTime man_date, RAMProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Тип памяти: " + Properties.RAMTypes + Environment.NewLine;
            info += "Частота: " + Properties.ClockFrequency + Environment.NewLine;
            info += "Объем: " + Properties.Volume + Environment.NewLine;
            info += "Напряжение: " + Properties.SupplyVoltage;
            return info;
        }
    }

    // = POWER  SUPPLY UNIT ============================================================ //

    public enum PSUTypes
    {
        ATX
    }

    public enum TypeConnectorMotherboard
    {
        pin20plus4
    }

    public class PowerSupplyUnitProperties
    {
        public PSUTypes PSUType { get; set; }                   //Форм фактор
        public TypeConnectorMotherboard TypeCM { get; set; }    //Тип коннектора питания к материнской плате

        public int Pin4plus4CPU { get; set; }                   //Количество пинов 4+4 CPU
        public int Pin6plus2PCIE { get; set; }                  //Количество PCI-E пинов 6+2
        public int Pin15SATA { get; set; }                      //Количество 15 пинов SATA
        public int Pin4IDE { get; set; }                        //Количество 4 пинов IDE

        public int MinNoiseLevel { get; set; }                  //Минимальный уровень шума дБА
        public int MaxNoiseLevel { get; set; }                  //Максимальный уровень шума дБА

        public bool OvervoltageProtection { get; set; }         //Защита от перенапряжения
        public bool OverloadProtection { get; set; }            //Защита от перегрузки
        public bool ShortCircuitProtection { get; set; }        //Защита от короткого замыкания
    }

    public class PowerSupplyUnit : Item<PowerSupplyUnitProperties>
    {
        public PowerSupplyUnit(int uid,
                               string name,
                               string type,
                               int price,
                               DateTime man_date,
                               PowerSupplyUnitProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Форм фактор: " + Properties.PSUType + Environment.NewLine;
            info += "Питание к материнской плате: " + Properties.TypeCM + Environment.NewLine;
            info += "Количество пинов CPU 4+4: " + Properties.Pin4plus4CPU + Environment.NewLine;
            info += "Количество пинов PCI-E 6+2: " + Properties.Pin6plus2PCIE + Environment.NewLine;
            info += "Количество пинов SATA 15: " + Properties.Pin6plus2PCIE + Environment.NewLine;
            info += "Количество пинов IDE 4: " + Properties.Pin6plus2PCIE + Environment.NewLine;
            info += "Уровень шума: " + Properties.MinNoiseLevel + " - " + Properties.MaxNoiseLevel + Environment.NewLine;
            info += "Защита от перенапряжения: " + (Properties.OvervoltageProtection ? "Да" : "Нет") + Environment.NewLine;
            info += "Защита от перегрузки: " + (Properties.OverloadProtection ? "Да" : "Нет") + Environment.NewLine;
            info += "Защита от короткого замыкания: " + (Properties.ShortCircuitProtection ? "Да" : "Нет");
            return info;
        }

        public bool CheckCompatibility(CaseProperties @case)
        {
            foreach (PSUTypes type in @case.FormFactorPSU)
            {
                if (type == Properties.PSUType) { return true; }
            }

            return false;
        }
    }

    // = CPU COLLER ==================================================================== //

    public class CPUCoolerProperties
    {
        public Collection<Sockets> Sockets { get; set; }
        public int MinRotationalSpeed { get; set; }
        public int MaxRotationalSpeed { get; set; }
        public int AirFlow { get; set; }                        //Воздушный поток в CFM
        public double MinNoiseLevel { get; set; }                  //Минимальный уровень шума дБ
        public double MaxNoiseLevel { get; set; }                  //Максимальный уровень шума дБ
        public bool SpeedController { get; set; }               //Регулятор оборотов
        public Size Size { get; set; }
    }

    public class CPUCooler : Item<CPUCoolerProperties>
    {
        public CPUCooler(int uid, string name, string type, int price, DateTime man_date, CPUCoolerProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Сокет: "; foreach (Sockets socket in Properties.Sockets) { info += socket + ", "; } info = info.Remove(info.Length - 2, 2); info += Environment.NewLine;
            info += "Минимальная скорость вращения: " + Properties.MinRotationalSpeed + Environment.NewLine;
            info += "Мaксимальная скорость вращения: " + Properties.MaxRotationalSpeed + Environment.NewLine;
            info += "Воздушный поток: " + Properties.AirFlow + " CFM" + Environment.NewLine;
            info += "Уровень шума: " + Properties.MinNoiseLevel + " - " + Properties.MaxNoiseLevel + Environment.NewLine;
            info += "Регулирование оборотов: " + (Properties.SpeedController ? "Да" : "Нет") + Environment.NewLine;
            info += "Размер: " + Properties.Size.Width + "x" + Properties.Size.Height + "x" + Properties.Size.Depth;
            return info;
        }

        public bool CheckCompatibility(CPUProperties cpu)
        {
            foreach (Sockets type in Properties.Sockets)
            {
                if (type == cpu.Socket) { return true; }
            }

            return false;
        }
    }


    // = MONITOR ======================================================================= //

    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    public class MonitorProperties
    {
        public double Size { get; set; }
        public Resolution Resolution {get; set;}
        public int MaxFrameRefreshRate { get; set; }                           //Максимальная частота обновления кадров (Гц)
        public Collection<VideoInterface> VideoInterfaces { get; set; }
    }

    public class Monitor : Item<MonitorProperties>
    {
        public Monitor(int uid, string name, string type, int price, DateTime man_date, MonitorProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Размер: " + Properties.Size + "\"" + Environment.NewLine;
            info += "Разрешение: " + Properties.Resolution.Width + "x" + Properties.Resolution.Height + Environment.NewLine;
            info += "Максимальная частот обновления кадров: " + Properties.MaxFrameRefreshRate + " Гц." + Environment.NewLine;
            info += "Интерфейс подключения: "; foreach (VideoInterface vInterface in Properties.VideoInterfaces) { info += vInterface + ", "; } info = info.Remove(info.Length - 2, 2);
            return info;
        }

        public bool IsCompatibility(Collection<VideoInterface> vInterfaces)
        {
            foreach (VideoInterface vInterface in vInterfaces)
            {
                foreach (VideoInterface mvInterface in Properties.VideoInterfaces)
                {
                    if (vInterface == mvInterface) { return true; }
                }
            }

            return false;
        }

        public Collection<VideoInterface> Compatibility(Collection<VideoInterface> vInterfaces)
        {
            Collection<VideoInterface> videoInterfaces = new Collection<VideoInterface>();

            foreach (VideoInterface vInterface in vInterfaces)
            {
                foreach (VideoInterface mvInterface in Properties.VideoInterfaces)
                {
                    if (vInterface == mvInterface) { videoInterfaces.Add(mvInterface); }
                }
            }

            return videoInterfaces;
        }
    }

    // = VIDEO CARD ===================================================================== //

    public enum Interface
    {
        PCI_E16x3_0
    }

    public enum TypeVideoMemory
    {
        GDDR5
    }
    public class VideoСardProperties
    {
        public string GraphicsProcessor { get; set; }
        public Interface Interface { get; set; }
        public Resolution MaxResolution { get; set; }
        public int GPUFrequency { get; set; } //МГц
        public int VideoMemory { get; set; } //Кбит
        public TypeVideoMemory TypeVideoMemory { get; set; }
        public int VideoMemoryFrequency { get; set; } //МГц
        public Collection<VideoInterface> VideoInterfaces { get; set; }
    }

    public class VideoСard : Item<VideoСardProperties>
    {
        public VideoСard(int uid, string name, string type, int price, DateTime man_date, VideoСardProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public bool IsCompatibility(MotherboardProperties motherboard)
        {
            if (Properties.Interface == Interface.PCI_E16x3_0 && motherboard.PCI_Ex16 >= 1 && motherboard.PCIE3_0 == true)
            {
                return true;
            } else {
                return false;
            }
        }
        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            return info;
        }
        public int Compatibility(MotherboardProperties motherboard)
        {
            if (Properties.Interface == Interface.PCI_E16x3_0 && motherboard.PCI_Ex16 >= 1 && motherboard.PCIE3_0 == true)
            {
                return motherboard.PCI_Ex16;
            } else {
                return 0;
            }
        }
    }

    // = OPTICAL DRIVE ================================================================== //


    public enum OpticalDriveInterface
    {
        IDE,
        SATA
    }
    public class OpticalDriveProperties
    {
        public OpticalDriveInterface Interface { get; set; }
        public Size Size { get; set; }
        public int[] MaxWritingSpeed { get; set; } //CD-R, CD-RW, DVD-R, DVD-R DL, DVD-RW, DVD+R, DVD+R DL, DVD+RW, DVD-RAM x
        public int MaxReadSpeedCD { get; set; }     //x
        public int MaxReadSpeedDVD { get; set; }    //x
        public int ReadAccessTimeCD { get; set; }   //Время доступа в режиме чтения CD в милисикундах
        public int ReadAccessTimeDVD { get; set; }  //Время доступа в режиме чтения DVD в милисикундах
        public OpticalDisc OpticalDisc { get; set; }
    }
    public class OpticalDrive : Item<OpticalDriveProperties>
    {
        public OpticalDrive(int uid, string name, string type, int price, DateTime man_date, OpticalDriveProperties properties) : base(uid, name, type, price, man_date, properties) { }
        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            return info;
        }
        public int Compatibility(MotherboardProperties motherboard)
        {
            if (Properties.Interface == OpticalDriveInterface.SATA)
            {
                return motherboard.SATA2_0 + motherboard.SATA3_0;
            } else if (Properties.Interface == OpticalDriveInterface.IDE) {
                return motherboard.IDE;
            } else {
                return 0;
            }
        }
    }

    // = MOUSE ========================================================================== //

    public enum InputInterfaces
    {
        PSby2,
        USB,
    }
    public class MouseProperties
    {
        public InputInterfaces Interface { get; set; }
    }
    public class Mouse : Item<MouseProperties>
    {
        public Mouse(int uid, string name, string type, int price, DateTime man_date, MouseProperties properties) : base(uid, name, type, price, man_date, properties) { }
        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            return info;
        }
        public int Compatibility(MotherboardProperties motherboard)
        {
            if (Properties.Interface == InputInterfaces.USB)
            {
                return motherboard.USB2_0 + motherboard.USB3_0;
            } else if (Properties.Interface == InputInterfaces.PSby2 && motherboard.PS2Mouse) {
                return 1;
            } else {
                return 0;
            }
        }

        public int Compatibility(MotherboardProperties motherboard, CaseProperties @case)
        {
            if (Properties.Interface == InputInterfaces.USB)
            {
                return motherboard.USB2_0 + motherboard.USB3_0 + @case.USB2_0 + @case.USB3_0;
            } else if (Properties.Interface == InputInterfaces.PSby2 && motherboard.PS2Mouse)  {
                return 1;
            } else {
                return 0;
            }
        }
    }

    // = KEYBOARD ======================================================================= //

    public class KeyboardProperties
    {
        public InputInterfaces Interface { get; set; }
    }
    public class Keyboard : Item<KeyboardProperties>
    {
        public Keyboard(int uid, string name, string type, int price, DateTime man_date, KeyboardProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            return info;
        }
        public int Compatibility(MotherboardProperties motherboard)
        {
            if (Properties.Interface == InputInterfaces.USB)
            {
                return motherboard.USB2_0 + motherboard.USB3_0;
            } else if (Properties.Interface == InputInterfaces.PSby2 && motherboard.PS2Keyboard) {
                return 1;
            } else {
                return 0;
            }
        }
        public int Compatibility(MotherboardProperties motherboard, CaseProperties @case)
        {
            if (Properties.Interface == InputInterfaces.USB)
            {
                return motherboard.USB2_0 + motherboard.USB3_0 + @case.USB2_0 + @case.USB3_0;
            } else if (Properties.Interface == InputInterfaces.PSby2 && motherboard.PS2Keyboard) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}