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

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для OSInstallation.xaml
    /// </summary>
    public partial class OSInstallation : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        Collection<HDDPartition> HDDPartitions = new Collection<HDDPartition>();
        public OSInstallation(GameEnvironment gameEnvironment)
        {
            InitializeComponent();

            GameEnvironment = gameEnvironment;
            Objects.OperatingSystem operatingSystem;

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
                        operatingSystem = os;
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

            int counter = 0;
            foreach (HDD hdd in GameEnvironment.Computers.CurrentPlayerComputer.HDDs)
            {
                if (hdd.Properties.Partitions.Count == 0)
                {
                    KeyValuePair<double, MediaCapacityUnits> convert_volume = СonversionToMore(MediaCapacityUnits.Kilobyte, hdd.Properties.Volume);
                    HDDPartitions.Add(new HDDPartition("Не занятое место на диске " + counter + " (" + Math.Round(convert_volume.Key, 2) + " " + convert_volume.Value.ToString() + ")", hdd, null));
                }
                counter++;
            }
            ListPartition.ItemsSource = HDDPartitions;
        }
    }

    internal class HDDPartition
    {
       public string Caption { get; set; }
       public  HDD HDD { get; set; }
       public  Partition Partition { get; set; }

        public HDDPartition(string caption, HDD hdd, Partition partition)
        {
            Caption = caption;
            HDD = hdd;
            Partition = partition;
        }
    }
}
