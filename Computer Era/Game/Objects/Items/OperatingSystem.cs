using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    public class OperatingSystemProperties
    {
        public string Description { get; set; }
        public string Author { get; set; }
        public int[] Programms { get; set; }
        public int Size { get; set; } //В килобайтах
        public Collection<FileSystem> FileSystems { get; set; } = new Collection<FileSystem>();
    }

    public class OperatingSystem : Item<OperatingSystemProperties>
    {
        public OperatingSystem(int uid, string name, string type, int price, DateTime man_date, OperatingSystemProperties properties) : base(uid, name, type, price, man_date, properties) { }
    }
}
