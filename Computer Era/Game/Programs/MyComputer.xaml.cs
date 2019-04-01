using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        bool initialize = false;
        public MyComputer(GameEnvironment gameEnvironment)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;
            initialize = true;
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
                if (string.IsNullOrEmpty(GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.Letter)) { continue; }
                if (GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.OpticalDisc == null)
                {
                    name = Properties.Resources.OpticalDrive + " (" + GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.Letter + ":)";
                } else {
                    name = GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.OpticalDisc.Name + " (" + GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.Letter + ":)";
                }

                Collection<OpticalDisc> opticalDiscs = new Collection<OpticalDisc>();
                for (int j=0; j < GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives.Count; j++)
                {
                    if (GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[j].Properties.OpticalDisc != null &&
                        GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[j] != GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i])
                    { opticalDiscs.Add(GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[j].Properties.OpticalDisc); }
                }

                Label partitionName = new Label
                {
                    Content = name,
                    FontSize = 16,
                };

                ComboBox comboBox = new ComboBox
                {
                    Height = 20,
                    FlowDirection = FlowDirection.LeftToRight,
                    Tag = GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i],
                };
                comboBox.SelectionChanged += OpticalDisc_SelectionChanged;

                Button runButton = new Button
                {
                    Background = new SolidColorBrush(Colors.Green),
                    Tag = Properties.Resources.Launch,
                    Width = 20,
                    FlowDirection = FlowDirection.LeftToRight,
                };
                runButton.Click += OpenDisc_Click;

                Button pullOutButton = new Button
                {
                    Background = new SolidColorBrush(Colors.Red),
                    Tag = Properties.Resources.PullOut,
                    Width = 20,
                    FlowDirection = FlowDirection.LeftToRight,
                };

                DockPanel discPanel = new DockPanel()
                {
                    FlowDirection = FlowDirection.RightToLeft,
                };

                int index = -1;
                ComboBoxItem cmbItem = null;
                for (int od = 0; od < GameEnvironment.Items.OpticalDiscs.Count; od++)
                {
                    bool add = true;
                    //int added = -1;

                    for (int tod = 0; tod < opticalDiscs.Count; tod++)
                    {
                        if (GameEnvironment.Items.OpticalDiscs[od] == opticalDiscs[tod]) { add = false; break; }
                    }
                    
                    if (add) { cmbItem = new ComboBoxItem { Content = GameEnvironment.Items.OpticalDiscs[od].Name, Tag = GameEnvironment.Items.OpticalDiscs[od] }; comboBox.Items.Add(cmbItem); }
                    if (GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.OpticalDisc != null && GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives[i].Properties.OpticalDisc == GameEnvironment.Items.OpticalDiscs[od]) { index = od; }
                }
                if (index >= 0 && cmbItem != null) { comboBox.SelectedItem = cmbItem; }

                discPanel.Children.Add(pullOutButton);
                discPanel.Children.Add(runButton);
                discPanel.Children.Add(comboBox);         

                Label freeSpace = new Label
                {
                    //Content = Math.Round(free_space.Key, 2) + " " + free_space.Value + " " + Properties.Resources.Free.ToLower() + " " + Properties.Resources.Of.ToLower() + " " + Math.Round(space.Key, 2) + " " + space.Value,
                    FontSize = 8,
                };

                StackPanel stackPanel = new StackPanel
                {
                    
                };

                stackPanel.Children.Add(partitionName);
                stackPanel.Children.Add(discPanel);
                //stackPanel.Children.Add(freeSpace);

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

            if (initialize) { initialize = false; }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public static T GetChildOfType<T>(DependencyObject depObj)
        where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void OpticalDisc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (initialize) { return; }
            ComboBox comboBox = (sender as ComboBox);
            if (comboBox.SelectedItem != null)
            {
                OpticalDisc opticalDisc = ((comboBox.SelectedItem as ComboBoxItem).Tag as OpticalDisc);
                (comboBox.Tag as OpticalDrive).Properties.OpticalDisc = opticalDisc;
                Drawing();
            }
        }

            private void OpenDisc_Click(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button);
            var parent = FindParent<DockPanel>(button);
            if (parent == null) { return; }
            if (parent.Tag is OpticalDisc)
            {
                OpticalDisc opticalDisc = (parent.Tag as OpticalDisc);
                GameMessageBox.Show(opticalDisc.Name);
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
