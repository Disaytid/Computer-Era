using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Threading;

namespace Computer_Era.Game
{
    public delegate void MethodContainer(GameEvent @event);

    public class GameEvents
    {
        public List<GameEvent> Events = new List<GameEvent>();
        public GameTimer GameTimer = new GameTimer();

        public GameEvents()
        {
            GameTimer.Minute += Minute;
            GameTimer.Hour += Hour;
            GameTimer.Day += Day;
            GameTimer.Week += Week;
            GameTimer.Month += Month;
            GameTimer.Year += Year;
        }
        private void EventRun(GameEvent game_event) //Запускает переданный эвент и удаляет в случае отключенного перезапуска
        {
            if (DateTime.Compare(game_event.ResponseTime, GameTimer.DateAndTime) <= 0)
            {
                game_event.Method(game_event);

                if (game_event.Restart)
                {
                    game_event.ResponseTime = GetDateTimeFromPeriodicity(game_event.ResponseTime, game_event.InitialPeriodicity, game_event.PeriodicityValue);
                    game_event.Periodicity = game_event.InitialPeriodicity;
                } else {  Events.Remove(game_event); }
            } else {
                if (game_event.ResponseTime.Year == GameTimer.DateAndTime.Year)
                {
                    if (game_event.Periodicity == Periodicity.Year) { game_event.Periodicity = Periodicity.Month; }
                } else { return; }

                if (game_event.ResponseTime.Month == GameTimer.DateAndTime.Month)
                {
                    if (game_event.Periodicity == Periodicity.Month) { game_event.Periodicity = Periodicity.Week; }
                } else { return; }

                if ((game_event.ResponseTime - GameTimer.DateAndTime).Days <= 7 & (game_event.ResponseTime - GameTimer.DateAndTime).Days >= 0)
                {
                    if (game_event.Periodicity == Periodicity.Week) { game_event.Periodicity = Periodicity.Day; }
                } else { return; }

                if (game_event.ResponseTime.Day == GameTimer.DateAndTime.Day)
                {
                    if (game_event.Periodicity == Periodicity.Day) { game_event.Periodicity = Periodicity.Hour; }
                } else { return; }

                if (game_event.ResponseTime.Hour == GameTimer.DateAndTime.Hour)
                {
                    if (game_event.Periodicity == Periodicity.Hour) { game_event.Periodicity = Periodicity.Minute; }
                } else { return; }
            }
        }

        public DateTime GetDateTimeFromPeriodicity(DateTime dateTime, Periodicity periodicity, int periodicity_value)
        {
            DateTime newDateTime;
            switch (periodicity)
            {
                case Periodicity.Minute:
                    newDateTime = dateTime.AddMinutes(periodicity_value);
                    break;
                case Periodicity.Hour:
                    newDateTime = dateTime.AddHours(periodicity_value);
                    break;
                case Periodicity.Day:
                    newDateTime = dateTime.AddDays(periodicity_value);
                    break;
                case Periodicity.Week:
                    newDateTime = dateTime.AddDays(periodicity_value * 7);
                    break;
                case Periodicity.Month:
                    newDateTime = dateTime.AddMonths(periodicity_value);
                    break;
                case Periodicity.Year:
                    newDateTime = dateTime.AddYears(periodicity_value);
                    break;
                default:
                    newDateTime = dateTime;
                    break;
            }
            return newDateTime;
        }
        private void RunEvents(Periodicity periodicity)
        {
            List<GameEvent> events = Events.Where(e => e.Periodicity == periodicity).ToList();
            for (int i = 0; i < events.Count; i++) { EventRun(events[index: i]); }
        }
        private void Minute() => RunEvents(Periodicity.Minute);
        private void Hour() => RunEvents(Periodicity.Hour);
        private void Day() => RunEvents(Periodicity.Day);
        private void Week() => RunEvents(Periodicity.Week);
        private void Month() => RunEvents(Periodicity.Month);
        private void Year() => RunEvents(Periodicity.Year);

        public string FromPeriodicityToLocalizedString(Periodicity periodicity)
        {
            if (periodicity == Periodicity.Year) { return Properties.Resources.Year; }
            else if (periodicity == Periodicity.Month) { return Properties.Resources.Month; }
            else if (periodicity == Periodicity.Week) { return Properties.Resources.Week; }
            else if (periodicity == Periodicity.Day) { return Properties.Resources.Day; }
            else if (periodicity == Periodicity.Hour) { return Properties.Resources.Hour; }
            else if (periodicity == Periodicity.Minute) { return Properties.Resources.Minute; }
            else { return periodicity.ToString(); }
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
        public Periodicity InitialPeriodicity;
        public Periodicity Periodicity;
        public int PeriodicityValue;
        public MethodContainer Method;
        public bool Restart;

        public GameEvent(string name, DateTime response_time, Periodicity periodicity, int periodicity_value, MethodContainer method, bool restart = false)
        {
            Name = name;
            ResponseTime = response_time;
            InitialPeriodicity = periodicity;
            Periodicity = periodicity;
            PeriodicityValue = periodicity_value;
            Method = method;
            Restart = restart;
        }
    }

    public class GameTimer
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
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 20);

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
            if (DateAndTime.Hour > OldDateAndTime.Hour & DateAndTime.Day == OldDateAndTime.Day
                || DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day > OldDateAndTime.Day
                || DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month > OldDateAndTime.Month
                || DateAndTime.Hour < OldDateAndTime.Hour & DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month < OldDateAndTime.Month & DateAndTime.Year > OldDateAndTime.Year)
            {
                OldDateAndTime = new DateTime(OldDateAndTime.Year, OldDateAndTime.Month, OldDateAndTime.Day, DateAndTime.Hour, OldDateAndTime.Minute, OldDateAndTime.Second);
                Hour?.Invoke();
            }
        }

        private void Days()
        {
            if (DateAndTime.Day > OldDateAndTime.Day & DateAndTime.Month == OldDateAndTime.Month
                || DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month > OldDateAndTime.Month
                || DateAndTime.Day < OldDateAndTime.Day & DateAndTime.Month < OldDateAndTime.Month & DateAndTime.Year > OldDateAndTime.Year)
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
