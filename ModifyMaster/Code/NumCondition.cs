using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cheng.DataStructure.NumGenerators;

namespace Cheng.ModifyMaster
{

    public abstract class BaseCondition
    {

        public abstract bool Condition();

        public bool TryCondition(bool errorValue)
        {
            try
            {
                return Condition();
            }
            catch (Exception)
            {
                return errorValue;
            }
        }
    }

    /// <summary>
    /// 数值生成器条件判断
    /// </summary>
    public class NumCondition : BaseCondition
    {

        #region 结构

        private class ConstNumGenerator : NumGenerator
        {

            public ConstNumGenerator(DynamicNumber num)
            {
                this.num = num;
            }

            private readonly DynamicNumber num;

            public override DynamicNumber Generate()
            {
                return num;
            }
        }

        /// <summary>
        /// 条件判断类型
        /// </summary>
        public enum CondType
        {
            /// <summary>
            /// 无条件返回true
            /// </summary>
            None = 0,

            /// <summary>
            /// x == y
            /// </summary>
            Equal = 1,

            /// <summary>
            /// x != y
            /// </summary>
            NotEqual,

            /// <summary>
            /// <![CDATA[x > y]]>
            /// </summary>
            Greater,

            /// <summary>
            /// <![CDATA[x < y]]>
            /// </summary>
            Less,

            /// <summary>
            /// <![CDATA[x >= y]]>
            /// </summary>
            GreaterEqual,

            /// <summary>
            /// <![CDATA[x <= y]]>
            /// </summary>
            LessEqual
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化
        /// </summary>
        public NumCondition()
        {
            p_type = CondType.None;
            p_x = defaultNum;
            p_y = defaultNum;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="x">参数x</param>
        /// <param name="y">参数y</param>
        public NumCondition(NumGenerator x, NumGenerator y)
        {
            p_type = CondType.None;
            p_x = x ?? defaultNum;
            p_y = y ?? defaultNum;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="type">条件判断类别</param>
        /// <param name="x">参数x</param>
        /// <param name="y">参数y</param>
        public NumCondition(CondType type, NumGenerator x, NumGenerator y)
        {
            p_type = type;
            p_x = x ?? defaultNum;
            p_y = y ?? defaultNum;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="type">条件判断类别</param>
        public NumCondition(CondType type)
        {
            p_type = type;
            p_x = defaultNum;
            p_y = defaultNum;
        }

        #endregion

        #region

        private readonly static NumGenerator defaultNum = new ConstNumGenerator(0);

        private CondType p_type;

        private NumGenerator p_x;

        private NumGenerator p_y;

        #endregion

        #region 功能

        /// <summary>
        /// x参数
        /// </summary>
        /// <exception cref="ArgumentNullException">设为null</exception>
        public NumGenerator X
        {
            get => p_x;
            set
            {
                p_x = value ?? defaultNum;
            }
        }

        /// <summary>
        /// y参数
        /// </summary>
        /// <exception cref="ArgumentNullException">设为null</exception>
        public NumGenerator Y
        {
            get => p_y;
            set
            {
                p_y = value ?? defaultNum;
            }
        }

        /// <summary>
        /// 条件判断类别
        /// </summary>
        public CondType ConditionType
        {
            get => p_type;
            set => p_type = value;
        }

        /// <summary>
        /// 返回此刻条件函数值
        /// </summary>
        /// <returns>条件布尔值</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool Condition()
        {
            switch (p_type)
            {
                case CondType.Equal:
                    return p_x.Generate() == p_y.Generate();
                case CondType.NotEqual:
                    return p_x.Generate() != p_y.Generate();
                case CondType.Greater:
                    return p_x.Generate() > p_y.Generate();
                case CondType.Less:
                    return p_x.Generate() < p_y.Generate();
                case CondType.GreaterEqual:
                    return p_x.Generate() >= p_y.Generate();
                case CondType.LessEqual:
                    return p_x.Generate() <= p_y.Generate();
                default:
                    return true;
            }
        }


        #endregion

    }

    /// <summary>
    /// 多条件判断
    /// </summary>
    public class MultCondition : BaseCondition
    {

        #region 结构

        /// <summary>
        /// 多条件参数类型
        /// </summary>
        public enum MultType
        {
            /// <summary>
            /// 与
            /// </summary>
            And,
            /// <summary>
            /// 或
            /// </summary>
            Or,
            /// <summary>
            /// 异或
            /// </summary>
            Xor,
            /// <summary>
            /// 同或
            /// </summary>
            NXor,
            /// <summary>
            /// 取反x
            /// </summary>
            Neg
        }

        #endregion

        #region 参数

        public MultCondition()
        {
        }

        #endregion

        #region 参数

        private BaseCondition p_x;
        private BaseCondition p_y;
        private MultType p_type;

        #endregion

        #region

        /// <summary>
        /// x 参数
        /// </summary>
        public BaseCondition X
        {
            get => p_x;
            set => p_x = value;
        }

        /// <summary>
        /// y 参数
        /// </summary>
        public BaseCondition Y
        {
            get => p_y;
            set => p_y = value;
        }

        /// <summary>
        /// 多条件判断方式
        /// </summary>
        public MultType MultConditionType
        {
            get => p_type;
            set => p_type = value;
        }

        public override bool Condition()
        {
            switch (p_type)
            {
                case MultType.And:
                    return p_x.Condition() && p_y.Condition();
                case MultType.Or:
                    return p_x.Condition() || p_y.Condition();
                case MultType.Xor:
                    return p_x.Condition() != p_y.Condition();
                case MultType.NXor:
                    return p_x.Condition() == p_y.Condition();
                case MultType.Neg:
                    return !p_x.Condition();
                default:
                    throw new ArgumentException($"多条件参数指示出错:{p_type.ToString()}");
            }
        }

        #endregion

    }

}
