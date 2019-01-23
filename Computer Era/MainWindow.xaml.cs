using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
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

namespace Computer_Era
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SQLiteConnection connection = ConnectDB();

        public MainWindow()
        {
            InitializeComponent();
        }

        public static SQLiteConnection ConnectDB()
        {
            string baseName = "ComputerEra.db3";

            if (System.IO.File.Exists(baseName))
            {
                SQLiteConnection connection = new SQLiteConnection("Data Source = " + baseName);
                connection.Open();
                return connection;
            }
            else
            {
                SQLiteConnection.CreateFile(baseName);

                SQLiteConnection.CreateFile(baseName);

                SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
                using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
                {
                    connection.ConnectionString = "Data Source = " + baseName;
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"CREATE TABLE [saves] (
                    [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [name] char(100) NOT NULL,
                    [date] datetime NOT NULL
                    );";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                    return connection;
                }
            }
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
        }
    }
}
