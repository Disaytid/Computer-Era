﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Computer_Era.Game;
using Computer_Era.Game.Forms;
using Computer_Era.Game.Objects;
using Computer_Era.Game.Widgets;

namespace Computer_Era
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Random Random = new Random(DateTime.Now.Millisecond);

        DataBase dataBase = new DataBase("ComputerEra.db3");
        public SQLiteConnection connection;

        //Объекты (Game/Objects)
        PlayerProfile Player;
        GameEvents events;
        GameMessages messages;
        List<Program> programs = new List<Program>();
        Widgets Widgets = new Widgets();
        Items items;
        Money money;
        Professions professions;
        Companies companies;
        Computers computers;

        UserControl lastForm = null;

        //DEV
        //int dcout_click = 25;
        //int dclick_fix;

        public MainWindow()
        {
            InitializeComponent();
            connection = dataBase.ConnectDB();


        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerName.Text.Length > 0)
            {
                Player = new PlayerProfile(PlayerName.Text);
                this.Title = "Computer Era | Играет: " + PlayerName.Text;
                MenuPlayerItem.Header = PlayerName.Text;

                // ================================================================================ //

                events = new GameEvents(); //События и игровое время
                events.GameTimer.Minute += this.TimerTick;
                messages = new GameMessages(events, GameMessage, GameMessagePanel, MessageBubble);

                items = new Items(connection, 1); //Загрузка предметов (подключение, id сэйва)
                money = new Money(connection, 1); //Загрузка валют
                professions = new Professions(connection); //Загрузка списка профессий
                companies = new Companies(connection); //Загрузка списка компаний
                computers = new Computers(); //Компьютеры в сборе

                // = ЗАГРУЗКА ВИДЖЕТОВ ============================================================ //

                Widgets.PlayerWidgets.Add(new Widget(new PlayerWidget(Player, events)));
                Widgets.PlayerWidgets.Add(new Widget(new MoneyWidget(money, events)));
                Widgets.Draw(WidgetPanel);

                // ================================================================================ //

                CreateNewGame.Visibility = Visibility.Hidden;
                Game.Visibility = Visibility.Visible;
                GamePanel.Visibility = Visibility.Visible;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/agriculture.jpg"));
                brush.Stretch = Stretch.UniformToFill;
                this.Background = brush;

                LoadSave();
                DrawDesktop();
                LabelTime.Text = events.GameTimer.DateAndTime.ToString("HH:mm \r\n dd.MM.yyyy");
                events.GameTimer.Timer.Start();
            }
            else
            {
                MessageBox.Show("Пожалуйста введите имя =)", "Имя любимое мое, мое любимое", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadSave()
        {
            //int app_id = 1;

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = @"SELECT * from sv_apps;";
                command.CommandType = CommandType.Text;
                SQLiteDataReader data_reader = command.ExecuteReader();

                while (data_reader.Read())
                {
                    int id = Convert.ToInt32(data_reader["id"]);
                    string name = Convert.ToString(data_reader["name"]);
                    string control_name = Convert.ToString(data_reader["control_name"]);
                    string description = Convert.ToString(data_reader["description"]);
                    string icon_name = Convert.ToString(data_reader["icon_name"]);
                    int row = Convert.ToInt32(data_reader["row"]);
                    int column = Convert.ToInt32(data_reader["column"]);

                    programs.Add(new Program(id, name, control_name, description, icon_name, row, column));
                }
            }
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
            foreach (var program in programs)
            {
                //Написать проверку размеров сетки
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/" + program.IconName + ".png"));
                brush.Stretch = Stretch.UniformToFill;

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

                stackPanel.SetValue(Grid.RowProperty, program.Row);
                stackPanel.SetValue(Grid.ColumnProperty, program.Column);
                Desktop.Children.Add(stackPanel);
            }
        }

        void TimerTick ()
        {
            LabelTime.Text = events.GameTimer.DateAndTime.ToString("HH:mm \r\n dd.MM.yyyy");
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

        private void MenuInventoryItem_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = new Inventory(items);
            NewWindow(inventory);
        }
        private void MenuMapItem_Click(object sender, RoutedEventArgs e)
        {
            Map map = new Map(this, events.GameTimer.Timer.Interval, Random, messages, money);
            NewWindow(map);
        }
        private void MenuPurseItem_Click(object sender, RoutedEventArgs e)
        {
            Purse pl_cur = new Purse(money.PlayerCurrency, events.GameTimer.DateAndTime);
            NewWindow(pl_cur);
        }
        private void MenuHardwareItem_Click(object sender, RoutedEventArgs e)
        {
            HardwareInstallation hard_install = new HardwareInstallation(items, computers);
            NewWindow(hard_install);
        }

        public void ShowBuilding(string obj)
        {
            switch (obj)
            {
                case "labor_exchange":
                    LaborExchange l_ex = new LaborExchange(Player, professions.PlayerProfessions, companies.GameCompany, money.PlayerCurrency, events, Random);
                    NewWindow(l_ex);
                    break;
                default:
                    MessageBox.Show("Вы прибыли к " + obj + "!");
                    break;
            }
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            events.GameTimer.Timer.Stop();
        }

        private void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            events.GameTimer.Timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            events.GameTimer.Timer.Start();
        }

        private void FastPlayItem_Click(object sender, RoutedEventArgs e)
        {
            events.GameTimer.Timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            events.GameTimer.Timer.Start();
        }

        private void VeryFastPlayItem_Click(object sender, RoutedEventArgs e)
        {
            events.GameTimer.Timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            events.GameTimer.Timer.Start();
        }

        private void GameMessage_Click(object sender, RoutedEventArgs e)
        {
            if (GameMessagePanel.Visibility == Visibility.Collapsed) { GameMessagePanel.Visibility = Visibility.Visible; } else { GameMessagePanel.Visibility = Visibility.Collapsed; }
        }
    }
}
