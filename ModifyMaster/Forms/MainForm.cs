using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cheng.Windows.Forms;
using Cheng.Windows.Processes;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 主窗口
    /// </summary>
    public partial class MainForm : Form
    {

        public MainForm()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region code

        #region 全局参数

        static MainForm sp_mainForm;

        /// <summary>
        /// 获取单例
        /// </summary>
        public static MainForm Main
        {
            get => sp_mainForm;
        }

        #endregion

        #region 初始化

        private void f_lastInit()
        {
            sp_mainForm = this;
        }

        private void f_init()
        {
            f_initPar();

            f_initTimer();

            f_initMenuCol();

            f_initTitle();

            f_initListView();

            f_initDialog();

        }

        private void f_initPar()
        {
            p_winMinSize = false;
            p_inputDialog = new InputValueDialog();
            p_inputDialog.Title = "输入进程ID";
            p_inputDialog.ButtonOkText = "确定";
            p_inputDialog.ButtonCancelText = "取消";
            p_strBuffer = new StringBuilder();
            p_uiCheck_OpenProcess = false;
            Text = "万能改" + " v0.0.0 预发布版";
        }

        /// <summary>
        /// 初始化菜单栏控件
        /// </summary>
        private void f_initMenuCol()
        {

            Col_MenuItem_File_Exit.Click += fe_MenuItem_File_Exit_Click;
            Col_MenuItem_File_OpenConfig.Click += fe_MenuItem_File_OpenSetUp_Click;
            Col_MenuItem_File_OpenProcess.Click += fe_MenuItem_File_OpenProcess_Click;
            Col_MenuItem_File_OpenProcessByID.Click += fe_MenuItem_File_OpenProcessByID_Click;

        }

        /// <summary>
        /// 初始化计时器
        /// </summary>
        private void f_initTimer()
        {
            Col_ViewRefreshTimer.Interval = 500;
            Col_ViewRefreshTimer.Tick += fe_Timer_ViewRefresh;
        }

        /// <summary>
        /// 初始化视图参数
        /// </summary>
        private void f_initListView()
        {
            var list = Col_ListView;
            f_initView();
            f_updateOpenProcess();
            list.MouseDoubleClick += fe_ListView_DoubleClick;
            //list.RetrieveVirtualItem += fe_RetrieveVirtualItem;
            //list.VirtualMode = true;
            //list.VirtualListSize = 0;
            //var args = InitArgs.Args;
            //if (args.processModify.IsOpenSetup)
            //{
            //    list.VirtualListSize = args.processModify.ProcessModifys.ModifyList.Count;
            //}
        }

        private void f_initDialog()
        {
            var openFile = Col_OpenFileDialpg;
            openFile.Filter = "修改器配置文件|*.json;*.jsonc;*.txt|所有文件|*";
        }

        #endregion

        #region 参数

        private InputValueDialog p_inputDialog;

        private StringBuilder p_strBuffer;

        /// <summary>
        /// 窗体是否最小化
        /// </summary>
        private bool p_winMinSize;

        /// <summary>
        /// UI监控 - 进程是否打开
        /// </summary>
        private bool p_uiCheck_OpenProcess;

        #endregion

        #region 资源释放

        private void f_disposing(bool disposing)
        {
            if (disposing)
            {
                InitArgs.Args.Close();
            }
            
        }

        #endregion

        #region 控件访问

        #region 菜单栏

        #region 文件栏

        /// <summary>
        /// 菜单栏 - 文件 - 退出
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_Exit
        {
            get => col_menuItem_File_Exit;
        }

        /// <summary>
        /// 菜单栏 - 文件 - 打开修改器配置文件
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_OpenConfig
        {
            get => col_menuItem_File_openSetup;
        }

        /// <summary>
        /// 菜单栏 - 文件 - 选择打开一个进程
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_OpenProcess
        {
            get => col_menuItem_File_openProcess;
        }

        /// <summary>
        /// 菜单栏 - 文件 - 打开指定ID进程
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_OpenProcessByID
        {
            get => col_menuItem_File_openProcessByID;
        }

        #endregion

        #region 设置栏

        #endregion

        #endregion

        #region 修改器视图

        /// <summary>
        /// 标题文本标签
        /// </summary>
        private Label Col_Label_Title
        {
            get => col_label_title;
        }

        /// <summary>
        /// 标题 - 是否打开进程文本标签
        /// </summary>
        private Label Col_Label_ProcessName
        {
            get => col_label_ifOpenProcessText;
        }

        /// <summary>
        /// 修改器视图
        /// </summary>
        private ListView Col_ListView
        {
            get => col_listView;
        }

        #endregion

        #region 计时器

        /// <summary>
        /// 视图刷新脚本执行计时器
        /// </summary>
        private Timer Col_ViewRefreshTimer
        {
            get => col_timer;
        }

        #endregion

        #region 对话框

        /// <summary>
        /// 控件 - 打开文件的对话框
        /// </summary>
        private OpenFileDialog Col_OpenFileDialpg
        {
            get => col_openFileDialog;
        }

        #endregion

        #endregion

        #region 功能

        #region 弹窗

        /// <summary>
        /// 弹出错误窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void ShowMegError(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 弹出消息窗口
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public void ShowMegInf(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region 更新视图

        const string openToggle = "√";
        const string closeToggle = "×";
        const string updateToggle = "■";

        static string f_toDataTypeText(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Int32:
                    return "32位整数";
                case DataType.UInt32:
                    return "32位正整数";
                case DataType.Int64:
                    return "64位整数";
                //case DataType.UInt64:
                //    return "64位正整数";
                case DataType.Float:
                    return "单浮点";
                case DataType.Double:
                    return "双浮点";
                case DataType.Int16:
                    return "16位整数";
                case DataType.UInt16:
                    return "16位正整数";
                case DataType.Byte:
                    return "字节值";
                default:
                    return "ERROR";
            }
        }

        /// <summary>
        /// 初始化标题
        /// </summary>
        private void f_initTitle()
        {
            var args = InitArgs.Args;

            if (args.processModify.IsOpenSetup)
            {
                //Text = "万能改" + " v0.0.0 预发布版" + " " + args.processModify.Inf_Name;
                StringBuilder sb = p_strBuffer;
                sb.Clear();
                var inf = args.processModify.ModifyInformation;
                bool havName = !string.IsNullOrEmpty(inf.name);
                bool havAuthor = !string.IsNullOrEmpty(inf.author);
                //bool havSyn = !string.IsNullOrEmpty(inf.synopsis);

                if (havName)
                {
                    sb.Append(inf.name);
                    
                }
                if (havAuthor)
                {
                    if(havName) sb.Append("  作者:");
                    sb.Append(inf.author);
                }
                var text = Col_Label_Title.Text;
                var us = sb.ToString();
                if(text != us)
                {
                    Col_Label_Title.Text = us;
                }
            }
            else
            {
                
                Col_Label_Title.Text = "未打开修改配置";
            }

        }

        /// <summary>
        /// 初始化修改项到视图
        /// </summary>
        private void f_initView()
        {
            var args = InitArgs.Args;

            if (args.processModify.IsOpenSetup)
            {
                var proMods = args.processModify.ProcessModifys;
                var list = proMods.ModifyList;

                var listView = Col_ListView;
                var viewItems = listView.Items;
                viewItems.Clear();
                lock (list)
                {
                    //□ ■ √ ×
                    //const string openToggle = "√";
                    //const string closeToggle = "×";

                    int length = list.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var mod = list[i];
                        if (mod is null) continue;

                        ListViewItem vs;
                        vs = CreateViewItem(mod);
                        viewItems.Add(vs);
                    }

                }

            }

        }

        /// <summary>
        /// 更新修改器UI视图
        /// </summary>
        private void f_updateView()
        {
            var args = InitArgs.Args;

            if (args.processModify.IsOpenSetup)
            {
                var proMods = args.processModify.ProcessModifys;
                var list = proMods.ModifyList;

                var viewItems = Col_ListView.Items;

                //if (Col_ListView.VirtualMode)
                //{
                //    if (list.Count != Col_ListView.VirtualListSize)
                //    {
                //        Col_ListView.VirtualListSize = list.Count;
                //    }
                //    Col_ListView.Refresh();
                //    return;
                //}

                if (list.Count != viewItems.Count)
                {
                    //数量不一致
                    //刷新视图
                    f_initView();
                    return;
                }

                lock (list)
                {
                    //□ ■ √ ×
                    //const string openToggle = "√";
                    //const string closeToggle = "×";
                    try
                    {

                        int length = list.Count;
                        for (int i = 0; i < length; i++)
                        {
                            var mod = list[i];
                            if (mod is null) continue;

                            var vitem = viewItems[i];

                            //项名
                            //固定

                            //开关 sub1
                            string tog;

                            switch (mod.ModifyType)
                            {
                                case ModifyAddressType.Add:
                                case ModifyAddressType.Sub:
                                case ModifyAddressType.Once:
                                    tog = updateToggle;
                                    break;
                                default:
                                    tog = mod.Toggle ? openToggle : closeToggle;
                                    break;
                            }

                            if (vitem.SubItems[1].Text != tog)
                            {
                                vitem.SubItems[1].Text = tog;
                            }

                            //修改类型 sub2
                            //固定

                            //值 sub3
                            string text;
                            if (string.IsNullOrEmpty(mod.ViewValue))
                            {
                                text = "[无]";
                            }
                            else
                            {
                                text = mod.ViewValue;
                            }
                            if (vitem.SubItems[3].Text != text)
                            {
                                vitem.SubItems[3].Text = text;
                            }

                            //条件 sub4
                            if (vitem.SubItems[4].Text != mod.ViewCondition)
                            {
                                vitem.SubItems[4].Text = mod.ViewCondition;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        //不太可能出现这种bug🤔
                        StringBuilder sb = new StringBuilder(64);
                        InitArgs.PrintException(ex, sb);
                        args.DebugPrintLineF(sb.ToString());
                    }
                   
                }


            }

        }

        /// <summary>
        /// 从修改条目创建视图项
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        private static ListViewItem CreateViewItem(ModifyItem mod)
        {
            if (mod is null) return null;
            ListViewItem vs;
            ListViewItem.ListViewSubItem sub;
            string text;

            //项名
            text = mod.Text;
            vs = new ListViewItem(text);
            vs.Name = mod.KeyName;

            //sub = new ListViewItem.ListViewSubItem(vs, text);
            //sub.Name = text;
            //sub.Text = text;
            //vs.SubItems.Add(sub);

            //开关

            switch (mod.ModifyType)
            {
                case ModifyAddressType.Add:
                case ModifyAddressType.Sub:
                case ModifyAddressType.Once:
                    text = updateToggle;
                    break;
                default:
                    text = mod.Toggle ? openToggle : closeToggle;
                    break;
            }

            //text = mod.Toggle ? openToggle : closeToggle;
            sub = new ListViewItem.ListViewSubItem();
            //sub.Name = text;
            sub.Text = text;
            vs.SubItems.Add(sub);


            //修改类型
            text = f_toDataTypeText(mod.AddressDataType);
            sub = new ListViewItem.ListViewSubItem();
            //sub.Name = text;
            sub.Text = text;
            vs.SubItems.Add(sub);

            //值
           
            if (string.IsNullOrEmpty(mod.ViewValue))
            {
                text = "[无]";
            }
            else
            {
                text = mod.ViewValue;
            }
            sub = new ListViewItem.ListViewSubItem();
            //sub.Name = text;
            sub.Text = text;
            vs.SubItems.Add(sub);

            //条件
            
            text = mod.ViewCondition;
            sub = new ListViewItem.ListViewSubItem();
            //sub.Name = text;
            sub.Text = text;
            vs.SubItems.Add(sub);
            //viewItems.Add();

            return vs;
        }

        /// <summary>
        /// 更新打开进程标签UI
        /// </summary>
        private void f_updateOpenProcess()
        {
            var args = InitArgs.Args;
            //p_uiCheck_OpenProcess = InitArgs.Args.processModify.IsOpenProcess;
            if (InitArgs.Args.processModify.IsOpenProcess)
            {
                //打开进程
                StringBuilder sb = p_strBuffer;
                sb.Clear();
                sb.Append("进程: ");
                sb.Append(args.processModify.OpenProcessName);
                sb.Append(' ');
                sb.Append(args.processModify.OpenProcessID);

                var text = Col_Label_ProcessName.Text;
                var us = sb.ToString();
                if(text != us)
                {
                    Col_Label_ProcessName.Text = us;
                }
            }
            else
            {
                Col_Label_ProcessName.Text = "未打开进程";
            }

        }

        /// <summary>
        /// 暂停运行 视图更新事件计时器
        /// </summary>
        private void StopViewTimer()
        {
            Col_ViewRefreshTimer.Stop();
        }

        /// <summary>
        /// 开始运行 视图更新事件计时器
        /// </summary>
        private void StartViewTimer()
        {
            Col_ViewRefreshTimer.Start();
        }

        #endregion

        #region 选择 打开

        /// <summary>
        /// 选择配置文件并重新初始化
        /// </summary>
        private void f_showSelectSetupFile()
        {
            var openFile = Col_OpenFileDialpg;

            openFile.Title = "选择一个修改配置";

            var re = openFile.ShowDialog(this);

            if (re == DialogResult.OK)
            {
                var path = openFile.FileName;

                if (File.Exists(path))
                {
                    try
                    {
                        var error = InitArgs.Args.processModify.InitByFile(path);
                        if (error == ProcessModify.InitSetupError.FileReadError)
                        {
                            ShowMegError("错误", "配置文件不是json格式");
                        }
                        else if (error == ProcessModify.InitSetupError.JsonError)
                        {
                            ShowMegError("错误", "json文件不符合修改器配置参数格式");
                        }
                        else
                        {
                            f_initTitle();
                            f_initView();
                            f_updateOpenProcess();
                        }
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder(32);
                        InitArgs.PrintException(ex, sb);
                        InitArgs.Args.DebugPrintLineF(sb.ToString());
                    }
                    
                }
                else
                {
                    ShowMegError("错误", "给定文件不存在");
                }
            }

        }

        /// <summary>
        /// 弹出选择一个进程打开
        /// </summary>
        private void f_showOpenProcess()
        {
            int re;
            try
            {
                
                using (ProcessListSelectForm pform = new ProcessListSelectForm())
                {
                    pform.FilterWindowsProcess = true;
                    pform.Font = this.Font;
                    re = pform.ShowSelectProDialog(this);
                }

                if(re != 0)
                {
                    var args = InitArgs.Args;
                    if (args.processModify.OpenProcessByID(re))
                    {
                        f_updateOpenProcess();
                        //p_uiCheck_OpenProcess = true;
                        //p_uiCheck_proCloseBuf = false;
                    }
                }
                
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(64);
                InitArgs.PrintException(ex, sb);
                InitArgs.Args.DebugPrintLineF(sb.ToString());
            }
            finally
            {
                GC.Collect();
            }
            
        }

        /// <summary>
        /// 弹出一个输入框输入进程ID
        /// </summary>
        private void f_showInputIDProcess()
        {
            var b = p_inputDialog.ShowInputInt32Dialog(this, out int id);

            if (b.HasValue)
            {
                if (b.Value)
                {
                    var args = InitArgs.Args;
                    try
                    {
                        //Cheng.Windows.Processes.WinAPI.CloseHandle

                        if (args.processModify.OpenProcessByID(id))
                        {
                            //p_uiCheck_OpenProcess = true;
                            //p_uiCheck_proCloseBuf = false;
                            f_updateOpenProcess();
                        }
                        else
                        {
                            ShowMegError("错误", "未打开进程");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMegError("错误", InitArgs.ExceptionText(ex));
                    }
                   
                }
                else
                {
                    ShowMegError("错误", "输入的ID不是数字");
                }
            }
            
        }

        #endregion

        #endregion

        #region 事件注册

        #region 公共事件

        /// <summary>
        /// 公共事件 - 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_ExitApp(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 计时器

        /// <summary>
        /// UI更新计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_Timer_ViewRefresh(object sender, EventArgs e)
        {

            if ((InitArgs.Args.processModify.IsOpenProcess) && (!p_uiCheck_OpenProcess))
            {
                p_uiCheck_OpenProcess = true;
                f_updateOpenProcess();
            }

            if((!InitArgs.Args.processModify.IsOpenProcess) && (p_uiCheck_OpenProcess))
            {
                p_uiCheck_OpenProcess = false;
                f_updateOpenProcess();
            }

            //if (p_uiCheck_OpenProcess)
            //{
            //    p_uiCheck_OpenProcess = false;
            //}
            f_updateView();
        }

        #endregion

        #region 菜单栏

        #region 文件菜单栏

        /// <summary>
        /// 打开配置文件选项 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_MenuItem_File_OpenSetUp_Click(object sender, EventArgs e)
        {
            StopViewTimer();
            f_showSelectSetupFile();
            StartViewTimer();
        }

        /// <summary>
        /// 打开进程选项 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_MenuItem_File_OpenProcess_Click(object sender, EventArgs e)
        {
            StopViewTimer();
            f_showOpenProcess();
            StartViewTimer();
        }

        /// <summary>
        /// 打开进程ID选项 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_MenuItem_File_OpenProcessByID_Click(object sender, EventArgs e)
        {
            StopViewTimer();
            f_showInputIDProcess();
            StartViewTimer();
        }

        /// <summary>
        /// 退出选项 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_MenuItem_File_Exit_Click(object sender, EventArgs e)
        {
            fe_ExitApp(sender, e);
        }

        #endregion

        #region

        #endregion

        #endregion

        #region 列表视图

        private void fe_ListView_DoubleClick(object sender, MouseEventArgs e)
        {
            var buts = e.Button;
            
            if((buts & MouseButtons.Left) == MouseButtons.Left)
            {

                var list = Col_ListView;

                if (list.SelectedIndices.Count > 0)
                {
                    var args = InitArgs.Args;
                    if (args.processModify.IsOpenSetup)
                    {
                        try
                        {
                            var index = list.SelectedIndices[0];
                            var mods = args.processModify.ProcessModifys;
                            var mod = mods.ModifyList[index];
                            lock (mod)
                            {
                                mod.ToggleOnActive();
                                //mod.ModifyType;
                                switch (mod.ModifyType)
                                {
                                    case ModifyAddressType.Add:
                                    case ModifyAddressType.Sub:
                                    case ModifyAddressType.Once:
                                        args.PlayAudioCheckToggle(true);
                                        break;
                                }
                                f_updateView();
                            }
                        }
                        catch (Exception ex)
                        {
                            StringBuilder sb = new StringBuilder(64);
                            InitArgs.PrintException(ex, sb);
                            args.DebugPrintLineF(sb.ToString());
                        }

                    }


                }

            }

        }

        private void fe_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var args = InitArgs.Args;

            if (args.processModify.IsOpenSetup)
            {
                //args.DebugPrintLine("once Update");
                var mods = args.processModify.ProcessModifys;

                if (mods.ModifyList.Count != Col_ListView.VirtualListSize)
                {
                    Col_ListView.VirtualListSize = mods.ModifyList.Count;
                }

                if (e.ItemIndex >= mods.ModifyList.Count)
                {
                    e.Item = null;
                    return;
                }

                var mod = mods.ModifyList[e.ItemIndex];
                e.Item = CreateViewItem(mod);
            }
            else
            {
                Col_ListView.VirtualListSize = 0;
                e.Item = null;
            }

        }

        #endregion

        #endregion

        #region 派生

        #region 窗口大小

        /// <summary>
        /// 窗体退出大小调整模式时发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.WindowState == FormWindowState.Minimized)
            {
                if (!p_winMinSize)
                {
                    p_winMinSize = true;
                    StopViewTimer();
                    InitArgs.Args.DebugPrintLineF("窗体最小化 暂停UI更新计时器");
                }

            }
            else
            {
                if (p_winMinSize)
                {
                    p_winMinSize = false;
                    StartViewTimer();
                    InitArgs.Args.DebugPrintLineF("窗体结束最小化 开启UI更新计时器");
                }
                
            }
        }

        #endregion

        #region 窗口打开和关闭

        /// <summary>
        /// 在第一次显示窗体前发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// 首次显示窗体时发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            Col_ViewRefreshTimer.Start();

            base.OnShown(e);
        }

        /// <summary>
        /// 关闭窗体前发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopViewTimer();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// 关闭窗体后发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        /// <summary>
        /// 关闭窗体时发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        /// <summary>
        /// 关闭窗体时发生
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        #endregion

        #endregion

        #endregion

    }
}
