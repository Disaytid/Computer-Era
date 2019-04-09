using System;
using System.Collections.ObjectModel;

namespace Computer_Era.Game.Objects
{
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
}
