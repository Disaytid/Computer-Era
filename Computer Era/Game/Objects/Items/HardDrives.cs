using System;
using System.Collections.ObjectModel;

namespace Computer_Era.Game.Objects
{
    // = HDD =========================================================================== //

    public enum HDDFormFactor
    {
        three_five,
        two_five
    }

    public enum HDDInterface
    {
        sata_20,
        sata_30,
        IDE
    }

    public class HDDProperties
    {
        public HDDFormFactor FormFactor { get; set; }
        public int Volume { get; set; }                      //В килобайтах
        public int WriteSpeed { get; set; }                 //В килобайтах в секунду
        public int ReadSpeed { get; set; }                  //В килобайтах в секунду
        public int BufferCapacity { get; set; }
        public HDDInterface Interface { get; set; }
        public int MaximumTemperature { get; set; }         //В градусах по цельсию
        public Collection<Partition> Partitions { get; set; } = new Collection<Partition>();
    }

    public class HDD : Item<HDDProperties>
    {
        public HDD(int uid, string name, string type, int price, DateTime man_date, HDDProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string info = "Имя: " + Name + Environment.NewLine;
            info += "Форм фактор: " + (Properties.FormFactor == HDDFormFactor.three_five ? "3.5" : "2.5") + Environment.NewLine;
            info += "Объем: " + Properties.Volume + " Кбит" + Environment.NewLine;
            info += "Скорость записи: " + Properties.WriteSpeed + " Кбит/c" + Environment.NewLine;
            info += "Скорость чтения: " + Properties.ReadSpeed + " Кбит/c" + Environment.NewLine;
            info += "Интерфейс: " + (Properties.Interface == HDDInterface.sata_20 ? "SATA 2.0" : "SATA 3.0") + Environment.NewLine;
            info += "Максимальная рабочая температура: " + Properties.MaximumTemperature + " °C";
            return info;
        }

        public int Compatibility(MotherboardProperties motherboard)
        {
            if (Properties.Interface == HDDInterface.sata_20 || Properties.Interface == HDDInterface.sata_30)
            {
                return motherboard.SATA2_0 + motherboard.SATA3_0;
            }  else if (Properties.Interface == HDDInterface.IDE) {
                return motherboard.IDE;
            } else {
                return 0;
            }
        }
    }
    public enum FileSystem
    {
        FAT12,
        FAT16,
        FAT32,
        VFAT,
        NTFS
    }
    public class Partition
    {
        public string Name { get; set; }
        public int PartitionNumber { get; set; }
        public string Letter { get; set; }
        public int Volume { get; set; } //В килобайтах
        public FileSystem FileSystem { get; set; }
        public OperatingSystem OperatingSystem { get; set; }
    }
}
