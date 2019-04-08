using Computer_Era.Game.Objects;
using System.Windows;
using System.Windows.Controls;

namespace Computer_Era.Game.Programs
{
    /// <summary>
    /// Логика взаимодействия для GuessTheNumber.xaml
    /// </summary>
    public partial class GuessTheNumber : UserControl
    {
        readonly GameEnvironment GameEnvironment;
        private GameState gameState = GameState.newgame;
        public GuessTheNumber(GameEnvironment gameEnvironment, Program program)
        {
            InitializeComponent();
            GameEnvironment = gameEnvironment;
            Title.Text = program.Name;
        }

        enum GameState
        {
            newgame,
            game,
        }

        private int number = 0;
        private int attempt = 0;
        private void Game_Click(object sender, RoutedEventArgs e)
        {
            if (gameState == GameState.newgame)
            {
                number = GameEnvironment.Random.Next(1,101);
                Text.Text = "Введите число от 1 до 100 и нажмите кнопку 'Угадать'";
                Game.Content = "Угадать";
                Number.IsEnabled = true;
                gameState = GameState.game;
            } else if (gameState == GameState.game) {
                if (int.TryParse(Number.Text, out int num))
                {
                    if (num < 1 || num > 100) { GameMessageBox.Show("Введенное число не поподает в диапазон от 1 до 100"); return; }
                    attempt++;
                    NumberAttempt.Content = "Сделано попыток: " + attempt;
                    if (num < number) { Text.Text = "Загаданное число больше"; }
                    else if (num > number) { Text.Text = "Загаданное число меньше"; }
                    else
                    {
                        Text.Text = "Поздравляю, Вы угадали число с " + attempt + " попыток";
                        GameMessageBox.Show("Поздравляю, Вы угадали число с " + attempt + " попыток");
                        gameState = GameState.newgame;
                        number = 0;
                        attempt = 0;
                        Number.Text = string.Empty;
                        Number.IsEnabled = false;
                        Game.Content = "Играть";
                        Text.Text = "Поздравляю с победой, может сыграем еще раз?";
                        NumberAttempt.Content = "Сделано попыток: " + attempt;
                    }
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
