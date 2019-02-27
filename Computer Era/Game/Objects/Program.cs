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
    }

    public class Program : Item<ProgramProperties>
    {
        public Program(int uid, string name, string type, int price, DateTime man_date, ProgramProperties properties) : base(uid, name, type, price, man_date, properties) { }
    }

    public class InstalledProgram : Program
    {
        public string ControlName { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public InstalledProgram(int uid, string name, string type, int price, DateTime man_date, ProgramProperties properties, string control_name, int row, int column) : base(uid, name, type, price, man_date, properties)
        {
            ControlName = control_name;
            Row = row;
            Column = column;
        }
    }
}
