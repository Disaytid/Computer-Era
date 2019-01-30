using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Computer_Era.Game;
using Computer_Era.Game.Forms;
using Computer_Era.Game.Objects;
using Newtonsoft.Json;

namespace Computer_Era
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase dataBase = new DataBase("ComputerEra.db3");
        public SQLiteConnection connection;
        List<Program> programs = new List<Program>();
        Items items;
        DispatcherTimer timer = new DispatcherTimer();
        DateTime GameDate = new DateTime(1990, 1, 1,7,0,0);

        public MainWindow()
        {
            InitializeComponent();
            connection = dataBase.ConnectDB();

            items = new Items(connection, 1); //Загрузка предметов (подключение, id сэйва)
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            
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

            //MessageBox.Show((Desktop.ActualWidth / cell_size).ToString());

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

        void TimerTick (object sender, EventArgs args)
        {
            GameDate = GameDate.AddMinutes(1);
            LabelTime.Text = GameDate.ToString("HH:mm \r\n dd.MM.yyyy");
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
            LabelTime.Text = GameDate.ToString("HH:mm \r\n dd.MM.yyyy");
            timer.Start();
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
            Program.Visibility = Visibility.Visible;
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            timer.Start();
        }

        private void FastPlayItem_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Start();
        }

        private void VeryFastPlayItem_Click(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();
        }
    }
}
