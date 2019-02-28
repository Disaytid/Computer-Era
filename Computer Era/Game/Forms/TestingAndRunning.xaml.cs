using Computer_Era.Game.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using static Computer_Era.Game.Objects.Computer;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для TestingAndRunning.xaml
    /// </summary>
    public partial class TestingAndRunning : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        Computer SelectedComputer;
        public TestingAndRunning(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;

            ComputersList.ItemsSource = GameEnvironment.Computers.PlayerComputers;
        }

        private void ComputersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComputersList.SelectedItem != null)
            {
                SelectedComputer = (ComputersList.SelectedItem as Computer);
                ControlPanel.Visibility = Visibility.Visible;
                AssemblyName.Content = SelectedComputer.Name;
            }
        }

        private void ComputerStart_Click(object sender, RoutedEventArgs e)
        {
            OutputFromComputer.Text = string.Empty;

            if (SelectedComputer != null)
            {
                if (SelectedComputer.Case != null)
                {
                    foreach (ErrorСodes errorCode in SelectedComputer.Diagnostics())
                    {
                        if (errorCode == ErrorСodes.Ok)
                        {
                            if (SelectedComputer.Monitors.Count == 0) { MessageBox.Show("У вас нет монитора, это не критично для работы компьютера, но вам не начем будет посмотреть выводимую информацию."); }
                        } else if (errorCode == ErrorСodes.NoCPUCooler && SelectedComputer.CPU != null) {
                            if (SelectedComputer.Monitors.Count == 0) { MessageBox.Show("Без куллера на процесоре компьютер запуститься но процессор будет быстро перегреваться в результате чего компьютер будет быстро выключаться!"); }
                        } else  {
                            OutputFromComputer.Text += SelectedComputer.GetLocalizedErrorCode(errorCode) + Environment.NewLine;
                        }
                    }  
                } else {
                    GameMessageBox.Show("Запуск компьютера", "Корпус отсутствует, а у вас к сожалению нет навыка включения компьютера без кнопки на корпусе!", GameMessageBox.MessageBoxType.Information);
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
