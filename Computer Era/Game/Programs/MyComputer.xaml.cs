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
using static Computer_Era.Game.GameConverter;

namespace Computer_Era.Game.Programs
{
    /// <summary>
    /// Логика взаимодействия для MyComputer.xaml
    /// </summary>
    public partial class MyComputer : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        public MyComputer(GameEnvironment gameEnvironment)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;
            Drawing();
        }

        private void Drawing()
        {
            DevicesAndDrives.Children.Clear();
            DevicesAndDrives.ColumnDefinitions.Clear();
            DevicesAndDrives.RowDefinitions.Clear();

            double cell_size = 256;
            double size = Math.Floor(DevicesAndDrives.ActualWidth / cell_size);
            double len = DevicesAndDrives.ActualWidth / size;

            for (int i = 0; i < size; i++)
            {
                ColumnDefinition col = new ColumnDefinition
                { Width = new GridLength(len, GridUnitType.Star) };
                DevicesAndDrives.ColumnDefinitions.Insert(i, col);
            }

            int row = 0;
            int collumn = 0;

            RowDefinition rowd = new RowDefinition
            { Height = new GridLength(0, GridUnitType.Auto) };
            DevicesAndDrives.RowDefinitions.Insert(row, rowd);

            for (int i = 0; i < GameEnvironment.Computers.CurrentPlayerComputer.HDDs.Count; i++)
            {
                for (int j = 0; j < GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions.Count; j++ )
                {
                    Image icon = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/stone-tablet.png")),
                        Stretch = Stretch.UniformToFill,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 54,
                        Height = 54,
                        Margin = new Thickness(10)
                    };

                    string name;
                    if (string.IsNullOrEmpty(GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Name))
                    {
                        name = Properties.Resources.LocalDisk + " (" + GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Letter + ":)";
                    } else {
                        name = GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Name + " (" + GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Letter + ":)";
                    }

                    Label partitionName = new Label
                    {
                        Content = name,
                        FontSize = 16,
                    };

                    double volumeOccupied =  0;
                    if (GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].OperatingSystem != null)
                    { volumeOccupied += GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].OperatingSystem.Properties.Size; }

                    ProgressBar progressBar = new ProgressBar
                    {
                        Height = 20,
                        Value = volumeOccupied * 100 / GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Volume,
                    };

                    KeyValuePair<double, MediaCapacityUnits> free_space = СonversionToMore(MediaCapacityUnits.Kilobyte, GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Volume - volumeOccupied);
                    KeyValuePair<double, MediaCapacityUnits> space = СonversionToMore(MediaCapacityUnits.Kilobyte, GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Volume);

                    Label freeSpace = new Label
                    {
                        Content = Math.Round(free_space.Key, 2) + " " + free_space.Value + " " + Properties.Resources.Free.ToLower() + " " + Properties.Resources.Of.ToLower() + " " + Math.Round(space.Key, 2) + " " + space.Value,
                        FontSize = 8,
                    };

                    StackPanel stackPanel = new StackPanel
                    {

                    };

                    stackPanel.Children.Add(partitionName);
                    stackPanel.Children.Add(progressBar);
                    stackPanel.Children.Add(freeSpace);

                    DockPanel dockPanel = new DockPanel
                    {
                        Margin = new Thickness(10),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };

                    dockPanel.Children.Add(icon);
                    dockPanel.Children.Add(stackPanel);

                    dockPanel.SetValue(Grid.RowProperty, row);
                    dockPanel.SetValue(Grid.ColumnProperty, collumn);
                    DevicesAndDrives.Children.Add(dockPanel);
                    if (collumn == Convert.ToInt32(size) - 1)
                    {
                        collumn = 0;
                        row++;

                        rowd = new RowDefinition
                        { Height = new GridLength(1, GridUnitType.Auto) };
                        DevicesAndDrives.RowDefinitions.Insert(row, rowd);
                    } else { collumn++; }
                }
            }

            for (int i=0; i < GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives.Count; i++)
            {
                Image icon = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/compact-disc.png")),
                    Stretch = Stretch.UniformToFill,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 54,
                    Height = 54,
                    Margin = new Thickness(10)
                };

                string name;
                if (string.IsNullOrEmpty(GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Name))
                {
                   // name = Properties.Resources.LocalDisk + " (" + GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Letter + ":)";
                }
                else
                {
                    //name = GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Name + " (" + GameEnvironment.Computers.CurrentPlayerComputer.HDDs[i].Properties.Partitions[j].Letter + ":)";
                }

                Label partitionName = new Label
                {
                    //Content = name,
                    FontSize = 16,
                };

                ComboBox progressBar = new ComboBox
                {
                    Height = 20,
                };

                Label freeSpace = new Label
                {
                    //Content = Math.Round(free_space.Key, 2) + " " + free_space.Value + " " + Properties.Resources.Free.ToLower() + " " + Properties.Resources.Of.ToLower() + " " + Math.Round(space.Key, 2) + " " + space.Value,
                    FontSize = 8,
                };

                StackPanel stackPanel = new StackPanel
                {

                };

                stackPanel.Children.Add(partitionName);
                stackPanel.Children.Add(progressBar);
                stackPanel.Children.Add(freeSpace);

                DockPanel dockPanel = new DockPanel
                {
                    Margin = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                dockPanel.Children.Add(icon);
                dockPanel.Children.Add(stackPanel);

                dockPanel.SetValue(Grid.RowProperty, row);
                dockPanel.SetValue(Grid.ColumnProperty, collumn);
                DevicesAndDrives.Children.Add(dockPanel);
                if (collumn == Convert.ToInt32(size) - 1)
                {
                    collumn = 0;
                    row++;

                    rowd = new RowDefinition
                    { Height = new GridLength(1, GridUnitType.Auto) };
                    DevicesAndDrives.RowDefinitions.Insert(row, rowd);
                }
                else { collumn++; }
            }
        }

        private void DevicesAndDrives_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Drawing();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
