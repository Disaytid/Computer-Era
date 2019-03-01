using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    public enum OpticalDiscType
    {
        CD,
        DVD
    }
    public class OpticalDiscProperties
    {
        public OpticalDiscType Type { get; set; }
        public bool Rewritable { get; set;  }
        public int Volume { get; set; }         //В киллобайтах
        public int ReadSpeed { get; set; }      //В x где x = 150 кб/c (килобайт в секунду)
        public int WriteSpeed { get; set; }     //В x где x = 150 кб/c (килобайт в секунду)
        public string CoverName { get; set; }   //Название обложки
        public int OperatingSystem { get; set; }
        public int[] Programms { get; set; }
    }
    public class OpticalDisc : Item<OpticalDiscProperties>
    {
        public OpticalDisc(int uid, string name, string type, int price, DateTime man_date, OpticalDiscProperties properties) : base(uid, name, type, price, man_date, properties) { }

        public override string ToString()
        {
            string str = "Тип: " + Properties.Type + Environment.NewLine +
            "Скорость чтения: x" + Properties.ReadSpeed;
            return str;
        }
    }
}
