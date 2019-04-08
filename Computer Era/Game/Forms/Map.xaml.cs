using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        string Obj;
        readonly GameEnvironment GameEnvironment;

        public Map(object sender, GameEnvironment gameEnvironment)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;

            MapReader mapReader = new MapReader(this);
            MapBrowser.ObjectForScripting = mapReader;

            string path = System.IO.Path.GetFullPath("..\\..\\Game\\Leaflet\\index.html"); //Для релиза скорей всего нужна будет замена
            MapBrowser.Navigate(path);
        }

        TransitionType transition;

        private enum TransitionType
        {
            Walk,
            Bus,
        }

        public void TransitionProcessing(string obj)
        {
            Obj = obj;
            MapBrowser.Visibility = Visibility.Collapsed;
            TransitionPanel.Visibility = Visibility.Visible;
        }

        int max = 100;
        private void Transition(int time) //Время в минутах
        {
            time *= 2;
            ChoiсePanel.Visibility = Visibility.Hidden;
            MovePanel.Visibility = Visibility.Visible;

            int periodicity = 1;
            if (time < 100)
            {
                max = time;
            } else {
                max = 100;
                periodicity = Convert.ToInt32(Math.Round((double)time / 100));
            }

            MoveProgress.Minimum = 0;
            MoveProgress.Maximum = max;
            MoveProgress.Value = 0;

            GameEnvironment.GameEvents.Events.Add(new GameEvent("Move", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddMinutes(periodicity), Periodicity.Minute, periodicity, Move, true));
            //Timer.Start();
        }

        private void Move(GameEvent @event)
        {
            if (MoveProgress.Value == max)
            {
                GameEnvironment.GameEvents.Events.Remove(@event);
                EndTransition();
                GameEnvironment.Main.ShowBuilding(Obj);
            } else {
                MoveProgress.Value += 1;
            }
        }

        private void EndTransition()
        {
            switch (transition)
            {
                case TransitionType.Walk:
                    if (GameEnvironment.Random.Next(1, 101) <= 10)
                    {
                        int money = Convert.ToInt32(GameEnvironment.Random.Next(1, 21) / (double)100 * GameEnvironment.Money.PlayerCurrency[0].Course);
                        GameEnvironment.Money.PlayerCurrency[0].TopUp("Нашли на дороге", GameEnvironment.Player.Name, GameEnvironment.GameEvents.GameTimer.DateAndTime, money);
                        GameEnvironment.Messages.NewMessage("Поступление средств", "Оказываеться прогулки на воздухе полезны не только для здоровья но и для кармана. Вы нашли на дороге " + money + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation, GameMessages.Icon.Money);
                    }
                    break;
                case TransitionType.Bus:
                    if (GameEnvironment.Random.Next(0, 2) == 1)
                    {
                        if (!payment)
                        {
                            int fine = 15; //Размер штрафа в универсальной игровой валюте
                            if (GameEnvironment.Money.PlayerCurrency[0].Withdraw("Оплата штрафа за неоплаченный проезд", "Автобусный парк №1", GameEnvironment.GameEvents.GameTimer.DateAndTime, fine * GameEnvironment.Money.PlayerCurrency[0].Course))
                            {
                                GameEnvironment.Messages.NewMessage("Автобусный парк #1", "Вам был выписан штраф за неоплаченный проезд!", GameMessages.Icon.Info);
                            } else { GameEnvironment.Scenario.GameOver("Вы не смогли оплатить штраф за неоплаченный проезд."); }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Walk_Click(object sender, RoutedEventArgs e)
        {
            transition = TransitionType.Walk;
            int transition_time = 15;
            int speed = 6000 / 60; //метров в минуту где 6000 скорость пешехода в метрах/ч, а 60 количество минут в часе
            if (GameEnvironment.Player.House != null) { transition_time += Convert.ToInt32(Math.Floor(GameEnvironment.Player.House.Distance / (double)speed)); } 
            Transition(transition_time);
        }

        double fare = 0.1;
        bool payment = false;
        private void ByBus_Click(object sender, RoutedEventArgs e)
        {
            transition = TransitionType.Bus;
            int transition_time = 3;
            int speed = 40000 / 60; //метров в минуту где 40000 скорость пешехода в метрах/ч, а 60 количество минут в часе
            int price = Convert.ToInt32(0.15 * GameEnvironment.Money.PlayerCurrency[0].Course);
            if (GameEnvironment.Player.House != null)
            {
                transition_time += Convert.ToInt32(Math.Floor(GameEnvironment.Player.House.Distance / (double)speed));
                price += Convert.ToInt32(GameEnvironment.Player.House.Distance / 1000 * fare * GameEnvironment.Money.PlayerCurrency[0].Course);
            }
            if (GameMessageBox.Show("Оплата проезда", "Вы хотите купить билет за " + price + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation + "?", GameMessageBox.MessageBoxType.ConfirmationWithYesNo) == MessageBoxResult.Yes)
            {
                if (!GameEnvironment.Money.PlayerCurrency[0].Withdraw("Оплата за проезд в автобусе", GameEnvironment.Player.Name, GameEnvironment.GameEvents.GameTimer.DateAndTime, price))
                { GameMessageBox.Show("Оплата проезда", "У вас не хватает денег!", GameMessageBox.MessageBoxType.Information); return;  } else { payment = true; }
            } else { payment = false; }
            Transition(transition_time);
        }   

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
