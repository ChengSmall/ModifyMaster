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

        static void test()
        {

            Console.WriteLine("任意键退出");
            Console.ReadKey(true);
        }

        static void Start(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            //InitArgs.Init(args);
            InitArgs.Args.StartBackCode();
            using (var form = new MainForm())
            {
                Application.Run(form);
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            //test();
            Start(args);
        }

    }
}
