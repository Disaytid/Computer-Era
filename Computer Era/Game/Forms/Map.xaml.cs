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

        public Map(object sender, TimeSpan timeSpan)
        {
            InitializeComponent();

            main = sender;
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = timeSpan;

            MapReader mapReader = new MapReader(this);
            MapBrowser.ObjectForScripting = mapReader;

            string path = System.IO.Path.GetFullPath("..\\..\\Game\\Leaflet\\index.html"); //Для релиза скорей всего нужна будет замена
            MapBrowser.Navigate(path);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
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

        void TimerTick(object sender, EventArgs args)
        {
            if (MoveProgress.Value == 100)
            {
                Timer.Stop();
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
            Transition(240);
        }
    }
}
