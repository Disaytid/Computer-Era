using System;
using System.Collections.ObjectModel;

namespace Computer_Era.Game.Objects
{
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
        BabyAT,
        EATX,
    }
    public class MotherboardProperties
    {
        public MotherboardTypes MotherboardType;            //Тип материнской платы
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
        public Collection<VideoInterface> VideoInterfaces = new Collection<VideoInterface>();  //Гнезда для вывода на монитор     
        public bool Sound;                                  //Наличие встроенной звуковой карты
        public int EthernetSpeed;                           //Скорость сетевой карты (если 0 карта отсутствует или вышла из строя) килобит в секунду
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
}
