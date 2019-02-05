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
        public DateTime GameDate = new DateTime();

        public GameTimer GameTimer = new GameTimer();

        public GameEvents()
        {

        }

        private void EventRun(GameEvent game_event) //Запускает переданный эвент и удаляет в случае отключенного перезапуска
        {
            if (DateTime.Compare(game_event.ResponseTime, GameTimer.DateAndTime) <= 0)
            {
                game_event.Method();

                if (game_event.Restart)
                {
                    game_event.ResponseTime = game_event.ResponseTime.AddHours(game_event.PeriodicityValue);
                }
                else {  Events.Remove(game_event); }
            }
        }

        private void Minute()
        {
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Minute).ToList())
            {
                EventRun(game_event);
            }
        }

        private void Hour()
        {
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Hour).ToList())
            {
                EventRun(game_event);
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
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Week).ToList())
            {
                EventRun(game_event);
            }
        }

        private void Month()
        {
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Month).ToList())
            {
                EventRun(game_event);
            }
        }

        private void Year()
        {
            foreach (GameEvent game_event in Events.Where<GameEvent>(e => e.Periodicity == Periodicity.Year).ToList())
            {
                EventRun(game_event);
            }
        }
    }

    public enum Periodicity
    {
        Year,
        Month,
        Week,
        Day,
        Hour,
        Minute
    }

    public class GameEvent
    {
        public string Name;
        public DateTime ResponseTime;
        public Periodicity Periodicity;
        public int PeriodicityValue;
        public MethodContainer Method;
        public bool Restart;


        public GameEvent(string name, DateTime response_time, Periodicity periodicity, int periodicity_value, MethodContainer method, bool restart=false)
        {
            Name = name;
            ResponseTime = response_time;
            Periodicity = periodicity;
            PeriodicityValue = periodicity_value;
            Method = method;
            Restart = restart;
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
        public event MethodContainer Month;
        public event MethodContainer Year;

        public GameTimer()
        {
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 40);

            OldDateAndTime = DateAndTime;
        }

        public void TimerTick(object sender, EventArgs args)
        {
            DateAndTime = DateAndTime.AddMinutes(1);

            Minutes();
            Hours();
            Days();
            Months();
            Years();
        }

        private void Minutes()
        {
            if (DateAndTime.Minute > OldDateAndTime.Minute & DateAndTime.Hour == OldDateAndTime.Hour 
                || DateAndTime.Minute < OldDateAndTime.Minute & DateAndTime.Hour > OldDateAndTime.Hour
                || DateAndTime.Minute < OldDateAndTime.Minute & DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day > OldDateAndTime.Day
                || DateAndTime.Minute < OldDateAndTime.Minute & DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month > OldDateAndTime.Month
                || DateAndTime.Minute < OldDateAndTime.Minute & DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month < OldDateAndTime.Month & DateAndTime.Year > OldDateAndTime.Year)
            {
                OldDateAndTime = new DateTime(OldDateAndTime.Year, OldDateAndTime.Month, OldDateAndTime.Day, OldDateAndTime.Hour, DateAndTime.Minute, OldDateAndTime.Second);
                Minute?.Invoke();
            }
        }

        private void Hours()
        {
            if (DateAndTime.Hour > OldDateAndTime.Hour & DateAndTime.Day == OldDateAndTime.Day || DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day > OldDateAndTime.Day)
            {
                OldDateAndTime = new DateTime(OldDateAndTime.Year, OldDateAndTime.Month, OldDateAndTime.Day, DateAndTime.Hour, OldDateAndTime.Minute, OldDateAndTime.Second);
                Hour?.Invoke();
            }
        }

        private void Days()
        {
            if (DateAndTime.Day > OldDateAndTime.Day & DateAndTime.Month == OldDateAndTime.Month || DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month > OldDateAndTime.Month)
            {
                OldDateAndTime = new DateTime(OldDateAndTime.Year, OldDateAndTime.Month, DateAndTime.Day, OldDateAndTime.Hour, OldDateAndTime.Minute, OldDateAndTime.Second);
                Day?.Invoke();
                if (DateAndTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    Week?.Invoke();
                }
            }
        }

        private void Months()
        {
            if (DateAndTime.Month > OldDateAndTime.Month & DateAndTime.Year == OldDateAndTime.Year || DateAndTime.Month < OldDateAndTime.Month & DateAndTime.Year > OldDateAndTime.Year)
            {
                OldDateAndTime = new DateTime(OldDateAndTime.Year, DateAndTime.Month, OldDateAndTime.Day, OldDateAndTime.Hour, OldDateAndTime.Minute, OldDateAndTime.Second);
                Month?.Invoke();
            }
        }

        private void Years()
        {
            if (DateAndTime.Year > OldDateAndTime.Year)
            {
                OldDateAndTime = new DateTime(DateAndTime.Year, OldDateAndTime.Month, OldDateAndTime.Day, OldDateAndTime.Hour, OldDateAndTime.Minute, OldDateAndTime.Second);
                Year?.Invoke();
            }
        }
    }
}
