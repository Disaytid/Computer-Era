using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Computer_Era.Game.Scenarios
{
    class Main : IScenario
    {
        public string Name { get; set; } = Properties.Resources.MainScenario;

        private object main;
        GameEnvironment GameEnvironment;
        public void Start(object sender, GameEnvironment gameEnvironment)
        {
            main = sender;
            GameEnvironment = gameEnvironment;

            GameEnvironment.Money.PlayerCurrency[0].TopUp(Properties.Resources.MainScenarioPaymentName, Properties.Resources.MainScenarioPaymentInitiator, GameEnvironment.GameEvents.GameTimer.DateAndTime, 10000);

            GameEnvironment.GameEvents.GameTimer.Timer.Start();
        }
        public void GameOver(string cause)
        {
            GameEnvironment.Main.CauseText.Text = "Вы проиграли!" + Environment.NewLine + "Причина: " + cause;
            GameEnvironment.Main.GameOver.Visibility = Visibility.Visible;
        }
    }
}
