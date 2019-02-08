using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Computer_Era.Game.Objects
{
    public class Items
    {
        SQLiteConnection Connection;
        public Collection<Case> Cases = new Collection<Case>();
        public Collection<Motherboard> Motherboards = new Collection<Motherboard>();
        public Collection<RAM> RAMs = new Collection<RAM>();
        public Collection<PowerSupplyUnit> PowerSupplyUnits = new Collection<PowerSupplyUnit>();
        public Collection<CPU> CPUs = new Collection<CPU>();

        public Items(SQLiteConnection connection, int save_id)
        {
            Connection = connection;

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * from sv_inventorys WHERE save_id=" + save_id + ";";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string name = Convert.ToString(data_reader["name"]);
                    string type = Convert.ToString(data_reader["type"]);
                    int price = Convert.ToInt32(data_reader["price"]);
                    DateTime manufacturing_date = Convert.ToDateTime(data_reader["manufacturing_date"]);

                    string json;
                    switch (type)
                    {
                        case "case":
                            json = Convert.ToString(data_reader["properties"]);
                            CaseProperties case_properties = JsonConvert.DeserializeObject<CaseProperties>(json);
                            Cases.Add(new Case(id, name, type, price, manufacturing_date, case_properties));
                            break;
                        case "motherboard":
                            json = Convert.ToString(data_reader["properties"]);
                            MotherboardProperties motherboard_properties = JsonConvert.DeserializeObject<MotherboardProperties>(json);
                            Motherboards.Add(new Motherboard(id, name, type, price, manufacturing_date, motherboard_properties));
                            break;
                        case "ram":
                            json = Convert.ToString(data_reader["properties"]);
                            RAMProperties ram_properties = JsonConvert.DeserializeObject<RAMProperties>(json);
                            RAMs.Add(new RAM(id, name, type, price, manufacturing_date, ram_properties));
                            break;
                        case "psu":
                            json = Convert.ToString(data_reader["properties"]);
                            PowerSupplyUnitProperties psu_properties = JsonConvert.DeserializeObject<PowerSupplyUnitProperties>(json);
                            PowerSupplyUnits.Add(new PowerSupplyUnit(id, name, type, price, manufacturing_date, psu_properties));
                            break;
                        case "cpu":
                            json = Convert.ToString(data_reader["properties"]);
                            CPUProperties cpu_properties = JsonConvert.DeserializeObject<CPUProperties>(json);
                            CPUs.Add(new CPU(id, name, type, price, manufacturing_date, cpu_properties));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public class Item
    {
        public int UId;
        public ImageSource Image { get; set; }
        public string Name { get; set; }
        private string type;
        public int Price { get; set; }
        public DateTime ManufacturingDate { get; set; }

        public string Type
        {
            get
            {
                switch (type)
                {
                    case "case":
                        return "Корпус";
                    case "motherboard":
                        return "Материнская плата";
                    case "ram":
                        return "Оперативная память";
                    case "psu":
                        return "Блок питания";
                    case "cpu":
                        return "Процессор";
                    default:
                        return type;
                }
            }
            set { this.type = value; }
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

    public class Case : Item
    {
        CaseProperties Properties = new CaseProperties();

        public Case(int uid, string name, string type, int price, DateTime man_date, CaseProperties properties)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Properties = properties;
        }

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
        LGA1151
    }

    public enum MotherboardBIOS
    {
        AMI
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

    public class MotherboardProperties
    {
        public MotherboardTypes MotherboardType;    //Тип корпуса
        public Sockets Socket;                      //Сокет
        public bool MultiCoreProcessor;             //Поддержка многоядерных процессоров
        public string Chipset;                      //Чипсет
        public MotherboardBIOS BIOS;                //Биос
        public bool EFI;                            //Поддержка EFI

        public RAMTypes RamType;                    //Тип поддерживаемой памяти
        public int RAMSlots;                        //Сколько слотов памяти
        public int MinFrequency;                    //Минимальная частота
        public int MaxFrequency;                    //Максимальная частота
        public int RAMVolume;                       //Максимально поддерживаемый объем памяти

        public int IDE;                             //Количество гнезд IDE
        public int SATA2_0;                         //Количество гнезд SATA 2.0
        public int SATA3_0;                         //Количество гнезд SATA 3.0
        public int PCI;                             //Количество шин PCI
        public int PCI_Ex1;                         //Количество шин PCI-Ex1
        public int PCI_Ex4;                         //Количество шин PCI-Ex4
        public int PCI_Ex8;                         //Количество шин PCI-Ex8
        public int PCI_Ex16;                        //Количество шин PCI-Ex16
        public bool PCIE2_0;                        //Поддержка PCI-Express 2.0
        public bool PCIE3_0;                        //Поддержка PCI-Express 3.0
        public bool Sound;                          //Наличие встроенной звуковой карты
        public int EthernetSpeed;                   //Скорость сетевой карты (если 0 карта отсутствует или вышла из строя)
        public bool PS2Keyboard;                    //PS/2 для клавиатуры
        public bool PS2Mouse;                       //PS/2 для мышки
        public int USB2_0;                          //Количество USB гнезд 2.0
        public int USB3_0;                          //Количество USB гнезд 3.0
    }

    public class Motherboard : Item
    {
        MotherboardProperties Properties = new MotherboardProperties();

        public Motherboard(int uid, string name, string type, int price, DateTime man_date, MotherboardProperties properties)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Properties = properties;
        }

        public bool CheckCompatibility(CaseProperties @case)
        {

            return false;
        }

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
    }

    // = CPU ========================================================================== //

    public class CPUProperties
    {
        public Sockets Socket { get; set; }         //Сокет
        public int NumberCores { get; set; }        //Количество ядер
        public int MinCPUFrequency { get; set; }    //Минимальная частота
        public int MaxCPUFrequency { get; set; }    //Максимальная частота

        public int MinHeatDissipation { get; set; } //Минимальное тепловыделение
        public int MaxHeatDissipation { get; set; } //Максимальное тепловыделение
        public int MaximumTemperature { get; set; } //Максимальная рабочая температура (градусы по цельсию)
    }

    public class CPU : Item
    {
        public CPUProperties Properties { get; set; } = new CPUProperties();

        public CPU(int uid, string name, string type, int price, DateTime man_date, CPUProperties properties)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Properties = properties;
        }

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

    public class RAM : Item
    {
        RAMProperties Properties { get; set; } = new RAMProperties();

        public RAM(int uid, string name, string type, int price, DateTime man_date, RAMProperties properties)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Properties = properties;
        }

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

    public class PowerSupplyUnit : Item
    {
        public PowerSupplyUnitProperties Properties { get; set; } = new PowerSupplyUnitProperties();

        public PowerSupplyUnit(int uid, string name, string type, int price, DateTime man_date, PowerSupplyUnitProperties properties)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Properties = properties;
        }

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
    }
}
