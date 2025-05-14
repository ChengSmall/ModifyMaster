using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.ModifyMaster
{

    public struct ProcessViewItem
    {

        public ProcessViewItem(string name, int id)
        {
            this.icon = null;
            this.name = name;
            this.id = id;
            this.isWindow = false;
        }

        public ProcessViewItem(Icon icon, string name, int id)
        {
            this.icon = icon;
            this.name = name;
            this.id = id;
            this.isWindow = false;
        }

        public Icon icon;
        public string name;
        public int id;
        public bool isWindow;
    }

}
