using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                if (SelectedComputer.OpticalDrives.Count > 0)
                {
                    DiscInDrive.IsEnabled = true;
                    Collection<OpticalDisc> opticalDiscs = new Collection<OpticalDisc>();
                    foreach (OpticalDisc disc in GameEnvironment.Items.OpticalDiscs)
                    {
                        if (disc.Properties.OperatingSystem > 0) { opticalDiscs.Add(disc); }
                    }

                    DiscInDrive.ItemsSource = opticalDiscs;
                }
                if (SelectedComputer.IsEnable) { SelectedComputerStart(); }
            }
        }

        private void ComputerStart_Click(object sender, RoutedEventArgs e)
        {
            SelectedComputerStart();
        }

        private void SelectedComputerStart()
        {
            DiscPanel.Visibility = Visibility.Collapsed;
            OutputFromComputer.Visibility = Visibility.Visible;
            OutputFromComputer.Text = string.Empty;

            if (SelectedComputer != null)
            {
                if (SelectedComputer.IsEnable == true) { SelectedComputer.IsEnable = false; return; }
                ComputersList.IsEnabled = false;
                if (SelectedComputer.Case != null)
                {
                    foreach (ErrorСodes errorCode in SelectedComputer.Diagnostics())
                    {
                        if (errorCode == ErrorСodes.Ok)
                        {
                            if (SelectedComputer.Monitors.Count == 0)
                            {
                                GameMessageBox.Show("Запуск компьютера", "У вас нет монитора, это не критично для работы компьютера, но вам не начем будет посмотреть выводимую информацию.", GameMessageBox.MessageBoxType.Information);
                                ComputersList.IsEnabled = true;
                            } else {
                                OutputFromComputer.Text += new BIOS().GetBIOSText(MotherboardBIOS.AMI) + Environment.NewLine +
                                SelectedComputer.Motherboard.Name + " BIOS Date:" + GameEnvironment.GameEvents.GameTimer.DateAndTime.ToString("MM/dd/yy") + Environment.NewLine +
                                "CPU : " + SelectedComputer.CPU.Name + " @ " + SelectedComputer.CPU.Properties.MinCPUFrequency + "MHz";
                                GameEnvironment.GameEvents.Events.Add(new GameEvent("", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddHours(1), Periodicity.Hour, 1, LoadComputer));
                                SelectedComputer.IsEnable = true;
                            }
                        } else if (errorCode == ErrorСodes.NoCPUCooler && SelectedComputer.CPU != null) {
                            if (SelectedComputer.Monitors.Count == 0) { GameMessageBox.Show("Запуск компьютера", "Без куллера на процесоре компьютер запуститься но процессор будет быстро перегреваться в результате чего компьютер будет быстро выключаться!", GameMessageBox.MessageBoxType.Information); }
                            ComputersList.IsEnabled = true;
                        } else  {
                            OutputFromComputer.Text += SelectedComputer.GetLocalizedErrorCode(errorCode) + Environment.NewLine;
                            ComputersList.IsEnabled = true;
                        }
                    }
                } else {
                    GameMessageBox.Show("Запуск компьютера", "Корпус отсутствует, а у вас к сожалению нет навыка включения компьютера без кнопки на корпусе!", GameMessageBox.MessageBoxType.Information);
                }
            }
        }

        private void DiscInDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DiscInDrive.SelectedItem != null)
            {
                if (SelectedComputer.OpticalDrives.Count > 0) { SelectedComputer.OpticalDrives[0].Properties.OpticalDisc = DiscInDrive.SelectedItem as OpticalDisc; }
            }
        }

        private void LoadComputer(GameEvent @event)
        {
            if (DiscInDrive.SelectedItem == null)
            {
                OutputFromComputer.Text = "Reboot and Select proper Boot device \r or Insert Boot Media in selected Boot device";
                ComputersList.IsEnabled = true;
            } else {
                OutputFromComputer.Text = "Load from CD...";
                GameEnvironment.GameEvents.Events.Add(new GameEvent("", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddHours(1), Periodicity.Hour, 1, LoadFromCD));
            }   
        }

        private void LoadFromCD(GameEvent @event)
        {
            if (DiscInDrive.SelectedItem != null)
            {
                OpticalDisc opticalDisc = DiscInDrive.SelectedItem as OpticalDisc;

                foreach (Objects.OperatingSystem operatingSystem in GameEnvironment.Items.AllOperatingSystems)
                {
                    if (opticalDisc.Properties.OperatingSystem == operatingSystem.Uid) { OutputFromComputer.Visibility = Visibility.Collapsed; DiscPanel.Visibility = Visibility.Visible; ComputersList.IsEnabled = true; break; }
                }
                ComputersList.IsEnabled = true;
            } else { OutputFromComputer.Text = "Reboot and Select proper Boot device \r or Insert Boot Media in selected Boot device"; ComputersList.IsEnabled = true; }
        }

        private void StartInstall_Click(object sender, RoutedEventArgs e)
        {
            GameMessageBox.Show("Тестирование и запуск", "Установка производиться непосредственно на главном экране, убедитесь что сборка установлена как сборка по умолчанию и закройте это окно.", GameMessageBox.MessageBoxType.Information);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
