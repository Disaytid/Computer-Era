using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    class Item
    {
        public int UId;
        public string Name;
        public string Type;
        public DateTime ManufacturingDate;
    }

    enum CaseTypes
    {
        AT,
        Slim,
        MiniTower,
        MidiTower,
        BigTower,
        Barebone,
        Rack
    }

    class Case : Item
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
}
