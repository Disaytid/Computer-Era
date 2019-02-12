using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using Computer_Era.Game.Objects;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        Object main;
        DispatcherTimer Timer = new DispatcherTimer();
        string Obj;
        Random rnd;
        GameMessages Messages;
        Money Money;

        public Map(object sender, TimeSpan timeSpan, Random random, GameMessages messages, Money money)
        {
            InitializeComponent();

            main = sender;
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = timeSpan;
            rnd = random;
            Messages = messages;
            Money = money;

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

        private void Transition(int time)
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
                int rm = rnd.Next(1, 21);
                int money = Convert.ToInt32(Math.Floor(rm / 100 * Money.PlayerCurrency[0].Course));
                Money.PlayerCurrency[0].TopUp(money);
                if (rnd.Next(1, 101) <= 90) { Messages.NewMessage("Поступление средств", "Оказываеться прогулки на воздухе полезны не только для здоровья но и для кармана. Вы нашли на дороге " + money + " " + Money.PlayerCurrency[0].Abbreviation, GameMessages.Icon.Money); }
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
