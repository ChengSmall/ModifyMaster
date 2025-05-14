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
using Cheng.ModifyMaster;

namespace Cheng.DataStructure.NumGenerators
{

    /// <summary>
    /// 值生成器 - 从修改器操作台读取进程值
    /// </summary>
    public class NumGeneratorModify : NumGenerator
    {

        #region

        /// <summary>
        /// 实例化一个地址读取值生成器
        /// </summary>
        /// <param name="processModify">修改器操作台</param>
        /// <param name="id">地址id</param>
        /// <param name="type">要读取的数据类型</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NumGeneratorModify(ProcessModify processModify, string id, DataType type)
        {
            if (processModify is null || id is null) throw new ArgumentNullException();

            p_mod = processModify;
            p_id = id;
            p_type = type;
        }

        /// <summary>
        /// 实例化一个地址读取值生成器
        /// </summary>
        /// <param name="processModify">修改器操作台</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public NumGeneratorModify(ProcessModify processModify)
        {
            if (processModify is null) throw new ArgumentNullException();

            p_mod = processModify;
            p_id = string.Empty;
            p_type = DataType.Int32;
        }

        #endregion

        #region

        private ProcessModify p_mod;

        private string p_id;

        private DataType p_type;

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置地址ID
        /// </summary>
        public string AddressID
        {
            get => p_id;
            set
            {
                p_id = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 访问或设置从地址处要读取的数据类型
        /// </summary>
        public DataType DataType
        {
            get => p_type;
            set
            {
                p_type = value;
            }
        }

        public override DynamicNumber Generate()
        {

            var ads = p_mod.Addresses;

            var ptr = ads.GetAddress(p_id, p_mod);

            if(ptr == IntPtr.Zero)
            {
                throw new NotImplementedException("无法获取地址");
            }

            var pro = p_mod.ProOperation;
            switch (p_type)
            {
                case DataType.Int32:
                    if (pro.Read<int>(ptr, out int i))
                    {
                        return i;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                case DataType.UInt32:
                    if (pro.Read<uint>(ptr, out uint ui))
                    {
                        return ui;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                case DataType.Int64:
                    if (pro.Read<long>(ptr, out long l))
                    {
                        return l;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                //case DataType.UInt64:
                //    if (pro.Read<ulong>(ptr, out ulong ul))
                //    {
                //        return ul;
                //    }
                //    else
                //    {
                //        throw new NotImplementedException("无法从地址读取值");
                //    }
                case DataType.Float:
                    if (pro.Read<float>(ptr, out float f))
                    {
                        return f;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                case DataType.Double:
                    if (pro.Read<double>(ptr, out double d))
                    {
                        return d;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                case DataType.Int16:
                    if (pro.Read<short>(ptr, out short s))
                    {
                        return s;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                case DataType.UInt16:
                    if (pro.Read<ushort>(ptr, out ushort us))
                    {
                        return us;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                case DataType.Byte:
                    if (pro.Read<byte>(ptr, out byte b))
                    {
                        return b;
                    }
                    else
                    {
                        throw new NotImplementedException("无法从地址读取值");
                    }
                default:
                    throw new NotImplementedException("数据类型参数不正确");
            }

        }

        #endregion

    }

}
