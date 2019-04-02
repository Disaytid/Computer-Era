using Computer_Era.Game.Objects;
using System.Collections.ObjectModel;
using System.Windows.Controls;

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

        private void InstallProgram_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
