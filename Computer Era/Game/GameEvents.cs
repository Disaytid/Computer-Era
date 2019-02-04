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

    public class GameEvents
    {
        public List<GameEvent> Events = new List<GameEvent>();
        public DateTime GameDate = new DateTime(1990, 1, 1, 7, 0, 0);

        public GameTimer GameTimer = new GameTimer();

        public GameEvents()
        {
            //GameTimer.Week += Handler1.Message;
        }

        private void Hour()
        {

        }

        private void Day()
        {

        }

        private void Week()
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
        Func<DataTable> Method;

        public GameEvent(int id, string name, DateTime response_time, Periodicity periodicity, int periodicity_value, Func<DataTable> method)
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

        public delegate void MethodContainer();

        public event MethodContainer Minute;
        public event MethodContainer Hour;
        public event MethodContainer Day;
        public event MethodContainer Week;

        public GameTimer()
        {
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 40);

            OldDateAndTime = DateAndTime;
        }

        public void TimerTick(object sender, EventArgs args)
        {
            DateAndTime = DateAndTime.AddMinutes(1);

            if (DateAndTime.Minute > OldDateAndTime.Minute)
            {
                OldDateAndTime = OldDateAndTime.AddMinutes((DateAndTime - OldDateAndTime).TotalMinutes);
                Minute?.Invoke();
            }
            if (DateAndTime.Hour > OldDateAndTime.Hour)
            {
                OldDateAndTime = OldDateAndTime.AddHours((DateAndTime - OldDateAndTime).TotalHours);
                Hour?.Invoke();
            }
            if (DateAndTime.Day > OldDateAndTime.Day)
            {
                OldDateAndTime = OldDateAndTime.AddDays((DateAndTime - OldDateAndTime).TotalDays);
                Day?.Invoke();
                if (DateAndTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    Week?.Invoke();
                }
            }
        }
    }
}
