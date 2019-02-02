using Computer_Era.Game.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Computer_Era.Game
{

    [ComVisible(true)]
    public class MapReader
    {
        object map;

        public MapReader(object sender)
        {
            map = sender;
        }
        
        public void ReadState(string obj)
        {
            if (map is Map)
            {
                (map as Map).TransitionProcessing(obj);
            }
        }
    }

    class DataReader
    {
    }
}
