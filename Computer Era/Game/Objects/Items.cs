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
                        return "Cистемный блок";
                    case "motherboard":
                        return "Материнская плата";
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

    //CASES
    public enum CaseTypes
    {
        AT,
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
        CaseProperties Propertys = new CaseProperties();

        public Case(int uid, string name, string type, int price, DateTime man_date, CaseProperties propertys)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Propertys = propertys;
        }
    }

    //
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

    public enum MotherboardSockets
    {
        AM4
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
        public MotherboardSockets Socket;           //Сокет
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
        MotherboardProperties Propertys = new MotherboardProperties();

        public Motherboard(int uid, string name, string type, int price, DateTime man_date, MotherboardProperties propertys)
        {
            UId = uid;
            Name = name;
            Type = type;
            Price = price;
            ManufacturingDate = man_date;

            Propertys = propertys;
        }
    }
}
