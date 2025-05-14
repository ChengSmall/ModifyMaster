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


namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 一条地址访问参数
    /// </summary>
    public abstract class ProcessAddress
    {

        #region

        #endregion

        #region

        /// <summary>
        /// 获取该条目的访问地址
        /// </summary>
        /// <param name="modify"></param>
        /// <returns>要访问或设置的最终所在进程地址，如果是空指针则表示给定的参数无法访问，或者进程已经结束</returns>
        /// <exception cref="Exception">其它错误</exception>
        public abstract IntPtr GetAddress(ProcessModify modify);

        /// <summary>
        /// 获取该条目的访问地址
        /// </summary>
        /// <param name="modify"></param>
        /// <returns>要访问或设置的最终所在进程地址，如果是空指针则表示因各种理有无法访问</returns>
        public virtual IntPtr TryGetAddress(ProcessModify modify)
        {
            try
            {
                return GetAddress(modify);
            }
            catch (Exception)
            {
                return IntPtr.Zero;
            }
        }

        #endregion

    }

    /// <summary>
    /// 纯地址值返回
    /// </summary>
    public sealed class ProcessAddressValue : ProcessAddress
    {

        public ProcessAddressValue(IntPtr ptr)
        {
            this.address = ptr;
        }

        /// <summary>
        /// 纯地址值
        /// </summary>
        public readonly IntPtr address;

        public override IntPtr GetAddress(ProcessModify modify)
        {
            return address;
        }
    }

    /// <summary>
    /// 按模块和多级指针偏移访问地址
    /// </summary>
    public unsafe sealed class ProcessModuleAddress : ProcessAddress
    {

        public ProcessModuleAddress()
        {
            p_offsets = new List<long>();
            p_count = 0;
            p_moduleName = string.Empty;
            p_moduleOffset = 0;
            p_baseAddress = null;
            p_isModule = true;
        }

        private long p_moduleOffset;

        private string p_moduleName;

        private List<long> p_offsets;

        private int p_count;

        private void* p_baseAddress;

        /// <summary>
        /// 是模块还是纯地址基址
        /// </summary>
        private bool p_isModule;

        /// <summary>
        /// 基址属于模块还是纯地址值
        /// </summary>
        /// <value>设置为true表示用模块当作基址，false表示用地址值当作基址</value>
        public bool IsModule
        {
            get => p_isModule;
            set => p_isModule = value;
        }

        /// <summary>
        /// 访问或设置基址值，仅在<see cref="IsModule"/>是false时有效
        /// </summary>
        public void* BaseAddress
        {
            get => p_baseAddress;
            set
            {
                p_baseAddress = value;
            }
        }

        /// <summary>
        /// 访问或设置模块名称，仅在<see cref="IsModule"/>是true时有效
        /// </summary>
        public string ModuleName
        {
            get => p_moduleName;
            set => p_moduleName = value ?? string.Empty;
        }

        /// <summary>
        /// 访问或设置如果出现同名模块时需要访问的索引顺序，默认为0
        /// </summary>
        public int Count
        {
            get => p_count;
            set => p_count = value;
        }

        /// <summary>
        /// 表示将模块作为地址获取时需要对模块基址添加的字节偏移量，默认为0
        /// </summary>
        public int ModuleOffset
        {
            get => (int)p_moduleOffset;
            set => p_moduleOffset = value;
        }

        /// <summary>
        /// 表示获取地址时需要对模块或基址额外添加的字节偏移量，默认为0
        /// </summary>
        public long ModuleOffsetLong
        {
            get => p_moduleOffset;
            set => p_moduleOffset = value;
        }

        /// <summary>
        /// 访问或设置多级指针偏移，空集合表示返回模块基址，默认为空集合
        /// </summary>
        public List<long> Offsets
        {
            get => p_offsets;
        }

        public override IntPtr GetAddress(ProcessModify modify)
        {

            if (modify is null) throw new ArgumentNullException();

            var pro = modify.ProOperation;
            if (pro is null) throw new ArgumentNullException("pro", "操作进程类是null");

            bool open64 = modify.OpenProcessIsX64;
            byte* baseAddress;
            //获取进程基址
            if (p_isModule)
            {
                var proMod = modify.GetModule(ModuleName, Count);

                if (proMod is null)
                {
                    //不存在模块
                    return IntPtr.Zero;
                }
                //模块基址
                baseAddress = (byte*)proMod.BaseAddress;
            }
            else
            {
                baseAddress = (byte*)p_baseAddress;
            }

            //添加基址偏移
            //baseAddress = baseAddress.AddOffset(p_moduleOffset);
            baseAddress += p_moduleOffset;

            if(p_offsets.Count == 0)
            {
                return new IntPtr(baseAddress); //没有多级指针
            }

            //循环访问的多及指针根级
            var ptr = baseAddress;

            int length = p_offsets.Count;

            for (int i = 0; i < length; i++)
            {

                if (open64)
                {

                    if (!pro.Read<Pointer64>(new IntPtr(ptr), out Pointer64 point))
                    {
                        return IntPtr.Zero; //失败
                    }

                    //访问后添加偏移
                    ptr = ((byte*)point) + p_offsets[i];

                }
                else
                {
                    if (!pro.Read<Pointer32>(new IntPtr(ptr), out Pointer32 point))
                    {
                        return IntPtr.Zero; //失败
                    }

                    ptr = ((byte*)point) + p_offsets[i];

                }

            }

            return new IntPtr(ptr);
        }
    }

}
