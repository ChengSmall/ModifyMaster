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
using Cheng.LoopThreads;
using Cheng.Windows.Hooks;
using Cheng.DataStructure.NumGenerators;

using DNum = Cheng.DataStructure.NumGenerators.DynamicNumber;
using System.Windows.Forms;
using Cheng.Algorithm.Sorts.Comparers;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 后台修改循环
    /// </summary>
    public class ModifyLoop : LoopFunction
    {

        #region 构造

        public ModifyLoop()
        {
            p_loopPrint = false;
            p_keyEventDown = false;
            p_keyDown = false;
            p_nowDowns = new List<Keys>();

            p_defDNumComp = Comparer<DNum>.Default;
            p_revDNumComp = new InvertComparer<DNum>(p_defDNumComp, true);
        }

        #endregion

        #region 参数

        private ProcessModify p_mod;

        private List<Keys> p_nowDowns;

        /// <summary>
        /// DNum常规比较器
        /// </summary>
        private readonly Comparer<DNum> p_defDNumComp;

        /// <summary>
        /// DNum反向比较器
        /// </summary>
        private readonly InvertComparer<DNum> p_revDNumComp;

        private bool p_loopPrint;

        /// <summary>
        /// 此轮循是否有按键按下
        /// </summary>
        private bool p_keyDown;

        /// <summary>
        /// 按键事件按下响应
        /// </summary>
        private bool p_keyEventDown;
        
        ///// <summary>
        ///// 按下按键后此次轮循操作时触发音效 音效：开
        ///// </summary>
        //private bool p_keyDownInvokePlayAudio_Open;

        /// <summary>
        /// 音效此次轮循已经播放一次 音效：开
        /// </summary>
        private bool p_keyDownAudioPlayOver_Open;

        ///// <summary>
        ///// 按下按键后此次轮循操作时触发音效 音效：关
        ///// </summary>
        //private bool p_keyDownInvokePlayAudio_Close;

        /// <summary>
        /// 音效此次轮循已经播放一次 音效：关
        /// </summary>
        private bool p_keyDownAudioPlayOver_Close;

        #endregion

        #region 功能

        #region 按键监控

        private void fe_keyHook(Hook hook, KeyHook.KeyHookArgs args)
        {
            if (args.State)
            {
                p_keyEventDown = true;

                //p_nowDowns.Clear();
                //var ag = InitArgs.Args;
                //ag.AppendNowHotkeysDown(p_nowDowns);
                //StringBuilder sb = new StringBuilder(32);
                //for (int i = 0; i < p_nowDowns.Count; i++)
                //{
                //    sb.Append(p_nowDowns[i].ToString());
                //    if(i + 1 < p_nowDowns.Count)
                //    {
                //        sb.Append(" + ");
                //    }
                //}
                //ag.DebugPrintLine(sb.ToString());
            }
        }

        #endregion

        #region 派生

        #region 封装

        #region 进程内存读写

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param name="pro"></param>
        /// <param name="ptr">写入位置</param>
        /// <param name="dataType">写入时类型</param>
        /// <param name="value">要写入的值</param>
        /// <returns>是否成功</returns>
        private bool f_write(ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {
            switch (dataType)
            {
                case DataType.Int32:
                    return pro.Write<int>(ptr, (int)value);
                case DataType.UInt32:
                    return pro.Write<uint>(ptr, (uint)value);
                case DataType.Int64:
                    return pro.Write<long>(ptr, (long)value);
                //case DataType.UInt64:
                //    return pro.Write<ulong>(ptr, (ulong)value);
                case DataType.Float:
                    return pro.Write<float>(ptr, (float)value);
                case DataType.Double:
                    return pro.Write<double>(ptr, (double)value);
                case DataType.Int16:
                    return pro.Write<short>(ptr, (short)value);
                case DataType.UInt16:
                    return pro.Write<ushort>(ptr, (ushort)value);
                case DataType.Byte:
                    return pro.Write<byte>(ptr, (byte)value);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param name="pro"></param>
        /// <param name="ptr">要读取的位置</param>
        /// <param name="dataType">读取时类型</param>
        /// <param name="value">要读取的值</param>
        /// <returns>是否成功</returns>
        private bool f_read(ProcessOperation pro, IntPtr ptr, DataType dataType, out DNum value)
        {
            bool b;
            value = default;
            switch (dataType)
            {
                case DataType.Int32:
                    int rei;
                    b = pro.Read<int>(ptr, out rei);
                    value = rei;
                    break;
                case DataType.UInt32:
                    uint ui;
                    b = pro.Read<uint>(ptr, out ui);
                    value = ui;
                    break;
                case DataType.Int64:
                    long l;
                    b = pro.Read<long>(ptr, out l);
                    value = l;
                    break;
                //case DataType.UInt64:
                //    ulong ul;
                //    b = pro.Read<ulong>(ptr, out ul);
                //    value = ul;
                //    break;
                case DataType.Float:
                    float f;
                    b = pro.Read<float>(ptr, out f);
                    value = f;
                    break;
                case DataType.Double:
                    double d;
                    b = pro.Read<double>(ptr, out d);
                    value = d;
                    break;
                case DataType.Int16:
                    short s;
                    b = pro.Read<short>(ptr, out s);
                    value = s;
                    break;
                case DataType.UInt16:
                    ushort us;
                    b = pro.Read<ushort>(ptr, out us);
                    value = us;
                    break;
                case DataType.Byte:
                    byte bs;
                    b = pro.Read<byte>(ptr, out bs);
                    value = bs;
                    break;
                default:
                    b = false;
                    break;
            }

            return b;
        }

        #region 修改器轮询

        private void f_modGR(ModifyItem modifyItem, ModifyAddressType modifyType)
        {

            if (!modifyItem.Toggle)
            {
                return;
            }
            bool? tb = modifyItem.TernaryConditionInvoke(false);

            if (tb.HasValue)
            {
                if (tb.Value)
                {
                    modifyItem.ViewCondition = ModifyItem.ViewConditionYes;
                }
                else
                {
                    //不符合条件
                    modifyItem.ViewCondition = ModifyItem.ViewConditionNo;
                    return;
                }
            }
            else
            {
                modifyItem.ViewCondition = ModifyItem.ViewConditionNone;
            }

            var pro = p_mod.ProOperation;
            if (pro is null) return;

            StringBuilder sb;

            var address = modifyItem.Address;

            var value = modifyItem.Value;

            ProcessAddress proAddress;

            if (!p_mod.Addresses.Addresses.TryGetValue(address.id, out proAddress))
            {
                //没有找到地址ID
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:未找到地址ID");
                return;
            }
            //地址
            IntPtr ptr;

            //获取要设置的值
            DNum setValue;

            try
            {
                if(value is null)
                {
                    if (InitArgs.Args.CanDebug)
                    {
                        p_loopPrint = true;
                        InitArgs.Args.DebugPrintLine("没有value参数");
                    }
                    return;
                }
                setValue = value.Generate();
            }
            catch (Exception)
            {
                if (InitArgs.Args.CanDebug)
                {
                    p_loopPrint = true;
                    //sb = new StringBuilder(64);
                    //InitArgs.PrintException(ex, sb);
                    InitArgs.Args.DebugPrintLine("无法获取value");
                }
                return;
            }

            try
            {
                ptr = proAddress.GetAddress(p_mod);
            }
            catch (Exception ex)
            {
                if (InitArgs.Args.CanDebug)
                {
                    p_loopPrint = true;
                    sb = new StringBuilder(64);
                    InitArgs.PrintException(ex, sb);
                    InitArgs.Args.DebugPrintLine(sb.ToString());
                }
                return;
            }

            if (ptr == IntPtr.Zero)
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:无法读取\"{address.id}\"指向的地址");
                return;
            }

            var dataType = modifyItem.AddressDataType;

            if (p_mod.IsOpenProcess)
            {
                pro = p_mod.ProOperation;
                if (pro is null)
                {
                    //p_loopPrint = true;
                    //InitArgs.Args.DebugPrintLine("进程操作类是null");
                    return;
                }

                switch (modifyType)
                {
                    case ModifyAddressType.Fixed:
                        f_fixed(modifyItem, pro, ptr, dataType, setValue);
                        break;
                    case ModifyAddressType.FixedUp:
                        f_fixedUp(modifyItem, pro, ptr, dataType, setValue);
                        break;
                    case ModifyAddressType.FixedDown:
                        f_fixedDown(modifyItem, pro, ptr, dataType, setValue);
                        break;
                    case ModifyAddressType.Once:
                        f_once(modifyItem, pro, ptr, dataType, setValue);
                        break;
                    case ModifyAddressType.Add:
                        f_add(modifyItem, pro, ptr, dataType, setValue);
                        break;
                    case ModifyAddressType.Sub:
                        f_sub(modifyItem, pro, ptr, dataType, setValue);
                        break;
                    default:
                        break;
                }

            }

        }

        private void f_fixed(ModifyItem modifyItem, ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {

            bool b;
            b = f_write(pro, ptr, dataType, value);
            if (b)
            {
                modifyItem.ViewValue = value.ToString();
            }
            else
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据写入失败 类型:{dataType.ToString()}");
            }
        }

        private void f_fixedUp(ModifyItem modifyItem, ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {

            bool b;
            DNum reValue;
            //读取现有值
            b = f_read(pro, ptr, dataType, out reValue);
            if (b)
            {
                //写入大值
                if (reValue > value)
                {
                    value = reValue;
                }

                b = f_write(pro, ptr, dataType, reValue);
            }
            else
            {
                //读取错误不写入
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据读取失败 类型:{dataType.ToString()} 位置:{((long)ptr).ToString("X")}");
                return;
            }

            if (b)
            {
                modifyItem.ViewValue = value.ToString();
            }
            else
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据写入失败 类型:{dataType.ToString()}");
            }

        }

        private void f_fixedDown(ModifyItem modifyItem, ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {

            bool b;
            DNum reValue;
            //读取现有值
            b = f_read(pro, ptr, dataType, out reValue);
            if (b)
            {
                //写入小值
                if (reValue < value)
                {
                    value = reValue;
                }

                b = f_write(pro, ptr, dataType, value);
            }

            else
            {
                //读取错误不写入
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据读取失败 类型:{dataType.ToString()} 位置:{((long)ptr).ToString("X")}");
                return;
            }

            if (b)
            {
                modifyItem.ViewValue = value.ToString();
            }
            else
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据写入失败 类型:{dataType.ToString()}");
            }
        }

        private void f_once(ModifyItem modifyItem, ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {

            modifyItem.Toggle = false;
            bool b;
            
            b = f_write(pro, ptr, dataType, value);

            //DNum reValue;
            //读取现有值
            //b = f_read(pro, ptr, dataType, out reValue);
            //if (b)
            //{
            //    //写入
            //    //value -= reValue;
                
            //}
            //else
            //{
            //    //读取错误不写入
            //    p_loopPrint = true;
            //    InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据读取失败 类型:{dataType.ToString()} 位置:{((long)ptr).ToString("X")}");
            //    return;
            //}

            if (b)
            {
                modifyItem.ViewValue = value.ToString();
            }
            else
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据写入失败 类型:{dataType.ToString()}");
            }
        }


        private void f_add(ModifyItem modifyItem, ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {

            modifyItem.Toggle = false;
            bool b;
            DNum reValue;
            //读取现有值
            b = f_read(pro, ptr, dataType, out reValue);
            if (b)
            {
                //写入
                value += reValue;
                f_write(pro, ptr, dataType, value);
            }
            else
            {
                //读取错误不写入
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据读取失败 类型:{dataType.ToString()} 位置:{((long)ptr).ToString("X")}");
                return;
            }

            if (b)
            {
                modifyItem.ViewValue = value.ToString();
            }
            else
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据写入失败 类型:{dataType.ToString()}");
            }
        }

        private void f_sub(ModifyItem modifyItem, ProcessOperation pro, IntPtr ptr, DataType dataType, DNum value)
        {

            modifyItem.Toggle = false;
            bool b;
            DNum reValue;
            //读取现有值
            b = f_read(pro, ptr, dataType, out reValue);
            if (b)
            {
                //写入
                 value -= reValue;
                f_write(pro, ptr, dataType, value);
            }
            else
            {
                //读取错误不写入
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据读取失败 类型:{dataType.ToString()} 位置:{((long)ptr).ToString("X")}");
                return;
            }

            if (b)
            {
                modifyItem.ViewValue = value.ToString();
            }
            else
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:数据写入失败 类型:{dataType.ToString()}");
            }
        }

        private void f_modFixedSelf(ModifyItem modifyItem)
        {
            if (!modifyItem.Toggle)
            {
                return;
            }

            var pro = p_mod.ProOperation;
            if (pro is null) return;

            StringBuilder sb;

            var address = modifyItem.Address;

            //var value = modifyItem.Value;

            ProcessAddress proAddress;

            if (!p_mod.Addresses.Addresses.TryGetValue(address.id, out proAddress))
            {
                //没有找到地址ID
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:未找到地址ID");
                return;
            }
            //地址
            IntPtr ptr;

            try
            {
                ptr = proAddress.GetAddress(p_mod);
            }
            catch (Exception ex)
            {
                if (InitArgs.Args.CanDebug)
                {
                    p_loopPrint = true;
                    sb = new StringBuilder(64);
                    InitArgs.PrintException(ex, sb);
                    InitArgs.Args.DebugPrintLine(sb.ToString());
                }
                return;
            }

            if (ptr == IntPtr.Zero)
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:无法读取\"{address.id}\"指向的地址");
                return;
            }

            //获取要设置的值
            DNum setValue;

            var dataType = modifyItem.AddressDataType;

            if (p_mod.IsOpenProcess)
            {
                pro = p_mod.ProOperation;
                if (pro is null)
                {
                    p_loopPrint = true;
                    InitArgs.Args.DebugPrintLine("进程操作类是null");
                    return;
                }
                //获取值
                var value = modifyItem.Value;

                if(value is null)
                {
                    //初次打开
                    if(f_read(pro, ptr, dataType, out DNum reValue))
                    {
                        value = new NumGeneratorValue(reValue);
                        modifyItem.Value = value;
                    }
                    else
                    {
                        p_loopPrint = true;
                        InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:无法读取地址值");
                        return;
                    }
                }

                var tb = modifyItem.TernaryConditionInvoke(false);
                if (tb.HasValue)
                {
                    if (tb.Value)
                    {
                        //play = true;
                        setValue = value.Generate();
                        f_write(pro, ptr, dataType, setValue);
                        modifyItem.ViewValue = setValue.ToString();
                        modifyItem.ViewCondition = ModifyItem.ViewConditionYes;
                    }
                    else
                    {
                        //play = false;
                        modifyItem.ViewCondition = ModifyItem.ViewConditionNo;
                    }
                }
                else
                {
                    //play = true;
                    setValue = value.Generate();
                    f_write(pro, ptr, dataType, setValue);
                    modifyItem.ViewValue = setValue.ToString();
                    modifyItem.ViewCondition = ModifyItem.ViewConditionNone;
                }
               

            }

        }

        private void f_modFixedSelfUpAndDown(ModifyItem modifyItem, IComparer<DNum> comparer)
        {
            if (!modifyItem.Toggle)
            {
                return;
            }

            var pro = p_mod.ProOperation;
            if (pro is null) return;

            StringBuilder sb;

            var address = modifyItem.Address;

            //var value = modifyItem.Value;

            ProcessAddress proAddress;

            if (!p_mod.Addresses.Addresses.TryGetValue(address.id, out proAddress))
            {
                //没有找到地址ID
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:未找到地址ID");
                return;
            }
            //地址
            IntPtr ptr;

            try
            {
                ptr = proAddress.GetAddress(p_mod);
            }
            catch (Exception ex)
            {
                if (InitArgs.Args.CanDebug)
                {
                    p_loopPrint = true;
                    sb = new StringBuilder(64);
                    InitArgs.PrintException(ex, sb);
                    InitArgs.Args.DebugPrintLine(sb.ToString());
                }
                return;
            }

            if (ptr == IntPtr.Zero)
            {
                p_loopPrint = true;
                InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:无法读取\"{address.id}\"指向的地址");
                return;
            }

            //获取要设置的值
            DNum setValue;

            var dataType = modifyItem.AddressDataType;

            if (p_mod.IsOpenProcess)
            {
                pro = p_mod.ProOperation;
                if (pro is null)
                {
                    p_loopPrint = true;
                    InitArgs.Args.DebugPrintLine("进程操作类是null");
                    return;
                }
                //获取值
                var value = modifyItem.Value;
                DNum reValue;
                if (value is null)
                {
                    //初次打开
                    if (f_read(pro, ptr, dataType, out reValue))
                    {
                        value = new NumGeneratorValue(reValue);
                        modifyItem.Value = value;
                    }
                    else
                    {
                        p_loopPrint = true;
                        InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:无法读取地址值");
                        return;
                    }
                }

                var tb = modifyItem.TernaryConditionInvoke(false);
                if (tb.HasValue)
                {
                    //满足条件
                    if (tb.Value)
                    {
                        modifyItem.ViewCondition = ModifyItem.ViewConditionYes;
                       // goto IsOper;
                    }
                    else
                    {
                        //未满足条件
                        //play = false;
                        modifyItem.ViewCondition = ModifyItem.ViewConditionNo;
                        goto isOperOver;
                    }
                }
                else
                {
                    //无条件
                    //play = true;
                    //setValue = value.Generate();
                    //f_write(pro, ptr, dataType, setValue);
                    //modifyItem.ViewValue = setValue.ToString();
                    modifyItem.ViewCondition = ModifyItem.ViewConditionNone;
                    //goto IsOper;
                }

                IsOper:

                {
                    //读取当前值
                    if (f_read(pro, ptr, dataType, out reValue))
                    {
                        //
                        var nowValue = modifyItem.Value.Generate();
                        if (comparer.Compare(nowValue, reValue) < 0)
                        {
                            //当前保存的值"小于"内存
                            setValue = reValue;
                            //保存新的值
                            modifyItem.Value = new NumGeneratorValue(reValue);
                        }
                        else
                        {
                            setValue = nowValue;
                        }
                    }
                    else
                    {
                        p_loopPrint = true;
                        InitArgs.Args.DebugPrintLine($"{modifyItem.Text}:无法读取地址值");
                        return;
                    }
                    //play = true;
                    //setValue = value.Generate();
                    f_write(pro, ptr, dataType, setValue);
                    modifyItem.ViewValue = setValue.ToString();
                }

                isOperOver:;


            }

        }

        #endregion

        private void f_update()
        {

            var mods = p_mod.ProcessModifys.ModifyList;

            try
            {
                int length = mods.Count;

                for (int i = 0; i < length; i++)
                {
                    var mod = mods[i];

                    if (mod is null) continue;

                    var modType = mod.ModifyType;
                    try
                    {
                        if(modType == ModifyAddressType.FixedSelf)
                        {
                            f_modFixedSelf(mod);
                        }
                        else if (modType == ModifyAddressType.FixedSelfUp)
                        {
                            //InitArgs.Args.DebugPrintLine("即将进行一次 fixedSelfUp 修改");
                            f_modFixedSelfUpAndDown(mod, p_defDNumComp);
                        }
                        else if (modType == ModifyAddressType.FixedSelfDown)
                        {
                            //InitArgs.Args.DebugPrintLine("即将进行一次 fixedSelfUp 修改");
                            f_modFixedSelfUpAndDown(mod, p_revDNumComp);
                        }
                        else
                        {
                            f_modGR(mod, modType);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        if (InitArgs.Args.CanDebug)
                        {
                            p_loopPrint = true;
                            StringBuilder sb = new StringBuilder(64);
                            var args = InitArgs.Args;
                            InitArgs.PrintException(ex, sb);
                            args.DebugPrintLineF(sb.ToString());
                        }
                    }
                   
                }

            }
            catch (Exception ex)
            {
                if (InitArgs.Args.CanDebug)
                {
                    p_loopPrint = true;
                    StringBuilder sb = new StringBuilder(64);
                    var args = InitArgs.Args;
                    InitArgs.PrintException(ex, sb);
                    args.DebugPrintLineF(sb.ToString());
                }
            }

        }

        #endregion

        #region 热键功能

        /// <summary>
        /// 判断触发热键
        /// </summary>
        /// <param name="item"></param>
        /// <param name="downs">带判断的热键集合</param>
        /// <returns>是否处于成功触发状态</returns>
        private bool f_amodByHotkey(ModifyItem item, IList<Keys> downs)
        {
            var hs = item.HotKeys;

            int hsLen = hs.Count;

            if (hsLen != downs.Count)
            {
                //键数不同
                return false;
            }

            for (int i = 0; i < hsLen; i++)
            {
                if (hs[i] != downs[i])
                {
                    //不同
                    return false;
                }
            }

            //热键相同，执行
            //item.SwitchToggle();
            return true;

        }

        private void f_hotkeyUpdate()
        {
            if (p_keyDown && p_mod.IsOpenSetup)
            {
                //热键状态更新
                var args = InitArgs.Args;
                p_nowDowns.Clear();
                args.AppendNowHotkeysDown(p_nowDowns);
                //p_nowDowns.Sort(args.comparerKeys);

                var proMod = p_mod.ProcessModifys;

                var modList = proMod.ModifyList;

                lock (modList)
                {
                    int length = modList.Count;
                    for (int i = 0; i < length; i++)
                    {
                        var mod = modList[i];
                        if (mod.HotKeys.Count != 0)
                        {
                            if(f_amodByHotkey(mod, p_nowDowns))
                            {
                                //热键触发

                                var mt = mod.ModifyType;
                                if (mt == ModifyAddressType.Once || mt == ModifyAddressType.Add || mt == ModifyAddressType.Sub)
                                {
                                    if (!p_keyDownAudioPlayOver_Open)
                                    {
                                        p_keyDownAudioPlayOver_Open = true;
                                        args.PlayAudioCheckToggle(true);
                                    }
                                    mod.Toggle = true;
                                }
                                else
                                {
                                    if (mod.Toggle)
                                    {
                                        if (!p_keyDownAudioPlayOver_Close)
                                        {
                                            p_keyDownAudioPlayOver_Close = true;
                                            args.PlayAudioCheckToggle(false);
                                        }

                                        mod.Toggle = false;
                                    }
                                    else
                                    {
                                        if (!p_keyDownAudioPlayOver_Open)
                                        {
                                            p_keyDownAudioPlayOver_Open = true;
                                            args.PlayAudioCheckToggle(true);
                                        }

                                        mod.Toggle = true;
                                    }
                                }
                               
                                //mod.SwitchToggle();
                            }
                        }
                    }
                }
               

            }
        }

        #endregion

        #endregion

        protected override void LoopStartInvoke()
        {
            var args = InitArgs.Args;
            p_mod = args.processModify;
            args.KeyEvent += fe_keyHook;
            //p_keyDownInvokePlayAudio_Open = false;
            p_keyDownAudioPlayOver_Open = false;
        }

        protected override void LoopFirst()
        {
            p_keyDownAudioPlayOver_Open = false;
            p_keyDownAudioPlayOver_Close = false;
            //p_keyDownInvokePlayAudio_Open = false;
            //p_keyDownInvokePlayAudio_Close = false;

            if (p_keyEventDown)
            {
                //接收事件按键后的首个轮循添加按下状态判断
                p_keyDown = true;
                p_keyEventDown = false;
            }
        }

        protected override void Update()
        {

            f_hotkeyUpdate();

            if (p_mod.IsNotDispose && p_mod.IsOpenProcess)
            {
                //进程打开了
                if ((p_mod.ProOperation.Process?.HasExited).GetValueOrDefault(true))
                {
                    //实际进程终止
                    //关闭进程
                    try
                    {
                        p_mod.CloseProcess();
                        p_loopPrint = true;
                        InitArgs.Args.DebugPrintLine("要操作的进程已经终止运行");
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (p_mod.IsNotDispose && p_mod.IsOpenProcess && p_mod.IsOpenSetup)
            {
                f_update();
            }

        }

        protected override void LoopEnd()
        {
            if (p_loopPrint)
            {
                p_loopPrint = false;
                InitArgs.Args.DebugFlush();
            }

            p_keyDown = false;
        }

        protected override void FixedUpdate()
        {
        }

        protected override void ExitLoop()
        {
            //var args = InitArgs.Args;
            //args.KeyEvent -= fe_keyHook;
        }

        #endregion

        #endregion

    }

}
