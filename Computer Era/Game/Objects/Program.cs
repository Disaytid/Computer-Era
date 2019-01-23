using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    class Program
    {
        public int Id;
        public string Name;
        public string ControlName;
        public string Description;
        public string IconName;
        public int Row;
        public int Column;

        public Program(int id, string name, string control_name, string description, string icon_name, int row, int column)
        {
            Id = id;
            Name = name;
            ControlName = control_name;
            Description = description;
            IconName = icon_name;
            Row = row;
            Column = column;
        }
    }
}
