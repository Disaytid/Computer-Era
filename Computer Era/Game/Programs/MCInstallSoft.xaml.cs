using Computer_Era.Game.Objects;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using OperatingSystem = Computer_Era.Game.Objects.OperatingSystem;

namespace Computer_Era.Game.Programs
{
    /// <summary>
    /// Логика взаимодействия для MCInstallSoft.xaml
    /// </summary>
    public partial class MCInstallSoft : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        public MCInstallSoft(GameEnvironment gameEnvironment, int[] programs)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;
            LoadProgramList(programs);
        }

        private void LoadProgramList(int[] programs)
        {
            Collection<Program> lprograms = new Collection<Program>();
            for (int program_a=0; GameEnvironment.Items.AllPrograms.Count > program_a; program_a++)
            {
                for (int program_b=0; programs.Length > program_b; program_b++)
                {
                    if (GameEnvironment.Items.AllPrograms[program_a].Uid == programs[program_b]) { lprograms.Add(GameEnvironment.Items.AllPrograms[program_a]); }
                    if (programs.Length == lprograms.Count) { goto AddToList; }
                }
            }
            AddToList: ProgramsList.ItemsSource = lprograms;
            ProgramsList.Items.Refresh();
        }
        Program program;
        private void InstallProgram_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (sender as Button);
            if (!(button.Tag is Program)) { return; }
            program = (button.Tag as Program);

            for (int i=0; GameEnvironment.Items.Programs.Count > i; i++)
            {
                if (GameEnvironment.Items.Programs[i].Uid == program.Uid ) { GameMessageBox.Show("У вас уже установлена эта программа!"); return; }
            }

            InstallationProgress.Minimum = 0;
            InstallationProgress.Maximum = 100;
            InstallationProgress.Value = 0;
            InstallationName.Content = Properties.Resources.Installation + " " + program.Name;
            List.Visibility = Visibility.Collapsed;
            Install.Visibility = Visibility.Visible;
            int minutes = 1;
            GameEnvironment.GameEvents.Events.Add(new GameEvent("InstallProgram", GameEnvironment.GameEvents.GameTimer.DateAndTime.AddMinutes(minutes), Periodicity.Minute, minutes, InstallProgram, true));
        }

        private void InstallProgram(GameEvent @event)
        {
            if (InstallationProgress.Value + 1 < 100)
            {
                InstallationProgress.Value++;
            } else {
                GameEnvironment.GameEvents.Events.Remove(@event);
                Install.Visibility = Visibility.Collapsed;
                List.Visibility = Visibility.Visible;
                program.Properties.Row = -1;
                program.Properties.Column = -1;
                GameEnvironment.Items.Programs.Add(program);
                GameEnvironment.Main.DrawDesktop();
                GameMessageBox.Show("Установка успешно произведена!");
            }
        }
    }
}
