using Computer_Era.Game.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    public class PlayerProfile
    {
        public string Name { get; set; }
        public JobCard Job { get; set; }
        public PlayerHouse House { get; set; }

        public PlayerProfile (string name)
        {
            Name = name;
        }
    }
}
