using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Cheng.Algorithm;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 选择进程的列表窗口
    /// </summary>
    public partial class ProcessListSelectForm : Form
    {

        public ProcessListSelectForm()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region code

        #region 释放

        private void f_disposeing(bool dispose)
        {
            //Dispose();
        }

        #endregion

        #region 初始化

        private void f_lastInit()
        {

        }

        private void f_init()
        {
            f_initPar();
            f_initListView();
            f_initButton();
        }

        private void f_initPar()
        {
            p_selectProID = 0;
            p_viewProList = new List<ProcessViewItem>(16);
            p_filterWin = false;
            p_comparer = new ComparerProcessIDLastForm();
            p_proViewComparer = new ComparerProcessViewIDLastForm();
        }

        private void f_initListView()
        {
            var list = Col_ListView;
            list.LargeImageList = new ImageList();
            list.SmallImageList = new ImageList();
            list.MouseDoubleClick += fe_ListView_MouseDoubleClick;
            var listw = list.Width;
            int bw = (int)((double)list.Width / 2);

            bw = Maths.Clamp(bw, 1, listw);
            col_columnHeader_pro.Width = bw;
            col_columnHeader_proID.Width = bw;

        }

        private void f_initButton()
        {
            Col_Button_OK.Click += fe_ButtonClick_OK;
            Col_Button_Cancel.Click += fe_ButtonClick_Cancel;
        }

        #endregion

        #region 参数

        private ComparerProcessViewIDLastForm p_proViewComparer;
        private ComparerProcessIDLastForm p_comparer;

        private List<ProcessViewItem> p_viewProList;

        /// <summary>
        /// 选择的进程ID
        /// </summary>
        private int p_selectProID;

        /// <summary>
        /// 是否筛选窗体进程
        /// </summary>
        private bool p_filterWin;

        #endregion

        #region 控件参数访问

        /// <summary>
        /// 进程显示列表
        /// </summary>
        private ListView Col_ListView
        {
            get => col_listView;
        }

        /// <summary>
        /// 对话框确定按钮
        /// </summary>
        private Button Col_Button_OK
        {
            get => col_button_ok;
        }

        /// <summary>
        /// 对话框取消按钮
        /// </summary>
        private Button Col_Button_Cancel
        {
            get => col_button_cancel;
        }

        #endregion

        #region 事件注册

        #region 列表事件

        private void fe_ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var buts = e.Button;

            if ((buts & MouseButtons.Left) == MouseButtons.Left)
            {
                try
                {
                    var ses = Col_ListView.SelectedIndices;
                    if (ses.Count > 0)
                    {
                        var pro = p_viewProList[ses[0]];
                        SelectSetIDAndRet(pro.id);
                    }
                }
                catch (Exception ex)
                {
                    InitArgs.Args.DebugPrintLineF(InitArgs.ExceptionText(ex));
                }
            }

        }

        private void fe_ButtonClick_OK(object sender, EventArgs e)
        {
            try
            {
                var ses = Col_ListView.SelectedIndices;
                if (ses.Count > 0)
                {
                    var pro = p_viewProList[ses[0]];
                    SelectSetIDAndRet(pro.id);
                }
                else
                {
                    MessageBox.Show(this, "错误", "你还未选择进程", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                //InitArgs.Args.DebugPrintLineF(InitArgs.ExceptionText(ex));
            }
        }

        private void fe_ButtonClick_Cancel(object sender, EventArgs e)
        {
            p_selectProID = 0;
            DialogResult = DialogResult.Cancel;
        }

        #endregion

        #endregion

        #region 功能

        public void SelectSetIDAndRet(int id)
        {
            p_selectProID = id;
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 是否仅筛选有窗体的进程
        /// </summary>
        public bool FilterWindowsProcess
        {
            get => p_filterWin;
            set => p_filterWin = value;
        }

        static ListViewItem f_createProItem(ProcessViewItem item, int index)
        {
            ListViewItem vi = new ListViewItem(item.name);

            //System.Drawing.Icon addIcon;
            //if(item.icon is null)
            //{
            //    addIcon = System.Drawing.SystemIcons.Application;
            //}
            //else
            //{
            //    addIcon = item.icon;
            //}

            //vi.ImageList.Images.Add("icon", addIcon);
            vi.SubItems.Add(item.id.ToString());
            //vi.ImageKey = index.ToString();

            return vi;
        }

        private void f_RefreshProcessList()
        {
            var fw = FilterWindowsProcess;
            var enr = ProcessCheckGets.GetUserProcesses(fw);
            //Col_ListView.Items.Clear();
            p_viewProList.Clear();
            ProcessViewItem pro;
            foreach (var process in enr)
            {
                try
                {
                    pro = default;

                    pro.icon = null;
                    try
                    {
                        var proPath = process.MainModule.FileName;
                        pro.icon = System.Drawing.Icon.ExtractAssociatedIcon(proPath);
                    }
                    catch (Exception)
                    {
                        if(pro.icon != null)
                        {
                            pro.icon.Dispose();
                            pro.icon = null;
                        }
                    }

                    try
                    {
                        pro.name = process.MainModule.ModuleName;
                    }
                    catch (Exception)
                    {
                        pro.name = process.ProcessName;
                    }

                    pro.id = process.Id;

                    try
                    {
                        if (fw)
                        {
                            pro.isWindow = true;
                        }
                        else
                        {
                            pro.isWindow = process.MainWindowHandle != IntPtr.Zero;
                        }
                    }
                    catch (Exception)
                    {
                        pro.isWindow = false;
                    }

                    p_viewProList.Add(pro);
                }
                catch (Exception)
                {
                }
                finally
                {
                    process.Close();
                }

            }

            p_viewProList.Sort(p_proViewComparer);

            var items = Col_ListView.Items;
            Col_ListView.SmallImageList.Images.Clear();
            Col_ListView.LargeImageList.Images.Clear();
            items.Clear();
            int length = p_viewProList.Count;
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < length; i++)
            {
                pro = p_viewProList[i];
                try
                {
                    sb.Clear();
                    var item = f_createProItem(pro, i);

                    if (pro.icon is null)
                    {
                        pro.icon = System.Drawing.SystemIcons.Application;
                    }

                    try
                    {
                        Col_ListView.LargeImageList.Images.Add(i.ToString(), pro.icon);
                    }
                    catch (Exception ex)
                    {
                        sb.Append("添加大图标时出错 ");
                        sb.Append("name:");
                        sb.Append(pro.name);
                        sb.Append(" id:");
                        sb.AppendLine(pro.id.ToString());
                        InitArgs.PrintException(ex, sb);
                    }
                    try
                    {
                        Col_ListView.SmallImageList.Images.Add(i.ToString(), pro.icon);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine();
                        sb.Append("添加小图标时出错 ");
                        sb.Append("name:");
                        sb.Append(pro.name);
                        sb.Append(" id:");
                        sb.AppendLine(pro.id.ToString());
                        InitArgs.PrintException(ex, sb);
                    }
                    if(sb.Length != 0)
                    {
                        InitArgs.Args.DebugPrintLineF(sb.ToString());
                    }
                    //Col_ListView.LargeImageList.Images.Add(i.ToString(), pro.icon);
                    items.Add(item);
                    item.ImageIndex = i;
                }
                catch (Exception)
                {

                }
             
            }

        }

        /// <summary>
        /// 弹出当前对话框并返回选择的进程ID（在外部线程调用）
        /// </summary>
        /// <param name="win32"></param>
        /// <returns>进程ID，取消选择返回0</returns>
        public int ShowSelectProDialog(IWin32Window win32)
        {
            f_RefreshProcessList();
            var re = this.ShowDialog(win32);
            if(re == DialogResult.OK)
            {
                return p_selectProID;
            }
            return 0;
        }

        #endregion

        #region 派生

        #endregion

        #endregion

    }
}
