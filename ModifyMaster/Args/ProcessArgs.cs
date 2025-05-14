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
using Cheng.Algorithm;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Collections;
using Cheng.Algorithm.Sorts;
using Cheng.Texts;
using Cheng.IO;

namespace Cheng.ModifyMaster
{


    /// <summary>
    /// 进程参数地址集
    /// </summary>
    public unsafe class ProcessArgs
    {

        #region 构造

        public ProcessArgs()
        {
            p_addresses = new Dictionary<string, ProcessAddress>();
        }

        #endregion

        #region 参数

        private Dictionary<string, ProcessAddress> p_addresses;

        #endregion

        #region 参数访问

        /// <summary>
        /// 地址集
        /// </summary>
        public Dictionary<string, ProcessAddress> Addresses
        {
            get => p_addresses;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 访问指定id的地址
        /// </summary>
        /// <param name="id">地址唯一id</param>
        /// <param name="modify">进程修改操作台</param>
        /// <returns>指定id的地址；空指针表示无法正常访问</returns>
        public IntPtr GetAddress(string id, ProcessModify modify)
        {
            if(p_addresses.TryGetValue(id, out var value))
            {
                return value.TryGetAddress(modify);
            }
            return IntPtr.Zero;
        }

        public void Clear()
        {
            lock(p_addresses) p_addresses.Clear();
        }

        #region json结构创建

        /// <summary>
        /// 判断一行整数或字符串16进制数并返回
        /// </summary>
        /// <param name="json"></param>
        /// <param name="value">返回的值</param>
        /// <returns>是否成功转化</returns>
        static bool f_textORlongValue(JsonVariable json, out long value)
        {
            value = 0;
            if(json.DataType == JsonType.Integer)
            {
                value = json.Integer;
                return true;
            }
            if(json.DataType == JsonType.String)
            {
                string str = json.String;

                return str.X16ToValue(0, str.Length, out value);
            }
            return false;
        }

        /// <summary>
        /// 使用一个datas项创建一个地址访问参数
        /// </summary>
        /// <param name="json">一个修改器参数的datas项</param>
        /// <returns>地址访问参数，null表示参数错误无法创建</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static ProcessAddress JsonToAddressValue(JsonVariable json)
        {
            if (json is null) throw new ArgumentNullException();

            try
            {
                var type = json.DataType;
                if (type == JsonType.Integer)
                {
                    return new ProcessAddressValue(new IntPtr((void*)json.Integer));
                }

                if (type == JsonType.String)
                {
                    string str = json.String;

                    if (str.X16ToValue(0, str.Length, out long value))
                    {
                        return new ProcessAddressValue(new IntPtr((void*)value));
                    }
                    return null;
                    //Convert.ToInt64(json.String, 16);
                }
                long temp;
                if (type == JsonType.Dictionary)
                {
                    var jd = json.JsonObject;

                    ProcessModuleAddress modp = new ProcessModuleAddress();
                    //模块地址
                    var baseMod = jd["baseAddress"];
                    
                    if(f_textORlongValue(baseMod, out temp))
                    {
                        //
                        modp.IsModule = false;
                        modp.BaseAddress = (void*)temp;
                    }
                    else
                    {
                        modp.IsModule = true;

                        //属于模块名
                        if (baseMod.DataType == JsonType.Dictionary)
                        {
                            var modict = baseMod.JsonObject;
                            modp.ModuleName = modict["name"].String;
                            modp.Count = (int)modict["count"].Integer;
                        }
                        else
                        {
                            modp.ModuleName = baseMod.String;
                            modp.Count = 0;
                        }
                    }

                    modp.ModuleOffset = 0;
                    //获取基础偏移量
                    if (jd.TryGetValue("offset", out var baseOffset))
                    {
                        //存在基础偏移值

                        if (f_textORlongValue(baseOffset, out temp))
                        {
                            //成功转化
                            modp.ModuleOffsetLong = temp;
                        }
                        else
                        {
                            //偏移值出错
                            modp.ModuleOffset = 0;
                        }
                    }

                    //获取多级指针偏移集合
                    if (jd.TryGetValue("addresses", out var offs))
                    {
                        if (offs.DataType == JsonType.List)
                        {
                            var offlist = offs.Array;

                            for (int i = 0; i < offlist.Count; i++)
                            {
                                //一条偏移
                                var ofv = offlist[i];

                                if (f_textORlongValue(ofv, out temp))
                                {
                                    //成功获取一条偏移
                                    modp.Offsets.Add(temp);
                                }
                                else
                                {
                                    //整段垮掉~
                                    modp.Offsets.Clear();
                                    break;
                                }
                            }

                        }
                    }

                    return modp;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// 使用一个datas结构重新初始化地址集
        /// </summary>
        /// <param name="json">修改器的datas结构</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void JsonToArgs(JsonDictionary json)
        {
            if (json is null) throw new ArgumentNullException();
            p_addresses.Clear();

            foreach (var item in json)
            {
                var v = JsonToAddressValue(item.Value);
                if(v != null)
                {
                    p_addresses[item.Key] = v;
                }
            }
        }

        #endregion

        #endregion

    }

}
