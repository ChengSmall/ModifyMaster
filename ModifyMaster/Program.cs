using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cheng.Json;
using Cheng.Texts;
using Cheng.Windows.Hooks;
using Cheng.Windows.Processes;

namespace Cheng.ModifyMaster
{
    static class Program
    {

        static void Start(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            InitArgs.Init(args);
            InitArgs.Args.StartBackCode();
            using (var form = new MainForm())
            {
                Application.Run(form);
            }
            InitArgs.Args.Close();
        }

        [STAThread]
        static void Main(string[] args)
        {
            Start(args);
        }

    }
}
