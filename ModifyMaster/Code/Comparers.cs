using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 按进程ID排序进程对象
    /// </summary>
    public sealed class ComparerProcessID : Comparer<Process>
    {

        public override int Compare(Process x, Process y)
        {
            if (x == (object)y) return 0;

            if(x is null)
            {
                return y is null ? 0 : -1;
            }
            else if(y is null)
            {
                return x is null ? 0 : 1;
            }

            return x.Id.CompareTo(y.Id);

        }
    }

    public sealed class ComparerProcessViewIDLastForm : Comparer<ProcessViewItem>
    {

        public override int Compare(ProcessViewItem x, ProcessViewItem y)
        {
            
            if (x.isWindow)
            {
                if (y.isWindow)
                {
                    return x.id.CompareTo(y.id);
                }
                else
                {
                    // x有窗 y无窗
                    return -1;
                }
            }
            else
            {
                if (y.isWindow)
                {
                    //x无窗 y有窗
                    return 1;
                }
                else
                {
                    //都有窗
                    return x.id.CompareTo(y.id);
                }
            }

        }
    }

    /// <summary>
    /// 按进程ID排序，但是窗体进程总是在前
    /// </summary>
    public sealed class ComparerProcessIDLastForm : Comparer<Process>
    {

        public override int Compare(Process x, Process y)
        {
            if (x == (object)y) return 0;

            if (x is null)
            {
                return y is null ? 0 : -1;
            }
            else if (y is null)
            {
                return x is null ? 0 : 1;
            }

            var xw = x.MainWindowHandle;
            var yw = y.MainWindowHandle;

            if(xw == IntPtr.Zero)
            {
                if(yw == IntPtr.Zero)
                {
                    return x.Id.CompareTo(y.Id);
                }
                else
                {
                    // x无窗 y有窗
                    return 1;
                }
            }
            else
            {
                if (yw == IntPtr.Zero)
                {
                    //x有窗 y无窗
                    return -1;
                }
                else
                {
                    //都有窗
                    return x.Id.CompareTo(y.Id);
                }
            }

        }
    }

    /// <summary>
    /// Keys排序器
    /// </summary>
    public sealed class ComparerKeys : Comparer<Keys>
    {
        public override int Compare(Keys x, Keys y)
        {
            return ((int)x).CompareTo((int)y);
        }
    }

    /// <summary>
    /// 模块按地址排序
    /// </summary>
    public unsafe sealed class ModuleComparer : Comparer<ProcessModule>
    {

        public override int Compare(ProcessModule x, ProcessModule y)
        {
            if (x == (object)y) return 0;

            if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }

            var xd = (void*)x.BaseAddress;
            var yd = (void*)y.BaseAddress;

            return xd < yd ? -1 : (xd == yd ? 0 : 1);
        }
    }


}
