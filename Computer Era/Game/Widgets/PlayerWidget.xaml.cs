using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Computer_Era.Game.Widgets
{
    /// <summary>
    /// Логика взаимодействия для PlayerWidget.xaml
    /// </summary>
    public partial class PlayerWidget : UserControl
    {
        GameEnvironment GameEnvironment;

        public PlayerWidget(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;
            Update();
            GameEnvironment.GameEvents.Events.Add(new GameEvent("player widget", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddHours(1), Periodicity.Hour, 1, Update, true));
        }
        
        private void Update(GameEvent @event)
        {
            Update();
        }
        private void Update()
        {
            PlayerName.Content = GameEnvironment.Player.Name;

            if (GameEnvironment.Player.Job != null)
            {
                PlayerJob.Content = "Работа: " + GameEnvironment.Player.Job.Name;
            } else {
                PlayerJob.Content = "Работа: нет";
            }
        }

        private void IsEnabledPlayerJob_Checked(object sender, RoutedEventArgs e)
        {
            PlayerJob.Visibility = Visibility.Visible;
        }

        private void IsEnabledPlayerName_Checked(object sender, RoutedEventArgs e)
        {
            PlayerName.Visibility = Visibility.Visible;
        }

        private void IsEnabledPlayerName_Unchecked(object sender, RoutedEventArgs e)
        {
            PlayerName.Visibility = Visibility.Collapsed;
        }

        private void IsEnabledPlayerJob_Unchecked(object sender, RoutedEventArgs e)
        {
            PlayerJob.Visibility = Visibility.Collapsed;
        }
    }
}
