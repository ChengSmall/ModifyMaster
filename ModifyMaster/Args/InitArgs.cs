using Cheng.Json;
using Cheng.LoopThreads;
using Cheng.Memorys;
using Cheng.Streams.Parsers;
using Cheng.Streams.Parsers.Default;
using Cheng.Windows.Processes;
using Cheng.Windows.Hooks;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using Cheng.Json.GeneratorNumbers;
using Cheng.Algorithm.Collections;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// C#程序基本参数
    /// </summary>
    public sealed unsafe class InitArgs : Cheng.Memorys.SafreleaseUnmanagedResources
    {

        #region 单例

        private readonly static InitArgs sp_args = new InitArgs();

        /// <summary>
        /// 获取参数
        /// </summary>
        public static InitArgs Args
        {
            get
            {
                //if(sp_args is null)
                //{
                //    sp_args = new InitArgs();
                //}
                return sp_args;
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="args">命令行参数</param>
        public InitArgs()
        {
            //commandArgs = args;
            commandLine = Environment.CommandLine;

            currentDomain = AppDomain.CurrentDomain;

            applicationName = currentDomain.FriendlyName;

            isX64 = sizeof(void*) == 8;

            if(string.IsNullOrEmpty(applicationName))
            {
                appConfigName = null;
            }
            else
            {
                appConfigName = applicationName + ".config";
            }

            rootDirectory = currentDomain?.BaseDirectory;

            systemIsX64 = Environment.Is64BitOperatingSystem;

            comparerKeys = new ComparerKeys();
            processModify = new ProcessModify();
            p_hotKeysDown = new SortedSet<Keys>(comparerKeys);
            
            
            f_init();
        }

        private void f_initDebugFile()
        {
            debugLogPrint = null;
            try
            {
                if (appConfigXml is null) return;

                var root = appConfigXml.DocumentElement;
                bool debug = false;
                try
                {
                    var setup = root["setup"];
                    if (setup is null) goto XmlDEBUG;
                   
                    var xmlDebug = setup["debug"];
                    if (xmlDebug is null) goto XmlDEBUG;

                    if (xmlDebug.GetAttribute("value") == "true")
                    {
                        debug = true;
                    }
                }
                catch (Exception)
                {
                    debug = false;
                }

                XmlDEBUG:
                if (debug)
                {
                    StreamWriter swr;
                    var filePath = Path.Combine(this.rootDirectory, "debug.log");
                    swr = new StreamWriter(filePath, false, Encoding.UTF8, 1024 * 2);
                    debugLogPrint = swr;
                }
                //debugLogPrint = Console.Out;
            }
            catch (Exception)
            {
                debugLogPrint = null;
            }
            //debugLogPrint = Console.Out;
        }

        private void f_init()
        {
            f_initCommandLineArgs();
            tempTask = null;
            p_allHotKeys = new Dictionary<string, Keys>(100);
            f_initAllHotkeys();
            //p_keyMessageList = new Queue<KeyHook.KeyHookArgs>();
            jsonParser = new JsonParserDefault();
            //jsonValueGenerator = new JsonValueGenerator();
            //streamParser = new StreamParserDefault();
            try
            {
                appConfigXml = new XmlDocument();
                using (StreamReader sr = new StreamReader(Path.Combine(rootDirectory, appConfigName), Encoding.UTF8, true, 1024 * 2))
                {
                    appConfigXml.Load(sr);
                }
            }
            catch (Exception)
            {
                appConfigXml = null;
            }

            f_initDebugFile();
            p_audioEffects = new AudioEffects();
        }

        private void f_initCommandLineArgs()
        {

            var beginI = commandLine.IndexOf(this.applicationName);
            int i;
            int length = commandLine.Length;
            for (i = beginI; i < length; i++)
            {
                if (char.IsWhiteSpace(commandLine[i]))
                {
                    //属于空格
                    break;
                }
            }
            if(i >= length)
            {
                commandlineArgs = null;
                return;
            }

            for (; i < length; i++)
            {
                if (!char.IsWhiteSpace(commandLine[i]))
                {
                    //不属于空格
                    break;
                }
            }
            if(i >= length)
            {
                commandlineArgs = null;
                return;
            }
            commandlineArgs = commandLine.Substring(i);

        }

        #endregion

        #region 参数

        #region 只读参数

        /// <summary>
        /// 原始命令行
        /// </summary>
        public readonly string commandLine;

        /// <summary>
        /// 当前应用程序域
        /// </summary>
        public readonly AppDomain currentDomain;

        /// <summary>
        /// 程序根目录
        /// </summary>
        public readonly string rootDirectory;

        /// <summary>
        /// 该程序文件名称
        /// </summary>
        public readonly string applicationName;

        /// <summary>
        /// 该程序公共配置文件名称
        /// </summary>
        public readonly string appConfigName;

        /// <summary>
        /// 是否为64位环境
        /// </summary>
        public readonly bool isX64;

        /// <summary>
        /// 该程序的配置文件xml文档
        /// </summary>
        public XmlDocument appConfigXml;

        /// <summary>
        /// 忽略启动文件路径的命令行参数字符串，null或空表示没有参数
        /// </summary>
        public string commandlineArgs;

        /// <summary>
        /// 当前运行的操作系统是否为64位
        /// </summary>
        public readonly bool systemIsX64;

        #endregion

        #region 功能参数

        /// <summary>
        /// Keys排序器
        /// </summary>
        public readonly ComparerKeys comparerKeys;

        private Dictionary<string, Keys> p_allHotKeys;

        /// <summary>
        /// json解析器
        /// </summary>
        public JsonParser jsonParser;

        /// <summary>
        /// DEBUG日志打印时调用对象
        /// </summary>
        public TextWriter debugLogPrint;

        /// <summary>
        /// 临时任务通讯字段
        /// </summary>
        public Task tempTask;

        /// <summary>
        /// 修改循环
        /// </summary>
        private Thread p_modThread;

        //private Thread p_keyMegThread;

        public ModifyLoop p_loopFunction;

        /// <summary>
        /// 按键消息挂钩
        /// </summary>
        public KeyHook p_keyHook;

        private AudioEffects p_audioEffects;

        /// <summary>
        /// 进程操作台
        /// </summary>
        public readonly ProcessModify processModify;

        #endregion

        #region 热键缓存

        /// <summary>
        /// 热键集合
        /// </summary>
        private readonly SortedSet<Keys> p_hotKeysDown;

        #endregion

        #endregion

        #region 功能

        #region 参数

        /// <summary>
        /// 可用热键全集合
        /// </summary>
        public Dictionary<string, Keys> AllHotKeysName
        {
            get => p_allHotKeys;
        }

        /// <summary>
        /// 使用默认解析器访问或设置json解析器
        /// </summary>
        public JsonParserDefault JsonParserDef
        {
            get
            {
                return jsonParser as JsonParserDefault;
            }
            set
            {
                jsonParser = value;
            }
        }

        /// <summary>
        /// 全局键盘事件
        /// </summary>
        public event HookAction<KeyHook.KeyHookArgs> KeyEvent
        {
            add
            {
                if (p_keyHook == null) return;
                p_keyHook.KeyHookEvent += value;
            }
            remove
            {
                if (p_keyHook == null) return;
                p_keyHook.KeyHookEvent -= value;
            }
        }
        
        /// <summary>
        /// 按键消息处于按下的按键
        /// </summary>
        public SortedSet<Keys> HotKeysDown
        {
            get => p_hotKeysDown;

        }

        /// <summary>
        /// 将热键按下缓存按顺序写入新集合
        /// </summary>
        /// <param name="append"></param>
        public void AppendNowHotkeysDown(IList<Keys> append)
        {
            lock (p_hotKeysDown)
            {
                foreach (var item in p_hotKeysDown)
                {
                    append.Add(item);
                }
            }
        }

        #endregion

        #region 事件注册

        private void fe_keyHook(Hook hook, KeyHook.KeyHookArgs args)
        {
            if (args.IsSystemKey) return;

            lock (p_hotKeysDown)
            {
                if (args.State)
                {
                    p_hotKeysDown.Add(args.Keys);
                }
                else
                {
                    p_hotKeysDown.Remove(args.Keys);
                }
            }
        }

        #endregion

        #region debug

        /// <summary>
        /// 此程序是否有可用的debug
        /// </summary>
        public bool CanDebug
        {
            get => debugLogPrint != null;
        }

        /// <summary>
        /// 打印一行日志
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrintLine(string message)
        {
            debugLogPrint?.WriteLine(message);
        }

        /// <summary>
        /// 打印一段日志信息
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrint(string message)
        {
            debugLogPrint?.Write(message);
        }

        /// <summary>
        /// 清扫缓冲区
        /// </summary>
        public void DebugFlush()
        {
            debugLogPrint?.Flush();
        }

        /// <summary>
        /// 打印一行日志并清扫缓冲区
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrintLineF(string message)
        {
            if(debugLogPrint != null)
            {
                debugLogPrint.WriteLine(message);
                debugLogPrint.Flush();
            }
        }

        /// <summary>
        /// 打印一段日志信息并清扫缓冲区
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrintF(string message)
        {
            if (debugLogPrint != null)
            {
                debugLogPrint.Write(message);
                debugLogPrint.Flush();
            }
        }

        /// <summary>
        /// 将异常信息添加到字符串缓冲区
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="append"></param>
        public static void PrintException(Exception exception, StringBuilder append)
        {
            if (append is null)
            {
#if DEBUG
                throw new ArgumentNullException(nameof(append));
#else
                InitArgs.Args.DebugPrintLineF("调用PrintException时字符串缓冲区是null");
                return;
#endif
            }

            Exception ex = exception;
            for (int i = 0; i < 3; i++)
            {
                if (ex is null) break;
                append.Append("错误:");
                append.AppendLine(ex.Message);
                append.AppendLine("错误类型:" + ex.GetType().FullName);
                append.Append("堆栈跟踪:");
                append.Append(ex.StackTrace);
                ex = ex.InnerException;
                if(ex != null)
                {
                    append.AppendLine();
                    append.Append("内部异常");
                }
            }

        }

        /// <summary>
        /// 返回异常信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string ExceptionText(Exception exception)
        {
            StringBuilder sb = new StringBuilder(64);
            PrintException(exception, sb);
            return sb.ToString();
        }

        #endregion

        #region 功能

        #region 初始化

        private void f_initAudioEffect()
        {
            p_audioEffects.LoadAudioByDefPath();
        }

        private void f_initAllHotkeys()
        {
            #region
            int i;
            //char c;
            string str;

            #region 控制按键

            p_allHotKeys.Add("Enter", Keys.Enter);
            p_allHotKeys.Add("Space", Keys.Space);
            p_allHotKeys.Add("LShift", Keys.LShiftKey);
            p_allHotKeys.Add("RShift", Keys.RShiftKey);
            p_allHotKeys.Add("LCtrl", Keys.LControlKey);
            p_allHotKeys.Add("RCtrl", Keys.RControlKey);
            p_allHotKeys.Add("LAlt", Keys.LMenu);
            p_allHotKeys.Add("RAlt", Keys.RMenu);
            p_allHotKeys.Add("BackSpace", Keys.Back);
            p_allHotKeys.Add("CapsLock", Keys.CapsLock);

            #endregion

            #region 字母

            for (i = 0; i < 26; i++)
            {
                str = ((char)(i + 'A')).ToString();
                p_allHotKeys.Add(str, (Keys)(i + Keys.A));
            }

            #endregion

            #region 数字键

            for (i = 0; i < 10; i++)
            {
                str = ((char)(i + '0')).ToString();
                p_allHotKeys.Add(str, (Keys)(i + Keys.D0));
            }

            #endregion

            #region 标点

            /*
            OemMinus
            Oemplus
            OemOpenBrackets
            Oem6
            Oem5
            Oem1
            Oem7
            Oemcomma
            OemPeriod
            OemQuestion
            Oemtilde
            */

            p_allHotKeys.Add("-", Keys.OemMinus);
            p_allHotKeys.Add("=", Keys.Oemplus);
            p_allHotKeys.Add("[", Keys.OemOpenBrackets);
            p_allHotKeys.Add("]", Keys.OemCloseBrackets);
            p_allHotKeys.Add("\\", Keys.OemPipe);
            p_allHotKeys.Add(";", Keys.OemSemicolon);
            p_allHotKeys.Add("'", Keys.OemQuotes);
            p_allHotKeys.Add(",", Keys.Oemcomma);
            p_allHotKeys.Add(".", Keys.OemPeriod);
            p_allHotKeys.Add("/", Keys.OemQuestion);
            p_allHotKeys.Add("`", Keys.Oemtilde);

            #endregion

            #region F1-F12

            for (i = 0; i < 12; i++)
            {
                p_allHotKeys.Add("F" + (i + 1).ToString(), Keys.F1 + i);
            }

            #endregion

            #region 小键盘

            for (i = 0; i < 10; i++)
            {

                p_allHotKeys.Add("Num_" + (i.ToString()), Keys.NumPad0 + i);
            }

            /*
            按下:Divide
            按下:Multiply
            按下:Subtract
            按下:Add
            按下:Decimal
            按下:Return
            */
            p_allHotKeys.Add("Num_/", Keys.Divide);
            p_allHotKeys.Add("Num_*", Keys.Multiply);
            p_allHotKeys.Add("Num_-", Keys.Subtract);
            p_allHotKeys.Add("Num_+", Keys.Add);
            p_allHotKeys.Add("Num_.", Keys.Decimal);

            #endregion

            #region 方向键和系统控制键

            p_allHotKeys.Add("Up", Keys.Up);
            p_allHotKeys.Add("Down", Keys.Down);
            p_allHotKeys.Add("Left", Keys.Left);
            p_allHotKeys.Add("Right", Keys.Right);

            //[ "Insert", "Delete", "Home", "End", "PageUp", "PageDown" ]

            p_allHotKeys.Add("Insert", Keys.Insert);
            p_allHotKeys.Add("Delete", Keys.Delete);
            p_allHotKeys.Add("Home", Keys.Home);
            p_allHotKeys.Add("End", Keys.End);
            p_allHotKeys.Add("PageUp", Keys.PageUp);
            p_allHotKeys.Add("PageDown", Keys.PageDown);
            #endregion

            #endregion

        }

        private void f_initKeyLoop()
        {
            DebugPrintLineF(DateTime.Now.ToString() + ": 初始化消息挂钩");
            p_keyHook = new KeyHook();
            p_keyHook.Active = true;
            p_keyHook.KeyHookEvent += fe_keyHook;

            //p_keyMegThread = new Thread(f_keyMessageGetting);
            //p_keyMegThread.IsBackground = true;
            //p_keyMegThread.SetApartmentState(ApartmentState.MTA);
            //p_keyMegThread.Priority = ThreadPriority.Normal;
            //p_keyMegThread.Start();
        }

        private void f_initLoopMod()
        {
            DebugPrintLineF(DateTime.Now.ToString() + ": " + "初始化后台修改线程");
            p_loopFunction = new ModifyLoop();
            p_loopFunction.FramesPerSecond = 30;
            p_modThread = new Thread(p_loopFunction.LoopFunc);
            p_modThread.IsBackground = true;
            p_modThread.TrySetApartmentState(ApartmentState.MTA);
            p_modThread.Name = "modLoop";
            p_modThread.Priority = ThreadPriority.Normal;
            p_modThread.Start();
        }

        private static string f_getCmdPath(string cmd)
        {
            if (string.IsNullOrEmpty(cmd)) return null;

            if(cmd.Length <= 2) return cmd;

            var first = cmd.IndexOf('"');

            if (first < 0) return cmd;

            var end = cmd.LastIndexOf('"');

            if (end < 0) return cmd;

            try
            {
                return cmd.Substring(first + 1, (end - first) - 1);
            }
            catch (Exception)
            {
                return cmd;
            }
            
        }

        /// <summary>
        /// Debug - 将当前的文件配置打印到文本缓冲区
        /// </summary>
        /// <param name="append"></param>
        public void Debug_PrintMods(StringBuilder append)
        {
            if (processModify.IsOpenSetup)
            {
                append.AppendLine("地址集:");
                foreach (var item in processModify.Addresses.Addresses)
                {
                    append.Append(item.Key);
                    append.Append(':');
                    var ads = item.Value;
                    if(ads is ProcessAddressValue adv)
                    {
                        append.AppendLine(((long)adv.address).ToString("X"));
                    }
                    else if (ads is ProcessModuleAddress adm)
                    {
                        if (adm.IsModule)
                        {
                            append.Append(adm.ModuleName);
                        }
                        else
                        {
                            append.Append(((long)adm.BaseAddress).ToString("X"));
                        }
                        if(adm.ModuleOffsetLong != 0)
                        {
                            append.Append(" + ");
                            append.Append(adm.ModuleOffsetLong.ToString("X"));
                        }
                        if (adm.Offsets.Count != 0)
                        {
                            append.Append(' ');
                            foreach (var offsetItem in adm.Offsets)
                            {
                                append.Append(" -> ");
                                if(offsetItem >= 0)
                                {
                                    append.Append('+');
                                    append.Append(offsetItem.ToString("X"));
                                }
                                else
                                {
                                    append.Append('-');
                                    append.Append((-offsetItem).ToString("X"));
                                }
                            }
                            append.AppendLine();
                        }
                    }
                    else
                    {
                        append.AppendLine(ads.GetType().Name);
                    }
                    
                }

                append.AppendLine("项数据:");
                foreach (var mod in processModify.ProcessModifys.ModifyList)
                {
                    append.Append(mod.Text);
                    append.Append(':');
                    append.Append(" 开关:");
                    append.Append(mod.Toggle ? "√": "×");
                    append.Append(" ");
                    append.Append("地址id:");
                    append.AppendLine(mod.AddressID);
                }

            }
            else
            {
                append.AppendLine("没有打开的配置项");
            }
        }

        /// <summary>
        /// 从命令行初始化修改项
        /// </summary>
        private void f_openFileMyCommand()
        {
            //路径
            var path = f_getCmdPath(commandlineArgs);

            if (!File.Exists(path))
            {
                DebugPrintLineF("命令行文件路径不存在");
                return;
            }

            try
            {
                StringBuilder sb;
                var error = processModify.InitByFile(path);
                if (error == ProcessModify.InitSetupError.None)
                {
                    DebugPrintLineF($"成功初始化文件\"{path}\"");
                    sb = new StringBuilder(64);
                    Debug_PrintMods(sb);
                    DebugPrintF(sb.ToString());
                }
                else if (error == ProcessModify.InitSetupError.ProcessMissing)
                {
                    DebugPrintLineF($"成功初始化文件\"{path}\" 进程打开失败");
                    sb = new StringBuilder(64);
                    Debug_PrintMods(sb);
                    DebugPrintF(sb.ToString());
                }
                else if (error == ProcessModify.InitSetupError.FileReadError)
                {
                    DebugPrintLineF("配置文件是错误的json语法格式");
                }
                else if (error == ProcessModify.InitSetupError.JsonError)
                {
                    DebugPrintLineF("json文件的配置格式错误");
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(64);
                PrintException(ex, sb);
                DebugPrintLineF(sb.ToString());
            }

        }

        /// <summary>
        /// 初始化后台代码
        /// </summary>
        public void StartBackCode()
        {
            //p_loopFunction = new LoopFunction();
            f_initAudioEffect();
            f_openFileMyCommand();
            f_initKeyLoop();
            f_initLoopMod();
            
        }

        #endregion

        #region 键盘消息

        /// <summary>
        /// 将一个json热键或热键组合添加到集合
        /// </summary>
        /// <param name="hotkeysItem"></param>
        /// <param name="hotkeys">待添加集合</param>
        /// <returns>是否成功添加了至少一个热键</returns>
        public static bool JsonHotkeysInit(JsonVariable hotkeysItem, IList<Keys> hotkeys)
        {
            //初始化热键组合并排序
            if (hotkeysItem is null || hotkeys is null) throw new ArgumentNullException();
            var args = InitArgs.Args;
            Keys ks;
            if (hotkeysItem.DataType == JsonType.String)
            {
                if(args.p_allHotKeys.TryGetValue(hotkeysItem.String, out ks))
                {
                    hotkeys.Add(ks);
                    return true;
                }
                return false;
            }
            else if (hotkeysItem.DataType == JsonType.List)
            {
                var harr = hotkeysItem.Array;
                int length = harr.Count;
                bool ok = false;
                for (int i = 0; i < length; i++)
                {
                    var hj = harr[i];
                    if(hj.DataType == JsonType.String)
                    {
                        if (args.p_allHotKeys.TryGetValue(hj.String, out ks))
                        {
                            ok = true;
                            hotkeys.Add(ks);
                        }
                    }
                }

                if(ok) hotkeys.Sort(args.comparerKeys);
                return ok;
            }
            else
            {
                return false;
            }
            
        }

        #endregion

        #region 线程循环

        #endregion

        #region 音效播放

        /// <summary>
        /// 播放切换开关音效
        /// </summary>
        /// <param name="toggle">true播放开启音效，false播放关闭音效</param>
        public void PlayAudioCheckToggle(bool toggle)
        {
            //if (toggle)
            //{
            //    DebugPrintLineF("音效播放：开启");
            //}
            //else
            //{
            //    DebugPrintLineF("音效播放：关闭");
            //}
            p_audioEffects?.PlayOnToggle(toggle);
        }

        #endregion

        #endregion

        #region 其它函数

        /// <summary>
        /// 裁剪从起始位到第一个空格，再到第一个非空格前一个字符
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string f_toCommand(string command)
        {
            
            int length = command.Length;
            int i;
            for (i = 0; i < length; i++)
            {
                if (char.IsWhiteSpace(command[i]))
                {
                    //属于空格
                    goto OK;
                }
            }

            return null;

            OK:

            i++;
            if (i >= length)
            {
                return null;
            }

            for (i = 0; i < length; i++)
            {
                if (!char.IsWhiteSpace(command[i]))
                {
                    //不属于空格
                    goto OK2;
                }
            }
            //没有不属于空格的字符
            return null;

            OK2:

            return command.Substring(i);
        }

        #endregion

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                //p_keyMegThread.Abort();
                p_keyHook?.Close();
                p_audioEffects?.Close();
                p_loopFunction.Exit();
                this.debugLogPrint?.Close();
            }
            
            p_keyHook = null;
            this.debugLogPrint = null;
            p_audioEffects = null;

            return true;
        }

        #endregion

    }

}
#if DEBUG

#endif
#if DEBUG

#else

#endif