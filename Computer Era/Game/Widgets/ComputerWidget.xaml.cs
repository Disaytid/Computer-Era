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
    /// Логика взаимодействия для ComputerWidget.xaml
    /// </summary>
    public partial class ComputerWidget : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        public ComputerWidget(GameEnvironment gameEnvironment)
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
            if (GameEnvironment.Computers.CurrentPlayerComputer != null) {
                NoComputer.Visibility = Visibility.Collapsed; Computer.Visibility = Visibility.Visible;
                ComputerName.Content = "Имя: " + GameEnvironment.Computers.CurrentPlayerComputer.Name;
                Case.Content = Properties.Resources.Case + ": " + (GameEnvironment.Computers.CurrentPlayerComputer.Case == null ? Properties.Resources.No : GameEnvironment.Computers.CurrentPlayerComputer.Case.Name);
                Motherboard.Content = Properties.Resources.Motherboard + ": " + (GameEnvironment.Computers.CurrentPlayerComputer.Motherboard == null ? Properties.Resources.No : GameEnvironment.Computers.CurrentPlayerComputer.Motherboard.Name);
                PSU.Content = Properties.Resources.PSU + ": " + (GameEnvironment.Computers.CurrentPlayerComputer.PSU == null ? Properties.Resources.No : GameEnvironment.Computers.CurrentPlayerComputer.PSU.Name);
                CPU.Content = Properties.Resources.CPU + ": " + (GameEnvironment.Computers.CurrentPlayerComputer.CPU == null ? Properties.Resources.No : GameEnvironment.Computers.CurrentPlayerComputer.CPU.Name);
            } else { NoComputer.Visibility = Visibility.Visible; Computer.Visibility = Visibility.Collapsed; }
        }
    }
}
