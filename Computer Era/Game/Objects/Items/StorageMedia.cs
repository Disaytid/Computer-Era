﻿using System;

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
        public int ReadSpeed { get; set; }      //В x где для CD x = 150 кб/c, а для DVD 1 352 кб/с (килобайт в секунду)
        public int WriteSpeed { get; set; }     //В x где для CD x = 150 кб/c, а для DVD 1 352 кб/с (килобайт в секунду)
        public string CoverName { get; set; }   //Название обложки
        public int OperatingSystem { get; set; }
        public int[] Programs { get; set; }
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
