using Cheng.DataStructure.NumGenerators;
using Cheng.Json;
using Cheng.Json.GeneratorNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cheng.ModifyMaster
{

    /*
    type可填如下参数：
    "fixed" => 表示将其地址的值固定为value参数，使用选项或热键控制开关
    "fixedUp" => 表示不让其地址的值小于value，使用按钮或热键控制开关
    "fixedDown" => 表示不让其地址的值大于value，使用按钮或热键控制开关
    "add" => 表示每修改一次在值的基础上添加value，使用按钮或热键修改一次
    "sub" => 表示每修改一次在值的基础上减少value，使用按钮或热键修改一次
    "fixedSelf" => 表示在打开开关的一刻，将值固定为当前值（忽略value参数）
    "fixedSelfUp" => 表示在打开开关的一刻，将值固定为不小于当前值的值（忽略value参数）
    "fixedSelfDown" => 表示在打开开关的一刻，将值固定为不大于当前值的值（忽略value参数）
    */

    /// <summary>
    /// 地址修改方式
    /// </summary>
    public enum ModifyAddressType
    {
        /// <summary>
        /// 固定为value
        /// </summary>
        Fixed,

        /// <summary>
        /// 表示不让其地址的值小于value
        /// </summary>
        FixedUp,

        /// <summary>
        /// 表示不让其地址的值大于value
        /// </summary>
        FixedDown,

        /// <summary>
        /// 每修改一次都会将值设置为value
        /// </summary>
        Once,

        /// <summary>
        /// 每修改一次在值的基础上添加value
        /// </summary>
        Add,

        /// <summary>
        /// 每修改一次在值的基础上减少value
        /// </summary>
        Sub,

        /// <summary>
        /// 将值固定为当前值（忽略value参数）
        /// </summary>
        FixedSelf,

        /// <summary>
        /// 将值固定为当前值并不锁定增加（忽略value参数）
        /// </summary>
        FixedSelfUp,

        /// <summary>
        /// 将值固定为当前值并不锁定减少（忽略value参数）
        /// </summary>
        FixedSelfDown,
    }

    public static class ModifyAddressTypeExtend
    {

        /// <summary>
        /// 当前值是否为Self类型（忽略value值的类型）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSelfValue(this ModifyAddressType type)
        {
            switch (type)
            {
                case ModifyAddressType.FixedSelf:
                case ModifyAddressType.FixedSelfUp:
                case ModifyAddressType.FixedSelfDown:
                    return true;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// address项
    /// </summary>
    public struct ModifyAddress
    {

        public ModifyAddress(string id, ModifyAddressType type)
        {
            this.id = id;
            addressType = type;
        }

        /// <summary>
        /// 地址id
        /// </summary>
        public string id;

        /// <summary>
        /// 值修改类型
        /// </summary>
        public ModifyAddressType addressType;

    }

    /// <summary>
    /// 条目UI
    /// </summary>
    public struct ModifyUI
    {
        /// <summary>
        /// 条目名称
        /// </summary>
        public string text;

        /// <summary>
        /// 条目悬浮框提示，可null
        /// </summary>
        public string promptWindow;

    }

    /// <summary>
    /// 一个修改条目
    /// </summary>
    public class ModifyItem
    {

        #region

        /// <summary>
        /// 实例化一个修改条目
        /// </summary>
        public ModifyItem()
        {
            p_hotkeys = new List<Keys>();
            Address = default;
            ItemUI = default;
            p_viewValue = string.Empty;
            p_viewCond = string.Empty;
            p_keyName = string.Empty;
        }

        #endregion

        #region

        private ModifyAddress p_address;

        /// <summary>
        /// 地址address所在值的类型
        /// </summary>
        private DataType p_dataType;

        /// <summary>
        /// value项，可null
        /// </summary>
        private NumGenerator p_value;

        private ModifyUI p_UI;

        private string p_keyName;

        /// <summary>
        /// 条件，可null
        /// </summary>
        private BaseCondition p_condition;

        /// <summary>
        /// 热键组合，可空集合
        /// </summary>
        private List<Keys> p_hotkeys;

        /// <summary>
        /// 值视图缓存
        /// </summary>
        private string p_viewValue;

        /// <summary>
        /// 是否开启
        /// </summary>
        private bool p_toggle;

        /// <summary>
        /// 条件参数的显示视图
        /// </summary>
        private string p_viewCond;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 项的key值
        /// </summary>
        public string KeyName
        {
            get => p_keyName;
            set
            {
                p_keyName = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 是否开启该条目修改
        /// </summary>
        public bool Toggle
        {
            get => p_toggle;
            set
            {
                if (!value)
                {
                    //关闭
                    //self类型重置恒定值依赖
                    switch (ModifyType)
                    {
                        case ModifyAddressType.FixedSelf:
                        case ModifyAddressType.FixedSelfUp:
                        case ModifyAddressType.FixedSelfDown:
                            Value = null;
                            break;
                    }
                }
                p_toggle = value;
            }
        }

        /// <summary>
        /// 条目address项
        /// </summary>
        public ModifyAddress Address
        {
            get => p_address;
            set
            {
                p_address = value;
                if (p_address.id is null) p_address.id = string.Empty;
            }
        }

        /// <summary>
        /// 要修改时的修改类型
        /// </summary>
        public ModifyAddressType ModifyType
        {
            get => p_address.addressType;
            set
            {
                p_address.addressType = value;
            }
        }

        /// <summary>
        /// 条目要修改的地址ID
        /// </summary>
        public string AddressID
        {
            get => p_address.id;
            set
            {
                p_address.id = value ?? string.Empty;
            }
        }

        /// <summary>
        /// address项对应的值类型
        /// </summary>
        public DataType AddressDataType
        {
            get => p_dataType;
            set => p_dataType = value;
        }

        /// <summary>
        /// value项获取器，可null
        /// </summary>
        public NumGenerator Value
        {
            get => p_value;
            set
            {
                p_value = value;
            }
        }

        /// <summary>
        /// 条件函数，可null
        /// </summary>
        public BaseCondition Condition
        {
            get => p_condition;
            set
            {
                p_condition = value;
            }
        }

        /// <summary>
        /// 条目UI
        /// </summary>
        public ModifyUI ItemUI
        {
            get => p_UI;
            set
            {
                if (value.text is null) p_UI.text = string.Empty;
                if (value.promptWindow is null) p_UI.promptWindow = string.Empty;
                p_UI = value;
            }
        }

        /// <summary>
        /// 条目UI的text名称
        /// </summary>
        public string Text
        {
            get => p_UI.text;
            set
            {
                p_UI.text = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 条目UI的悬浮框文本
        /// </summary>
        public string PromptWindow
        {
            get => p_UI.promptWindow;
            set
            {
                p_UI.promptWindow = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 条目的热键组合，空集合表示没有热键
        /// </summary>
        public List<Keys> HotKeys
        {
            get => p_hotkeys;
        }

        /// <summary>
        /// 需要显示在UI视图上的条目当前值文本
        /// </summary>
        public string ViewValue
        {
            get => p_viewValue;
            set
            {
                p_viewValue = value ?? string.Empty;
            }
        }

        /// <summary>
        /// 条件参数的显示视图缓存
        /// </summary>
        public string ViewCondition
        {
            get => p_viewCond;
            set => p_viewCond = value ?? string.Empty;
        }

        public const string ViewConditionYes = "满足条件";

        public const string ViewConditionNo = "未满足条件";

        public const string ViewConditionNone = "无条件";

        #endregion

        #region 功能封装

        /// <summary>
        /// 创建条件参数
        /// </summary>
        /// <param name="condJson"></param>
        /// <param name="jsonValueGenerator"></param>
        /// <returns>创建失败返回null</returns>
        static BaseCondition CreateCondition(JsonVariable condJson, JsonValueGenerator jsonValueGenerator)
        {

            try
            {
                /*
                "eq": 判断 x = y
                "noteq": 判断 x ≠ y
                "greater": 判断 x > y
                "less": 判断 x < y
                "greq": 判断 x >= y
                "lesseq": 判断 x <= y
                */
                NumCondition nc;
                var jdCond = condJson.JsonObject;
                var typeStr = jdCond["type"].String;

                switch (typeStr)
                {
                    case "eq":
                        nc = new NumCondition();
                        nc.ConditionType = NumCondition.CondType.Equal;
                        break;
                    case "noteq":
                        nc = new NumCondition();
                        nc.ConditionType = NumCondition.CondType.NotEqual;
                        break;
                    case "greater":
                        nc = new NumCondition();
                        nc.ConditionType = NumCondition.CondType.Greater;
                        break;
                    case "less":
                        nc = new NumCondition();
                        nc.ConditionType = NumCondition.CondType.Less;
                        break;
                    case "greq":
                        nc = new NumCondition();
                        nc.ConditionType = NumCondition.CondType.GreaterEqual;
                        break;
                    case "lesseq":
                        nc = new NumCondition();
                        nc.ConditionType = NumCondition.CondType.LessEqual;
                        break;
                    default:
                        goto NotBase;
                }

                nc.X = jsonValueGenerator.JsonToGenerator(jdCond["x"]);
                nc.Y = jsonValueGenerator.JsonToGenerator(jdCond["y"]);

                return nc;

                NotBase:

                MultCondition mc = new MultCondition();
                switch (typeStr)
                {
                    case "and":
                        mc.MultConditionType = MultCondition.MultType.And;
                        break;
                    case "or":
                        mc.MultConditionType = MultCondition.MultType.Or;
                        break;
                    case "xor":
                        mc.MultConditionType = MultCondition.MultType.Xor;
                        break;
                    case "nxor":
                        mc.MultConditionType = MultCondition.MultType.NXor;
                        break;
                    case "neg":
                        mc.MultConditionType = MultCondition.MultType.Neg;
                        break;
                    default:
                        return null;
                }

                mc.X = CreateCondition(jdCond["x"], jsonValueGenerator);
                mc.Y = CreateCondition(jdCond["y"], jsonValueGenerator);
                return mc;

            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 采用一个json修改条目初始化当前实例
        /// </summary>
        /// <param name="json"></param>
        /// <param name="itemKey">项key</param>
        /// <param name="jsonValueGenerator">值结构转化器</param>
        /// <param name="exception">可能引发的异常，没有则null</param>
        /// <returns>是否初始化成功</returns>
        public bool JsonToInit(JsonDictionary json, JsonValueGenerator jsonValueGenerator, string itemKey, out Exception exception)
        {
            if (json is null || jsonValueGenerator is null) throw new ArgumentNullException();
            exception = null;
            try
            {
                KeyName = itemKey;
                Toggle = false;
                //地址项
                var jaddress = json["address"].JsonObject;
                AddressID = jaddress["id"].String;
                /*
                "fixed" => 表示将其地址的值固定为value参数，使用选项或热键控制开关
                "fixedUp" => 表示不让其地址的值小于value，使用按钮或热键控制开关
                "fixedDown" => 表示不让其地址的值大于value，使用按钮或热键控制开关
                "add" => 表示每修改一次在值的基础上添加value，使用按钮或热键修改一次
                "sub" => 表示每修改一次在值的基础上减少value，使用按钮或热键修改一次
                "fixedSelf" => 表示在打开开关的一刻，将值固定为当前值（忽略value参数）
            "fixedSelfUp" => 表示在打开开关的一刻，将值固定为不小于当前值的值（忽略value参数）
            "fixedSelfDown" => 表示在打开开关的一刻，将值固定为不大于当前值的值（忽略value参数）
                */
                switch (jaddress["type"].String)
                {
                    case "fixed":
                        ModifyType = ModifyAddressType.Fixed;
                        break;
                    case "fixedUp":
                        ModifyType = ModifyAddressType.FixedUp;
                        break;
                    case "fixedDown":
                        ModifyType = ModifyAddressType.FixedDown;
                        break;
                    case "once":
                        ModifyType = ModifyAddressType.Once;
                        break;
                    case "add":
                        ModifyType = ModifyAddressType.Add;
                        break;
                    case "sub":
                        ModifyType = ModifyAddressType.Sub;
                        break;
                    case "fixedSelf":
                        ModifyType = ModifyAddressType.FixedSelf;
                        break;
                    case "fixedSelfUp":
                        ModifyType = ModifyAddressType.FixedSelfUp;
                        break;
                    case "fixedSelfDown":
                        ModifyType = ModifyAddressType.FixedSelfDown;
                        break;
                    default:
                        return false;
                }

                /*
                决定要修改的数据的类型和字节长度
                int32 => 表示4字节整形
                uint32 => 表示4字节无符号整形
                int64 => 表示8字节整形
                uint64 => 表示8字节无符号整形
                float => 表示单精度浮点型（4个字节）
                double => 表示双精度浮点型（8个字节）
                int16 => 表示2字节整形
                uint16 => 表示2字节无符号整形
                byte => 表示单个字节的整数
                */

                switch (json["dataType"].String)
                {
                    case "int32":
                        AddressDataType = DataType.Int32;
                        break;
                    case "uint32":
                        AddressDataType = DataType.UInt32;
                        break;
                    case "int64":
                        AddressDataType = DataType.Int64;
                        break;
                    //case "uint64":
                    //    AddressDataType = DataType.UInt64;
                    //  break;
                    case "float":
                        AddressDataType = DataType.Float;
                        break;
                    case "double":
                        AddressDataType = DataType.Double;
                        break;
                    case "int16":
                        AddressDataType = DataType.Int16;
                        break;
                    case "uint16":
                        AddressDataType = DataType.UInt16;
                        break;
                    case "byte":
                        AddressDataType = DataType.Byte;
                        break;
                    default:
                        return false;
                }

                //value参数
                Value = null;

                if (ModifyType != ModifyAddressType.FixedSelf)
                {
                    //获取值结构
                    if (json.TryGetValue("value", out var valueJson))
                    {
                        Value = jsonValueGenerator.JsonToGenerator(valueJson);
                    }
                }

                ItemUI = default;
                Text = itemKey;
                
                try
                {
                    if (json.TryGetValue("UI", out var UIjson))
                    {
                        /*
                        "text": "条目名称",
                        "promptWindow": "条目提示的悬浮窗口文本"
                        */
                        var uiObj = UIjson.JsonObject;

                        Text = uiObj["text"].String;

                        if (uiObj.TryGetValue("promptWindow", out var prompt))
                        {
                            PromptWindow = prompt.String;
                        }
                    }
                }
                catch (Exception)
                {
                    Text = itemKey;
                }

                Condition = null;
                ViewCondition = ViewConditionNone;
                try
                {
                    if (json.TryGetValue("condition", out var condJson))
                    {
                        Condition = CreateCondition(condJson, jsonValueGenerator);
                    }
                }
                catch (Exception)
                {
                    Condition = null;
                    //throw;
                }


                HotKeys.Clear();
                if (json.TryGetValue("hotkeys", out var hotkeysJson))
                {
                    if (!InitArgs.JsonHotkeysInit(hotkeysJson, HotKeys))
                    {
                        HotKeys.Clear();
                    }
                }

                var mte = ModifyType;
                if(mte != ModifyAddressType.Add && mte != ModifyAddressType.Once && mte != ModifyAddressType.Sub)
                {
                    if (json.TryGetValue("defaultToggle", out var defOpen))
                    {
                        if (defOpen.DataType == JsonType.Boolean)
                        {
                            Toggle = defOpen.Boolean;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }

        }

        /// <summary>
        /// 返回该项条件参数，无条件或函数执行出错则直接返回true
        /// </summary>
        /// <returns></returns>
        public bool ConditionInvoke()
        {
            return (p_condition?.TryCondition(true)).GetValueOrDefault(true);
        }

        /// <summary>
        /// 返回该项条件参数
        /// </summary>
        /// <param name="notConditionValue">如果没有条件则返回该值</param>
        /// <param name="errorValue">条件函数执行时出现错误返回该参数</param>
        /// <returns></returns>
        public bool ConditionInvoke(bool notConditionValue, bool errorValue)
        {
            return (p_condition?.TryCondition(errorValue)).GetValueOrDefault(notConditionValue);
        }

        /// <summary>
        /// 返回该项条件参数
        /// </summary>
        /// <param name="errorValue">条件函数执行时出现错误返回该参数</param>
        /// <returns>返回true表示符合条件，false表示不符合条件，null表示没有条件</returns>
        public bool? TernaryConditionInvoke(bool errorValue)
        {
            return (p_condition?.TryCondition(errorValue));
        }

        /// <summary>
        /// 根据开关和条件参数，判断是否应该实施动作
        /// </summary>
        /// <returns></returns>
        public bool CanModifys()
        {
            return Toggle && (p_condition?.TryCondition(false)).GetValueOrDefault(true);
        }

        /// <summary>
        /// 切换条目启用开关的状态
        /// </summary>
        public void SwitchToggle()
        {
            Toggle = !Toggle;
        }

        /// <summary>
        /// 根据修改类型动态启动开关
        /// </summary>
        public void ToggleOnActive()
        {
            var mt = this.ModifyType;
            if(mt == ModifyAddressType.Once || mt == ModifyAddressType.Add || mt == ModifyAddressType.Sub)
            {
                Toggle = true;
            }
            else
            {
                Toggle = !Toggle;
            }

        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 修改项集合
    /// </summary>
    public class Modifys
    {

        #region

        /// <summary>
        /// 实例化修改项集合
        /// </summary>
        public Modifys()
        {
            p_list = new List<ModifyItem>();
        }

        #endregion

        #region

        private List<ModifyItem> p_list;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 获取修改条目集合
        /// </summary>
        public List<ModifyItem> ModifyList
        {
            get => p_list;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 清空当前列表内所有条目
        /// </summary>
        public void Clear()
        {
            lock(p_list) p_list.Clear();
        }

        /// <summary>
        /// 使用json的modifier项重新初始化参数列表
        /// </summary>
        /// <param name="json">json的modifier列表</param>
        /// <param name="jsonValueGenerator">json值类型转化器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void JsonInitList(JsonDictionary json, JsonValueGenerator jsonValueGenerator)
        {
            if (json is null || jsonValueGenerator is null) throw new ArgumentNullException();
            p_list.Clear();
            ModifyItem mod;
            bool flush = false;
            foreach (var item in json)
            {
                if (item.Value.DataType == JsonType.Dictionary)
                {
                    mod = new ModifyItem();
                    if (mod.JsonToInit(item.Value.JsonObject, jsonValueGenerator, item.Key, out var exc))
                    {
                        p_list.Add(mod);
                    }
                    if (exc != null)
                    {
                        flush = true;
                        InitArgs.Args.DebugPrintLine(InitArgs.ExceptionText(exc));
                    }
                }
            }
            if (flush && InitArgs.Args.CanDebug)
            {
                InitArgs.Args.DebugFlush();
            }
        }

        #endregion

        #endregion

    }

}
