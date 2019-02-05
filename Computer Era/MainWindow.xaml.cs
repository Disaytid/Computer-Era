using System;
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

namespace Computer_Era
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase dataBase = new DataBase("ComputerEra.db3");
        public SQLiteConnection connection;

        //Объекты (Game/Objects)
        GameEvents events;
        List<Program> programs = new List<Program>();
        Items items;
        Money money;
        Professions professions;
        Companies companies;

        UserControl lastForm = null;

        //DEV
        //int dcout_click = 25;
        //int dclick_fix;

        public MainWindow()
        {
            InitializeComponent();
            connection = dataBase.ConnectDB();

            events = new GameEvents(); //События и игровое время
            events.GameTimer.Minute += this.TimerTick;

            items = new Items(connection, 1); //Загрузка предметов (подключение, id сэйва)
            money = new Money(connection, 1); //Загрузка валют
            professions = new Professions(connection); //Загрузка списка профессий
            companies = new Companies(connection); //Загрузка списка компаний
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

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
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

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Game.Visibility == Visibility.Visible) { DrawDesktop(); }
        }

        private void MenuInventoryItem_Click(object sender, RoutedEventArgs e)
        {
            //inventory = null;
            //inventory = new Inventory(Program, items);
            Inventory inventory = new Inventory(items);
            Program.Children.Add(inventory);
            if (lastForm != null) { lastForm.Visibility = Visibility.Hidden; }
            lastForm = inventory;
            Program.Visibility = Visibility.Visible;
        }

        private void MenuMapItem_Click(object sender, RoutedEventArgs e)
        {
            Map map = new Map(this, events.GameTimer.Timer.Interval);
            Program.Children.Add(map);
            if (lastForm != null) { lastForm.Visibility = Visibility.Hidden; }
            lastForm = map;
            Program.Visibility = Visibility.Visible;
        }

        private void MenuPurseItem_Click(object sender, RoutedEventArgs e)
        {
            Purse purse = new Purse(money.PlayerCurrency, events.GameTimer.DateAndTime);
            Program.Children.Add(purse);
            if (lastForm != null) { lastForm.Visibility = Visibility.Hidden; }
            lastForm = purse;
            Program.Visibility = Visibility.Visible;
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

        public void ShowBuilding(string obj)
        {
            switch (obj)
            {
                case "labor_exchange":
                    LaborExchange l_ex = new LaborExchange(professions.PlayerProfessions, companies.GameCompany, money.PlayerCurrency, events);
                    Program.Children.Add(l_ex);
                    if (lastForm != null) { lastForm.Visibility = Visibility.Hidden; }
                    lastForm = l_ex;
                    Program.Visibility = Visibility.Visible;
                    break;
                default:
                    MessageBox.Show("Вы прибыли к " + obj + "!");
                    break;
            }
        }

    }
}
