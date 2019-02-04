using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Computer_Era.Game
{
    public delegate void MethodContainer();

    public class GameEvents
    {
        public List<GameEvent> Events = new List<GameEvent>();
        public DateTime GameDate = new DateTime(1990, 1, 1, 7, 0, 0); //после проверки удалить

        public GameTimer GameTimer = new GameTimer();

        public GameEvents()
        {
            //GameTimer.Week += Handler1.Message;

            GameTimer.Hour += this.Hour;
            GameTimer.Day += this.Day;

            Events.Add(new GameEvent(1, "123", GameDate, Periodicity.Hour, 1, new MethodContainer(Test)));
            Events.Add(new GameEvent(2, "456", GameDate, Periodicity.Day, 1, new MethodContainer(Test)));
        }

        private void Test()
        {

        }

        private void Minute()
        {

        }

        private void Hour()
        {
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Hour).ToList())
            {
                //MessageBox.Show("Прошел час у " + game_event.Name);
            }
        }

        private void Day()
        {
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Day).ToList())
            {
                MessageBox.Show("Прошел день у " + game_event.Name);
            }
        }

        private void Week()
        {

        }

        private void Month()
        {

        }

        private void Year()
        {

        }
    }

    public enum Periodicity
    {
        No,
        Year,
        Month,
        Week,
        Day,
        Hour,
        Minute
    }

    public class GameEvent
    {
        public int Id;
        public string Name;
        DateTime ResponseTime;
        public Periodicity Periodicity;
        public int PeriodicityValue;
        public MethodContainer Method;

        public GameEvent(int id, string name, DateTime response_time, Periodicity periodicity, int periodicity_value, MethodContainer method)
        {
            Id = id;
            Name = name;
            ResponseTime = response_time;
            Periodicity = periodicity;
            PeriodicityValue = periodicity_value;
            Method = method;
        }
    }

    public class GameTimer  //Таймер
    {
        public DateTime DateAndTime = new DateTime(1990, 1, 1, 7, 0, 0); //Запилить защиту от изменений из вне
        public DispatcherTimer Timer = new DispatcherTimer();
        private DateTime OldDateAndTime;

        private int OldMinute;
        private int OldHour;
        private int OldDay;
        private int OldMonth;

        public delegate void MethodContainer();

        public event MethodContainer Minute;
        public event MethodContainer Hour;
        public event MethodContainer Day;
        public event MethodContainer Week;
        public event MethodContainer Month;
        public event MethodContainer Year;

        public GameTimer()
        {
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 40);

            OldDateAndTime = DateAndTime;

            OldMinute = DateAndTime.Minute;
            OldHour = DateAndTime.Hour;
            OldDay = DateAndTime.Day;
            OldMonth = DateAndTime.Month;
        }

        public void TimerTick(object sender, EventArgs args)
        {
            DateAndTime = DateAndTime.AddMinutes(1);

            //Переписать все проверки они не верны
            if (DateAndTime.Minute > OldMinute)
            {
                OldMinute += DateAndTime.Minute - OldMinute;
                if (OldMinute == 60) { OldMinute = 0;  }
                Minute?.Invoke();
            }
            if (DateAndTime.Hour > OldHour & DateAndTime.Day == OldDay || DateAndTime.Hour < OldHour & DateAndTime.Day > OldDay)
            {
                OldHour += DateAndTime.Hour - OldHour;
                if (OldHour == 24) { OldHour = 0; }
                Hour?.Invoke();
            }
            if (DateAndTime.Day > OldDay & DateAndTime.Month == OldMonth || DateAndTime.Day < OldDay & DateAndTime.Month > OldMonth)
            {
                OldDay += DateAndTime.Day - OldDay;
                if (OldHour > DateTime.DaysInMonth(DateAndTime.Year, DateAndTime.Month)) { OldDay = 1; }
                Day?.Invoke();
                if (DateAndTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    Week?.Invoke();
                }
            }
            if (DateAndTime.Month > OldDateAndTime.Month & DateAndTime.Year == OldDateAndTime.Year || DateAndTime.Month < OldDateAndTime.Month & DateAndTime.Year > OldDateAndTime.Year)
            {
                OldDateAndTime = OldDateAndTime.AddMonths(1);
                Month?.Invoke();
            }
            if (DateAndTime.Year > OldDateAndTime.Year)
            {
                OldDateAndTime = OldDateAndTime.AddYears(1);
                Year?.Invoke();
            }
        }
    }
}
