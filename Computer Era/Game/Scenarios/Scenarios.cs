using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Scenarios
{
    public interface IScenario
    {
        string Name { get; set; }
        void Start(object sender, GameEnvironment gameEnvironment);
        void GameOver(string cause);
    }
}
