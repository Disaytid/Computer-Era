using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
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

}
