using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using static Computer_Era.Game.GameConverter;

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для OSInstallation.xaml
    /// </summary>
    public partial class OSInstallation : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        OpticalDisc OpticalDisc;
        Objects.OperatingSystem OperatingSystem;
        Collection<HDDPartition> HDDPartitions = new Collection<HDDPartition>();
        HDDPartition SelectedHDDPartition;
        Partition SelectedPartition;
        public OSInstallation(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;

            Collection<OpticalDisc> opticalDiscs = new Collection<OpticalDisc>();
            foreach (OpticalDrive opticalDrive in GameEnvironment.Computers.CurrentPlayerComputer.OpticalDrives)
            {
                if (opticalDrive.Properties.OpticalDisc != null)
                {
                    opticalDiscs.Add(opticalDrive.Properties.OpticalDisc);
                }
            }

            foreach (Game.Objects.OperatingSystem os in GameEnvironment.Items.AllOperatingSystems)
            {
                foreach (OpticalDisc opticalDisc in opticalDiscs)
                {
                    if (opticalDisc.Properties.OperatingSystem == os.Uid)
                    {
                        LabelInstall.Content = "Установка " + os.Name;
                        StartInstallButton.IsEnabled = true;
                        OpticalDisc = opticalDisc;
                        OperatingSystem = os;
                        return;
                    }
                }
            }
            LabelInstall.Content = "Файлы операционной системы не найдены!";
        }

        private void StartInstallButton_Click(object sender, RoutedEventArgs e)
        {
            InitialSetupScreen.Visibility = Visibility.Collapsed;
            SectionSetup.Visibility = Visibility.Visible;
            foreach (FileSystem fileSystem in OperatingSystem.Properties.FileSystems)
            {
                FileSystems.Items.Add(fileSystem);
            }
            if (FileSystems.Items.Count > 0) { FileSystems.SelectedIndex = 0; }
            LoadPartitionsList();
        }

        private void LoadPartitionsList()
        {
            HDDPartitions.Clear();
            int counter = 0;
            foreach (HDD hdd in GameEnvironment.Computers.CurrentPlayerComputer.HDDs)
            {
                int summVolume = 0;
                foreach (Partition lpartition in hdd.Properties.Partitions)
                {
                    summVolume += lpartition.Volume;
                }

                if (hdd.Properties.Volume - summVolume > 0)
                {
                    KeyValuePair<double, MediaCapacityUnits> convert_volume = СonversionToMore(MediaCapacityUnits.Kilobyte, hdd.Properties.Volume - summVolume);
                    HDDPartitions.Add(new HDDPartition("Не занятое место на диске " + counter + " (" + Math.Round(convert_volume.Key, 2) + " " + convert_volume.Value.ToString() + ")", counter, hdd, null));
                }

                int pcounter = 0;
                foreach (Partition partition in hdd.Properties.Partitions)
                {
                    KeyValuePair<double, MediaCapacityUnits> convert_volume = СonversionToMore(MediaCapacityUnits.Kilobyte, partition.Volume);
                    HDDPartitions.Add(new HDDPartition(partition.Name + " " + pcounter + " на диске " + counter + " (" + Math.Round(convert_volume.Key, 2) + " " + convert_volume.Value.ToString() + ")", counter, hdd, partition));
                    pcounter++;
                }
                counter++;
            }
            ListPartition.ItemsSource = null;
            ListPartition.Items.Clear();
            ListPartition.ItemsSource = HDDPartitions;
            ListPartition.Items.Refresh();
        }

        private void ListPartition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreatePartition.IsEnabled = false;
            PartitionVolume.IsEnabled = true;
            DeletePartition.IsEnabled = false;
            Formatting.IsEnabled = false;
            FileSystems.IsEnabled = false;
            Next.IsEnabled = false;

            if (ListPartition.SelectedItem != null)
            {
                HDDPartition partition = ListPartition.SelectedItem as HDDPartition;
                if (partition.Partition == null && partition.HDD.Properties.Volume > 0)
                {
                    int summVolume = 0;
                    foreach (Partition lpartition in partition.HDD.Properties.Partitions)
                    {
                        summVolume += lpartition.Volume;
                    }

                    CreatePartition.IsEnabled = true;
                    PartitionVolume.IsEnabled = true;
                    PartitionVolume.MinValue = 1;
                    PartitionVolume.MaxValue = partition.HDD.Properties.Volume - summVolume;
                    PartitionVolume.Value = partition.HDD.Properties.Volume - summVolume;
                    SelectedHDDPartition = partition;
                } else if (partition.HDD.Properties.Partitions.Count > 0 && partition.Partition != null) {
                    DeletePartition.IsEnabled = true;
                    if (partition.Partition.FileSystem == 0) { Formatting.IsEnabled = true; FileSystems.IsEnabled = true; }
                    if (partition.Partition.Volume >= (OperatingSystem.Properties.Size)) { Next.IsEnabled = true; }
                    SelectedHDDPartition = partition;
                }
            }
        }

        private void CreatePartition_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedHDDPartition != null)
            {
                int summVolume = 0;
                foreach (Partition lpartition in SelectedHDDPartition.HDD.Properties.Partitions)
                {
                    summVolume += lpartition.Volume;
                }

                if (summVolume + PartitionVolume.Value <= SelectedHDDPartition.HDD.Properties.Volume)
                {
                    Partition partition = new Partition
                    {
                        Name = Properties.Resources.Partition,
                        Volume = Convert.ToInt32(PartitionVolume.Value),
                    };
                    SelectedHDDPartition.HDD.Properties.Partitions.Add(partition);

                    LoadPartitionsList();
                } else {
                    GameMessageBox.Show("Создание раздела", "На диске нет столько свободного места!", GameMessageBox.MessageBoxType.Warning);
                }
            }
        }

        private void DeletePartition_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedHDDPartition != null && SelectedHDDPartition.Partition != null)
            {
                SelectedHDDPartition.HDD.Properties.Partitions.Remove(SelectedHDDPartition.Partition);
                LoadPartitionsList();
            }
        }

        private void Formatting_Click(object sender, RoutedEventArgs e)
        {
            if (FileSystems.SelectedItem != null)
            {
                SelectedHDDPartition.Partition.FileSystem = (FileSystem)FileSystems.SelectedItem;
                GameMessageBox.Show("Форматирование", "Раздел успешно отформатирован в файловую систему " + (FileSystem)FileSystems.SelectedItem, GameMessageBox.MessageBoxType.Information);
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedHDDPartition != null && SelectedHDDPartition.Partition != null)
            {
                if (SelectedHDDPartition.Partition.FileSystem == 0) { SelectedHDDPartition.Partition.FileSystem = OperatingSystem.Properties.FileSystems[0]; }
                SelectedPartition = SelectedHDDPartition.Partition;

                LabelProcessInstall.Content = LabelInstall.Content;
                SectionSetup.Visibility = Visibility.Collapsed;
                InstallationProcess.Visibility = Visibility.Visible;
                InstallationProgress.Maximum = 100;
                int minutes = Convert.ToInt32((OperatingSystem.Properties.Size * (OpticalDisc.Properties.ReadSpeed * 150)) / 60);
                double speedInstall = (OpticalDisc.Properties.ReadSpeed * 150) / 60;

                GameEnvironment.GameEvents.Events.Add(new GameEvent("InstallOS", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddMinutes(minutes), Periodicity.Minute, minutes, InstallOS, true));
            }
        }

        private void InstallOS(GameEvent @event)
        {
            if (InstallationProgress.Value + 1 < 100)
            {
                InstallationProgress.Value++;
            } else {
                InstallationProgress.Value = 100;
                GameEnvironment.GameEvents.Events.Remove(@event);

                SelectedPartition.OperatingSystem = OperatingSystem;
                GameEnvironment.Computers.CurrentPlayerComputer.IsEnable = false;
                GameEnvironment.GameEvents.Events.Add(new GameEvent("RebootComputer", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddHours(1), Periodicity.Hour, 1, RebootComputer));
            }
        }

        private void RebootComputer(GameEvent @event)
        {
            GameEnvironment.Computers.CurrentPlayerComputer.IsEnable = true;
        }
    }

    internal class HDDPartition
    {
        public string Caption { get; set; }
        public int DiscNumber { get; set; }
        public HDD HDD { get; set; }
        public Partition Partition {get; set; } 

        public HDDPartition(string caption, int number, HDD hdd, Partition partition)
        {
            Caption = caption;
            DiscNumber = number;
            HDD = hdd;
            Partition = partition;
        }
    }
}
