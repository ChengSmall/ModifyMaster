using Cheng.Json.GeneratorNumbers;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.DataStructure.NumGenerators;
using Cheng.Json;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 值获取结构解析器 - 修改器扩展
    /// </summary>
    public class JsonValueGeneratorMod : JsonValueGenerator
    {

        public JsonValueGeneratorMod(ProcessModify modify)
        {
            p_mod = modify ?? throw new ArgumentNullException();
        }

        #region

        private ProcessModify p_mod;

        #endregion

        #region 派生

        protected override NumGenerator OtherJsonToNum(JsonDictionary json)
        {
            try
            {
                var type = json["type"];

                if (type.String == "address")
                {
                    NumGeneratorModify nmod = new NumGeneratorModify(p_mod);
                    //地址id
                    var id = json["id"];
                    nmod.AddressID = id.String;

                    var daType = json["dataType"];

                    /*
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

                    switch (daType.String)
                    {
                        case "int32":
                            nmod.DataType = DataType.Int32;
                            break;
                        case "uint32":
                            nmod.DataType = DataType.UInt32;
                            break;
                        case "int64":
                            nmod.DataType = DataType.Int64;
                            break;
                        //case "uint64":
                        //    nmod.DataType = DataType.UInt64;
                        //    break;
                        case "float":
                            nmod.DataType = DataType.Float;
                            break;
                        case "double":
                            nmod.DataType = DataType.Double;
                            break;
                        case "int16":
                            nmod.DataType = DataType.Int16;
                            break;
                        case "uint16":
                            nmod.DataType = DataType.UInt16;
                            break;
                        default:
                            nmod.DataType = DataType.Byte;
                            break;
                    }

                    return nmod;
                }

                return defaultNumGenerator;
            }
            catch (Exception)
            {
                return defaultNumGenerator;
            }
        }

        #endregion

    }

}
