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
        readonly Object main;
        readonly DispatcherTimer Timer = new DispatcherTimer();
        string Obj;
        readonly GameEnvironment GameEnvironment;

        public Map(object sender, GameEnvironment gameEnvironment)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;

            main = sender;
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = GameEnvironment.GameEvents.GameTimer.Timer.Interval;

            MapReader mapReader = new MapReader(this);
            MapBrowser.ObjectForScripting = mapReader;

            string path = System.IO.Path.GetFullPath("..\\..\\Game\\Leaflet\\index.html"); //Для релиза скорей всего нужна будет замена
            MapBrowser.Navigate(path);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        TransitionType transition;

        private enum TransitionType
        {
            Walk
        }

        public void TransitionProcessing(string obj)
        {
            Obj = obj;
            MapBrowser.Visibility = Visibility.Collapsed;
            TransitionPanel.Visibility = Visibility.Visible;
        }

        private void Transition(int time) //Доделать, не используеться время
        {
            ChoiсePanel.Visibility = Visibility.Hidden;
            MovePanel.Visibility = Visibility.Visible;

            MoveProgress.Minimum = 0;
            MoveProgress.Maximum = 100;
            MoveProgress.Value = 0;

            Timer.Start();
        }

        private void EndTransition()
        {
            if (transition == TransitionType.Walk)
            {
                if (GameEnvironment.Random.Next(1, 101) <= 10)
                {
                    int money = Convert.ToInt32((double)GameEnvironment.Random.Next(1, 21) / (double)100 * GameEnvironment.Money.PlayerCurrency[0].Course);
                    GameEnvironment.Money.PlayerCurrency[0].TopUp("Нашли на дороге", GameEnvironment.Player.Name, GameEnvironment.GameEvents.GameTimer.DateAndTime, money);
                    GameEnvironment.Messages.NewMessage("Поступление средств", "Оказываеться прогулки на воздухе полезны не только для здоровья но и для кармана. Вы нашли на дороге " + money + " " + GameEnvironment.Money.PlayerCurrency[0].Abbreviation, GameMessages.Icon.Money);
                }
            }
        }

        void TimerTick(object sender, EventArgs args)
        {
            if (MoveProgress.Value == 100)
            {
                Timer.Stop();
                EndTransition();
                if (main is MainWindow)
                {
                    (main as MainWindow).ShowBuilding(Obj);
                }
            } else {
                if (MoveProgress.Value < 100) { MoveProgress.Value += 1; }
            }
        }

        private void Walk_Click(object sender, RoutedEventArgs e)
        {
            transition = TransitionType.Walk;
            Transition(240);
        }
    }
}
