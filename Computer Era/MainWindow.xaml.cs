using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Computer_Era.Game;
using Computer_Era.Game.Forms;
using Computer_Era.Game.Objects;
using Computer_Era.Game.Scenarios;
using Computer_Era.Game.Widgets;

namespace Computer_Era
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public class GameEnvironment
    {
        public PlayerProfile Player;
        public GameEvents GameEvents;
        public GameMessages Messages;
        public Items Items;
        public GameEnviromentMoney Money;
        public Professions Professions;
        public Companies Companies;
        public Computers Computers;
        public Services Services;

        public Random Random = new Random(DateTime.Now.Millisecond);
    }
    public partial class MainWindow : Window
    { 
        readonly DataBase dataBase = new DataBase("ComputerEra.db3");
        public SQLiteConnection connection;

        GameEnvironment GameEnvironment = new GameEnvironment();

        readonly Widgets Widgets = new Widgets();

        UserControl lastForm = null;

        //DEV
        //int dcout_click = 25;
        //int dclick_fix;

        public MainWindow()
        {
            InitializeComponent();
            connection = dataBase.ConnectDB();

            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.GetInterfaces().Contains(typeof(IScenario))
                                     && t.GetConstructor(Type.EmptyTypes) != null
                            select Activator.CreateInstance(t) as IScenario;
            ScenariosList.ItemsSource = instances;
            if (ScenariosList.Items.Count > 0) { ScenariosList.SelectedIndex = 0; }
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            GameEnvironment.Player = new PlayerProfile(PlayerName.Text);
            GameEnvironment.GameEvents = new GameEvents();
            GameEnvironment.Messages = new GameMessages(GameEnvironment.GameEvents, GameMessage, GameMessagePanel, MessageBubble); //Объявляеться не раньше GameEvents
            GameEnvironment.Items = new Items(connection, 1);
            GameEnvironment.Money = new GameEnviromentMoney(connection, 1); //Объявляеться до Services
            GameEnvironment.Professions = new Professions(connection);
            GameEnvironment.Companies = new Companies(connection);
            GameEnvironment.Computers = new Computers();
            GameEnvironment.Services = new Services(connection, GameEnvironment.Money); //Объявляеться не раньше Money

            if (ScenariosList.SelectedItem != null) { ((IScenario)ScenariosList.SelectedItem).Start(this, GameEnvironment); } else { return; }

            if (PlayerName.Text.Length > 0)
            {
                BaseGrid.Background = new SolidColorBrush(Colors.Transparent);
                GameEnvironment.Player = new PlayerProfile(PlayerName.Text);
                this.Title = "Computer Era | Играет: " + PlayerName.Text;
                MenuPlayerItem.Header = PlayerName.Text;

                GameEnvironment.GameEvents.GameTimer.Minute += this.TimerTick;  

                CreateNewGame.Visibility = Visibility.Hidden;
                GamePanel.Visibility = Visibility.Visible;

                ComputerBoot();
                Game.Visibility = Visibility.Visible;
                GameEnvironment.GameEvents.Events.Add(new GameEvent("CheckComputer", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddMinutes(30), Periodicity.Minute, 30, ComputerBoot, true));

                LoadSave();
                DrawDesktop();
                LabelTime.Text = GameEnvironment.GameEvents.GameTimer.DateAndTime.ToString("HH:mm \r\n dd.MM.yyyy");
            } else {
                MessageBox.Show("Пожалуйста введите имя =)", "Имя любимое мое, мое любимое", MessageBoxButton.OK, MessageBoxImage.Information);
            }  
        }

        private bool OldState = false;
        public void ComputerBoot(GameEvent @event) => ComputerBoot();
        public void ComputerBoot()
        {
            bool currentState = false;
            if (GameEnvironment.Computers.CurrentPlayerComputer != null) { currentState = GameEnvironment.Computers.CurrentPlayerComputer.IsEnable; }

            if (OldState && currentState)
            {
                if (GameEnvironment.Computers.CurrentPlayerComputer != null) { OldState = currentState; }
                return;
            } else {
                OutputFromComputer.Visibility = Visibility.Visible;
                ComputerBootPanel.Visibility = Visibility.Collapsed;
            }

            if (GameEnvironment.Computers.CurrentPlayerComputer != null && currentState == true)
            {  
                foreach (HDD hdd in GameEnvironment.Computers.CurrentPlayerComputer.HDDs)
                {
                    bool isInstalledOS = false;
                    foreach (Partition partition in hdd.Properties.Partitions) { if (partition.OperatingSystem != null) { isInstalledOS = true; break; } }
                    if (isInstalledOS)
                    {
                        // = ЗАГРУЗКА ВИДЖЕТОВ ============================================================ //
                        WidgetPanel.Children.Clear();
                        Widgets.PlayerWidgets.Clear();
                        Widgets.PlayerWidgets.Add(new Widget(new PlayerWidget(GameEnvironment)));
                        Widgets.PlayerWidgets.Add(new Widget(new MoneyWidget(GameEnvironment)));
                        Widgets.PlayerWidgets.Add(new Widget(new ComputerWidget(GameEnvironment)));
                        Widgets.Draw(WidgetPanel);

                        // ================================================================================ //

                        ImageBrush brush = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/agriculture.jpg")),
                            Stretch = Stretch.UniformToFill
                        };
                        this.Background = brush;

                        NoComputerPanel.Visibility = Visibility.Collapsed;
                        Desktop.Visibility = Visibility.Visible;
                        DesktopWidgets.Visibility = Visibility.Visible;

                        OldState = currentState;
                        return;
                    }
                }
                ComputerBootPanel.Visibility = Visibility.Visible;

                Collection<OpticalDisc> opticalDiscs = new Collection<OpticalDisc>();
                foreach (OpticalDrive opticalDrive in GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives)
                {
                    if (opticalDrive.Properties.OpticalDisc != null)
                    {
                        opticalDiscs.Add(opticalDrive.Properties.OpticalDisc);
                        break;
                    }
                }

                if (opticalDiscs.Count == 0)
                {
                    OutputFromComputer.Text = "Reboot and Select proper Boot device \r or Insert Boot Media in selected Boot device";
                } else {
                    OutputFromComputer.Text = "Load from CD...";
                    GameEnvironment.GameEvents.Events.Add(new GameEvent("", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddHours(1), Periodicity.Hour, 1, LoadFromCD));
                }
            } else {
                Desktop.Visibility = Visibility.Collapsed;
                DesktopWidgets.Visibility = Visibility.Collapsed;
                NoComputerPanel.Visibility = Visibility.Visible;

                if (GameEnvironment.Computers.PlayerComputers.Count == 0)
                {
                    NoComputerText.Text = "О ужас, у вас нет компьютера! Нужно это срочно исправить, вот вам пошаговое руководство как исправить это недоразумение:" + Environment.NewLine +
                                          "1. Раздобудьте денег (можно устроиться на работу, можно взять кредит тут уж сами решайте)" + Environment.NewLine +
                                          "2. Закупитесь комплектующими в магазине \"" + Properties.Resources.ComponentStoreName + "\", при покупке обращайте внимание на совместимость компонентов" + Environment.NewLine +
                                          "3. В меню \"Установка комплектуюющих\" создай новую сборку и добавь к ней купленные компоненты" + Environment.NewLine+
                                          "4. В игре можно собирать и хранить несколько компьютеров, однако единовременно работать можно только за одним. Поэтому не забудь созданную сборку установить как сборку по умолчанию." + Environment.NewLine +
                                          "5. Теперь пришла пора проверить правильно ли был собран компьютер и запустить его, сделать это можно в меню \"Тестирование и запуск\"" + Environment.NewLine +
                                          "6. Наслаждаться гудением свежесобранного компьютера!";
                } else {
                    if (GameEnvironment.Computers.CurrentPlayerComputer != null && !GameEnvironment.Computers.CurrentPlayerComputer.IsEnable)
                    { NoComputerText.Text = string.Empty; } else { NoComputerText.Text = "У вас есть компьютерная сборка но она не установлена по умолчанию."; }
                }
            }
            if (GameEnvironment.Computers.CurrentPlayerComputer != null) { OldState = currentState; }
        }

        private void LoadFromCD(GameEvent @event)
        {
            Collection<OpticalDisc> opticalDiscs = new Collection<OpticalDisc>();
            foreach (OpticalDrive opticalDrive in GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives)
            {
                if (opticalDrive.Properties.OpticalDisc != null)
                {
                    opticalDiscs.Add(opticalDrive.Properties.OpticalDisc);
                }
            }

            if (opticalDiscs.Count != 0)
            {
                OutputFromComputer.Visibility = Visibility.Collapsed;
                ComputerBootPanel.Children.RemoveRange(1, ComputerBootPanel.Children.Count);
                ComputerBootPanel.Children.Add(new OSInstallation(GameEnvironment));
            }
            else { OutputFromComputer.Text = "Reboot and Select proper Boot device \r or Insert Boot Media in selected Boot device"; }
        }

        private void LoadSave()
        {
            //int app_id = 1;
        }

        private void DrawDesktop()
        {
            Desktop.ColumnDefinitions.Clear();
            Desktop.RowDefinitions.Clear();

            double cell_size = 96;
            double size = Math.Floor(Desktop.ActualWidth / cell_size);
            double len = Desktop.ActualWidth / size;

            for (int i = 0; i < size; i++)
            {
                ColumnDefinition col = new ColumnDefinition
                { Width = new GridLength(len, GridUnitType.Star) };
                Desktop.ColumnDefinitions.Insert(i, col);
            }

            size = Math.Floor(Desktop.ActualHeight / cell_size);
            len = Desktop.ActualHeight / size;

            for (int i = 0; i < size; i++)
            {
                RowDefinition col = new RowDefinition
                { Height = new GridLength(len, GridUnitType.Star) };
                Desktop.RowDefinitions.Insert(i, col);
            }

            //Прорисовка программ на рабочем столе
            foreach (var program in GameEnvironment.Items.Programs)
            {
                //Написать проверку размеров сетки
                ImageBrush brush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/" + program.Properties.IconName + ".png")),
                    Stretch = Stretch.UniformToFill
                };

                Button button = new Button {
                    Background = brush,
                    Width = 64,
                    Height = 64,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock textBlock = new TextBlock {
                    Text = program.Name, FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.White),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(10, 10, 10, 10)
                };
                stackPanel.Children.Add(button);
                stackPanel.Children.Add(textBlock);

                //stackPanel.SetValue(Grid.RowProperty, program.Row);
                //stackPanel.SetValue(Grid.ColumnProperty, program.Column);
                //Desktop.Children.Add(stackPanel);
            }
        }

        void TimerTick ()
        {
            LabelTime.Text = GameEnvironment.GameEvents.GameTimer.DateAndTime.ToString("HH:mm \r\n dd.MM.yyyy");
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            GameMenu.Visibility = Visibility.Hidden;
            CreateNewGame.Visibility = Visibility.Visible;
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Game.Visibility == Visibility.Visible) { DrawDesktop(); }
        }

        private void NewWindow(UserControl control)
        {
            Program.Children.Add(control);
            if (lastForm != null) { lastForm.Visibility = Visibility.Hidden; }
            lastForm = control;
            Program.Visibility = Visibility.Visible;
        }

        private void MenuInventoryItem_Click(object sender, RoutedEventArgs e) => NewWindow(new Inventory(GameEnvironment));
        private void MenuMapItem_Click(object sender, RoutedEventArgs e) => NewWindow(new Map(this, GameEnvironment));
        private void MenuPurseItem_Click(object sender, RoutedEventArgs e) => NewWindow(new Purse(GameEnvironment));
        private void MenuHardwareItem_Click(object sender, RoutedEventArgs e) => NewWindow(new HardwareInstallation(GameEnvironment));
        private void MenuTestingAndRunningItem_Click(object sender, RoutedEventArgs e) => NewWindow(new TestingAndRunning(GameEnvironment));
        public void ShowBuilding(string obj)
        {
            switch (obj)
            {
                case "labor_exchange":
                    NewWindow(new LaborExchange(GameEnvironment)); break;
                case "computer_parts_store":
                    NewWindow(new Shop(GameEnvironment)); break;
                case "bank":
                    NewWindow(new Bank(GameEnvironment)); break;
                case "disc_stand":
                    NewWindow(new DiscStand(GameEnvironment)); break;
                default:
                    MessageBox.Show("Вы прибыли к " + obj + "!");
                    break;
            }
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            GameEnvironment.GameEvents.GameTimer.Timer.Stop();
        }

        private void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            GameEnvironment.GameEvents.GameTimer.Timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            GameEnvironment.GameEvents.GameTimer.Timer.Start();
        }

        private void FastPlayItem_Click(object sender, RoutedEventArgs e)
        {
            GameEnvironment.GameEvents.GameTimer.Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            GameEnvironment.GameEvents.GameTimer.Timer.Start();
        }

        private void VeryFastPlayItem_Click(object sender, RoutedEventArgs e)
        {
            GameEnvironment.GameEvents.GameTimer.Timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            GameEnvironment.GameEvents.GameTimer.Timer.Start();
        }

        private void GameMessage_Click(object sender, RoutedEventArgs e)
        {
            if (GameMessagePanel.Visibility == Visibility.Collapsed) { GameMessagePanel.Visibility = Visibility.Visible; } else { GameMessagePanel.Visibility = Visibility.Collapsed; }
        }

        private void DellMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            GameMessagePanel.Children.RemoveRange(1, GameMessagePanel.Children.Count - 1);
            GameMessage.Content = String.Empty;
        }
    }
}
