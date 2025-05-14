using Cheng.Json;
using Cheng.Windows.Processes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cheng.Memorys;
using Cheng.Streams.Parsers;
using Cheng.Streams.Parsers.Default;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Cheng.Json.GeneratorNumbers;
using Cheng.Windows.Hooks;
using System.ComponentModel;

namespace Cheng.ModifyMaster
{


    /*
    int32 => 表示4字节整形
    uint32 => 表示4字节无符号整形
    int64 => 表示8字节整形
    float => 表示单精度浮点型（4个字节）
    double => 表示双精度浮点型（8个字节）
    int16 => 表示2字节整形
    uint16 => 表示2字节无符号整形
    byte => 表示单个字节的整数
    */

    /// <summary>
    /// 数据值类型
    /// </summary>
    public enum DataType
    {

        Int32,

        UInt32,

        Int64,

        //UInt64,

        Float,

        Double,

        Int16,

        UInt16,

        Byte
    }

    /// <summary>
    /// 信息
    /// </summary>
    public struct Information
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name;

        /// <summary>
        /// 作者
        /// </summary>
        public string author;

        /// <summary>
        /// 简介
        /// </summary>
        public string synopsis;
    }

    /// <summary>
    /// 修改器操作台
    /// </summary>
    public unsafe class ProcessModify : SafreleaseUnmanagedResources
    {

        #region

        /// <summary>
        /// 初始化修改器参数时的错误码
        /// </summary>
        public enum InitSetupError
        {

            /// <summary>
            /// 无错误
            /// </summary>
            None = 0,

            /// <summary>
            /// 成功将参数配置完毕，但是给定的进程名无法找到并打开
            /// </summary>
            ProcessMissing,

            /// <summary>
            /// Json参数格式错误
            /// </summary>
            JsonError,

            /// <summary>
            /// 在读取配置文件时出错
            /// </summary>
            FileReadError

        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化操作台
        /// </summary>
        public ProcessModify()
        {
            p_processOperation = null;
            p_setup = null;
            //p_pro = null;
            p_openPro = false;
            OpenProcessID = null;
            OpenProcessName = null;
            //InitArgs.Args.p_loopFunction.UpdateEvent;
            //InitArgs.Args.KeyEvent += Args_KeyEvent;
            p_modifys = new Modifys();
            moduleComparer = new ModuleComparer();
            p_openArgs = new ProcessArgs();
            p_modules = new Dictionary<string, List<ProcessModule>>(32);
            p_jnGenerator = new JsonValueGeneratorMod(this);
            p_comparerProcessID = new ComparerProcessID();
        }

        #endregion

        #region 参数

        private Information p_information;

        private ProcessOperation p_processOperation;

        /// <summary>
        /// 打开的进程名
        /// </summary>
        private string p_openProcessName;

        /// <summary>
        /// 打开的进程ID文本缓存
        /// </summary>
        private string p_openProID;

        /// <summary>
        /// 进程排序器
        /// </summary>
        public readonly ComparerProcessID p_comparerProcessID;

        /// <summary>
        /// 模块排序器
        /// </summary>
        public readonly ModuleComparer moduleComparer;

        /// <summary>
        /// json值生成器
        /// </summary>
        private JsonValueGeneratorMod p_jnGenerator;

        /// <summary>
        /// 修改器配置文件
        /// </summary>
        private JsonDictionary p_setup;

        /// <summary>
        /// 打开后进程的模块按名称分类，同名模块处于一个集合下
        /// </summary>
        private Dictionary<string, List<ProcessModule>> p_modules;

        /// <summary>
        /// 打开的地址集
        /// </summary>
        private ProcessArgs p_openArgs;

        /// <summary>
        /// 修改项集合
        /// </summary>
        private Modifys p_modifys;

        /// <summary>
        /// 操作进程是否64位
        /// </summary>
        private bool p_poIsX64;

        /// <summary>
        /// 是否打开了一个进程
        /// </summary>
        private bool p_openPro;

        #endregion

        #region 功能

        #region 初始化和关闭

        /// <summary>
        /// 从指定配置文件初始化配置
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>错误码</returns>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public InitSetupError InitByFile(string filePath)
        {
            ThrowObjectDisposeException();

            JsonVariable json;

            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sread = new StreamReader(file, Encoding.UTF8, false, 1024 * 2, true))
                    {
                        json = InitArgs.Args.jsonParser.ToJsonData(sread);
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(64);
                InitArgs.PrintException(ex, sb);
                InitArgs.Args.DebugPrintLineF(sb.ToString());
                return InitSetupError.FileReadError;
            }

            return Init(json);
        }

        /// <summary>
        /// 初始化一个设置
        /// </summary>
        /// <param name="setup">要初始化的配置参数</param>
        /// <returns>错误码</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public InitSetupError Init(JsonVariable setup)
        {
            ThrowObjectDisposeException();
            if (setup is null) throw new ArgumentNullException();
            if(setup.DataType == JsonType.Dictionary)
            {
                return f_initJson(setup.JsonObject);
            }
            return InitSetupError.JsonError;
        }

        /// <summary>
        /// 获取所有与给定名称相同的进程
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <param name="comparer">进程排序器，在获取多个进程后进行集合排序</param>
        /// <returns>所有相同名称的进程集合，没有找到则为空数组</returns>
        public static List<Process> GetAllNameProcess(string name, IComparer<Process> comparer)
        {
            List<Process> list;
            var pros = Process.GetProcesses();
            list = new List<Process>(1);
            for (int i = 0; i < pros.Length; i++)
            {
                try
                {
                    if (pros[i].MainModule.ModuleName == name)
                    {
                        list.Add(pros[i]);
                        continue;
                    }
                }
                catch (Exception)
                {

                }

                pros[i].Close();
            }

            list.Sort(comparer);

            return list;
        }

        /// <summary>
        /// 清空当前的修改配置
        /// </summary>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public void ClearSetup()
        {
            p_setup = null;
            ModifyInformation = default;
            Addresses.Clear();
            ProcessModifys.Clear();
        }

        #region init封装

        private InitSetupError f_initJson(JsonDictionary jsonDict)
        {
            InitSetupError re;
            p_setup = jsonDict;
            try
            {
                if (jsonDict.TryGetValue("information", out var inf))
                {
                    //name
                    try
                    {
                        var infObj = inf.JsonObject;
                        Inf_Name = infObj["name"].String;
                    }
                    catch (Exception)
                    {
                        Inf_Name = null;
                    }

                    //author
                    try
                    {
                        var infObj = inf.JsonObject;
                        Inf_Author = infObj["author"].String;
                    }
                    catch (Exception)
                    {
                        Inf_Author = null;
                    }

                    //synopsis
                    try
                    {
                        var infObj = inf.JsonObject;
                        Inf_Synopsis = infObj["synopsis"].String;
                    }
                    catch (Exception)
                    {
                        Inf_Synopsis = null;
                    }

                }

                //data
                var dataObj = jsonDict["data"].JsonObject;

                var datas = dataObj["datas"].JsonObject;

                var mods = dataObj["modifier"].JsonObject;

                Addresses.JsonToArgs(datas);

                this.ProcessModifys.JsonInitList(mods, p_jnGenerator);

                try
                {
                    re = InitSetupError.None;
                    if (jsonDict.TryGetValue("process", out var pro_json))
                    {
                        re = InitSetupError.None;
                        //存在进程参数
                        //moduleName
                        //count
                        var proDict = pro_json.JsonObject;
                        //模块名
                        var modName = proDict["moduleName"].String;

                        int count = 0;

                        if(proDict.TryGetValue("count", out var json_count))
                        {
                            if(json_count.DataType == JsonType.Integer)
                            {
                                count = (int)json_count.Integer;
                            }
                        }

                        var list = GetAllNameProcess(modName, p_comparerProcessID);
                        if(list.Count > 0)
                        {
                            Process pro;
                            if (list.Count == 1)
                            {
                                pro = list[0];
                            }
                            else
                            {
                                try
                                {
                                    pro = list[count];
                                }
                                catch (Exception)
                                {
                                    pro = list[0];
                                }
                            }

                            foreach (var proClear in list)
                            {
                                if(pro != proClear) proClear.Close();
                            }

                            //ProcessOperation proo = null;

                            try
                            {
                                //proo = new ProcessOperation(
                                //      pro.Id, ProcessAccessFlags.MemoryModification);

                                if (!OpenProcessByID(pro.Id))
                                {
                                    re = InitSetupError.ProcessMissing;
                                }
                            }
                            catch (Exception ex)
                            {
                                re = InitSetupError.ProcessMissing;
                                StringBuilder sb = new StringBuilder(32);
                                InitArgs.PrintException(ex, sb);
                                InitArgs.Args.DebugPrintLineF(sb.ToString());
                            }

                            pro.Close();
                        }
                        else
                        {
                            re = InitSetupError.ProcessMissing;
                        }

                        return re;
                    }
                }
                catch (Exception)
                {
                    re = InitSetupError.ProcessMissing;
                }

                return re;
            }
            catch (Exception)
            {
                p_setup = null;
                return InitSetupError.JsonError;
            }

        }

        #endregion

        /// <summary>
        /// 关闭当前打开的进程
        /// </summary>
        /// <returns>是否成功关闭；有进程关闭返回true，没有打开的进程返回false</returns>
        public bool CloseProcess()
        {
            ThrowObjectDisposeException();

            if (!p_openPro)
            {
                return false;
            }

            p_processOperation.Close();

            p_processOperation = null;
            //p_setup = null;

            p_openPro = false;
            ClearModule();
            return true;
        }

        /// <summary>
        /// 打开一个新的进程
        /// </summary>
        /// <param name="process">要打开的进程</param>
        /// <returns>是否成功打开</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数已经释放</exception>
        public bool OpenProcess(ProcessOperation process)
        {
            ThrowObjectDisposeException();
            if (process is null) throw new ArgumentNullException();
            if (!process.IsNotDispose)
            {
                throw new ArgumentException("参数是已释放实例");
            }

            try
            {
                if (InitArgs.Args.systemIsX64)
                {
                    p_poIsX64 = !process.IsWow64;
                }
                else
                {
                    p_poIsX64 = false;
                }
            }
            catch (Exception)
            {
                p_poIsX64 = InitArgs.Args.systemIsX64;
            }

            if (p_openPro)
            {
                p_processOperation?.Close();
                //p_pro.Close();
            }

           
            //p_pro = process.Process;
            OpenProcessID = process.Id.ToString();
           
            
            try
            {
                OpenProcessName = process.MainModule.ModuleName;
                
            }
            catch (Exception)
            {
                try
                {
                    OpenProcessName = process.Process.ProcessName;
                }
                catch (Exception)
                {
                    OpenProcessName = null;
                }
                
            }

            p_openPro = true;
            p_processOperation = process;
            try
            {
                InitProcessModules();
            }
            catch (Exception)
            {
                p_openPro = false;
                //p_processOperation?.Close();
                p_processOperation = null;
                return false;
            }
            
            
            return true;
        }

        /// <summary>
        /// 按进程id打开一个进程
        /// </summary>
        /// <param name="id">进程id</param>
        /// <returns>是否成功打开</returns>
        /// <exception cref="ArgumentException">参数无效</exception>
        /// <exception cref="InvalidOperationException">无法访问进程句柄</exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public bool OpenProcessByID(int id)
        {

            ProcessOperation pro = new ProcessOperation(id, ProcessAccessFlags.MemoryModification);
            try
            {
                if (!OpenProcess(pro))
                {
                    pro.Close();
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                pro.Close();
                throw;
            }
        }

        #endregion

        #region 参数访问

        /// <summary>
        /// 打开的进程ID文本显示缓存
        /// </summary>
        public string OpenProcessID
        {
            get => p_openProID;
            set
            {
                p_openProID = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 打开的进程程序名或主模块名
        /// </summary>
        public string OpenProcessName
        {
            get => p_openProcessName;
            set
            {
                p_openProcessName = value;
            }
        }

        /// <summary>
        /// 修改器配置文件信息
        /// </summary>
        public Information ModifyInformation
        {
            get => p_information;
            set
            {
                if (value.name is null) value.name = string.Empty;
                p_information = value;
            }
        }

        /// <summary>
        /// 修改器配置文件名称
        /// </summary>
        public string Inf_Name
        {
            get => p_information.name;
            set
            {
                p_information.name = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 修改器配置文件作者
        /// </summary>
        public string Inf_Author
        {
            get => p_information.author;
            set
            {
                p_information.author = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 修改器配置文件简介
        /// </summary>
        public string Inf_Synopsis
        {
            get => p_information.synopsis;
            set
            {
                p_information.synopsis = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 当前是否打开了一个进程
        /// </summary>
        public bool IsOpenProcess
        {
            get
            {
                if (IsDispose) return false;
                return p_openPro;
            }
        }

        /// <summary>
        /// 当前操作台是否已打开了一个修改配置
        /// </summary>
        public bool IsOpenSetup
        {
            get => ((object)p_setup != null);
        }

        /// <summary>
        /// 打开的进程是否为64位
        /// </summary>
        public bool OpenProcessIsX64
        {
            get
            {
                if(p_openPro) return p_poIsX64;
                throw new NotImplementedException("未打开进程");
            }
        }

        /// <summary>
        /// 获取打开的进程操作类
        /// </summary>
        /// <returns>如果此时没有打开进程则返回null</returns>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public ProcessOperation ProOperation
        {
            get
            {
                ThrowObjectDisposeException();
                if (p_openPro) return p_processOperation;
                return null;
            }
        }

        /// <summary>
        /// 访问打开的地址集
        /// </summary>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public ProcessArgs Addresses
        {
            get
            {
                ThrowObjectDisposeException();
                return p_openArgs;
            }
        }

        /// <summary>
        /// 获取json值生成器
        /// </summary>
        public JsonValueGeneratorMod JsonNumGenerator
        {
            get => p_jnGenerator;
        }

        /// <summary>
        /// 访问修改项集合
        /// </summary>
        public Modifys ProcessModifys
        {
            get => p_modifys;
        }

        #endregion

        #region 模块集合操作

        /// <summary>
        /// 重新从进程操作对象中初始化模块列表
        /// </summary>
        public void InitProcessModules()
        {
            if (!p_openPro) throw new NotImplementedException();
            
            var mods = p_processOperation.Process.Modules;

            int length = mods.Count;
            ClearModule();

            for (int i = 0; i < length; i++)
            {
                var mod = mods[i];
                AddModule(mod);
            }
        }

        /// <summary>
        /// 打开的进程所加载的模块集合
        /// </summary>
        public Dictionary<string, List<ProcessModule>> Modules
        {
            get
            {
                ThrowObjectDisposeException();
                return p_modules;
            }
        }

        /// <summary>
        /// 添加一个模块到列表
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(ProcessModule module)
        {
            if (module is null) throw new ArgumentNullException();
            var name = module.ModuleName;
            List<ProcessModule> mlist;
            if (p_modules.TryGetValue(name, out mlist))
            {
                //存在同名模块
                mlist.Add(module);
            }
            else
            {
                //新模块
                mlist = new List<ProcessModule>(1);
                mlist.Add(module);
                p_modules.Add(name, mlist);
            }
        }

        /// <summary>
        /// 删除一个模块
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool Remove(ProcessModule module)
        {
            if (module is null) throw new ArgumentNullException();
            var name = module.ModuleName;
            List<ProcessModule> mlist;
            if (p_modules.TryGetValue(name, out mlist))
            {
                //存在同名模块
                int i = mlist.IndexOf(module);
                if(i != -1)
                {
                    //删除
                    mlist.RemoveAt(i);
                    if(mlist.Count == 0)
                    {
                        //集合空
                        p_modules.Remove(name);
                    }
                    return true;
                }
                //没有相同模块
                return false;
            }
            //不存在项
            return false;
        }

        /// <summary>
        /// 清空模块列表
        /// </summary>
        public void ClearModule()
        {
            p_modules.Clear();
        }

        /// <summary>
        /// 从列表获取模块
        /// </summary>
        /// <param name="name">模块名称</param>
        /// <param name="count">如果存在同名模块，指定获取的列表顺序；索引超出返回第一个</param>
        /// <returns>获取的模块，null表示不存在</returns>
        /// <exception cref="ArgumentNullException">名称是null</exception>
        public ProcessModule GetModule(string name, int count)
        {
            if(p_modules.TryGetValue(name, out var list))
            {
                if (list.Count == 0)
                {
                    return null;
                }
                if (list.Count == 1)
                {
                    return list[0];
                }

                list.Sort(moduleComparer);
                try
                {
                    return list[count];
                }
                catch (Exception)
                {
                    return list[0];
                }
            }

            return null;
        }

        /// <summary>
        /// 从列表获取模块
        /// </summary>
        /// <param name="name">模块名称</param>
        /// <returns>获取的模块，null表示不存在</returns>
        /// <exception cref="ArgumentNullException">名称是null</exception>
        public ProcessModule GetModule(string name)
        {
            if (p_modules.TryGetValue(name, out var list))
            {
                if (list.Count > 0)
                {
                    return list[0];
                }
                return null;
            }

            return null;
        }

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_processOperation?.Close();
                //p_pro?.Close();
            }
            //p_pro = null;
            
            ClearSetup();
            ClearModule();
            p_processOperation = null;
            p_setup = null;
            
            return true;
        }

        #endregion

        #region 事件注册

        #endregion

        #endregion

    }

}
