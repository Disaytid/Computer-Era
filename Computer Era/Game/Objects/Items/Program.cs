using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    public class ProgramProperties
    {
        public string IconName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string ControlName { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class Program : Item<ProgramProperties>
    {
        public Program(int uid, string name, string type, int price, DateTime man_date, ProgramProperties properties) : base(uid, name, type, price, man_date, properties) { }
    }
}
