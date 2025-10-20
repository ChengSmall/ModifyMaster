using Cheng.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 语言包语言包文件索引保存参数
    /// </summary>
    public struct LanguageInf
    {

        /// <summary>
        /// 语言包文件名
        /// </summary>
        public string languageFileName;

        /// <summary>
        /// 语言包友好名
        /// </summary>
        public string languageDisplayName;

        /// <summary>
        /// 将参数保存到json对象
        /// </summary>
        /// <returns></returns>
        public JsonDictionary SaveToJson()
        {
            JsonDictionary jd = new JsonDictionary(2);
            jd.Add("displayName", languageDisplayName);
            jd.Add("fileName", languageFileName);
            return jd;
        }

        /// <summary>
        /// 从json创建语言包文件索引
        /// </summary>
        /// <param name="json"></param>
        /// <param name="inf"></param>
        /// <returns></returns>
        public static bool CreateByJson(JsonVariable json, out LanguageInf inf)
        {
            try
            {
                inf = new LanguageInf();
                var jd = json.JsonObject;
                json = jd["displayName"];
                inf.languageDisplayName = json.String;

                json = jd["fileName"];
                inf.languageFileName = json.String;

                return true;
            }
            catch (Exception)
            {
                inf = default;
                return false;
            }

        }

    }

    /// <summary>
    /// 字体信息
    /// </summary>
    public struct FontInf
    {

        #region

        /// <summary>
        /// 名称 - string
        /// </summary>
        public string familyName;

        /// <summary>
        /// 大小 - double
        /// </summary>
        public float emSize;

        /// <summary>
        /// 样式 - 4字节位值
        /// </summary>
        public FontStyle style;

        /// <summary>
        /// 度量值单位 - string
        /// </summary>
        public GraphicsUnit unit;

        /// <summary>
        /// gdi - int
        /// </summary>
        public byte gdiCharSet;

        /// <summary>
        /// 是否从 GDI 垂直字体派生 - bool
        /// </summary>
        public bool gdiVerticalFont;

        #endregion

        #region json

        /// <summary>
        /// 将当前参数保存到json
        /// </summary>
        /// <returns>保存的json信息</returns>
        public JsonDictionary SaveToJson()
        {
            JsonDictionary jd = new JsonDictionary(6);

            jd.Add("familyName", familyName);
            jd.Add("emSize", emSize);
            jd.Add("style", (int)this.style);
            jd.Add("Unit", unitToString(this.unit));
            jd.Add("GDI", this.gdiCharSet);
            jd.Add("GDIVertica", this.gdiVerticalFont);
            return jd;
        }

        /// <summary>
        /// 用json创建一个FontInf
        /// </summary>
        /// <param name="json">存有FontInf对象的json</param>
        /// <param name="inf">创建的inf</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateByJson(JsonVariable json, out FontInf inf)
        {
            try
            {
                inf = new FontInf();
                var jd = json.JsonObject;

                json = jd["familyName"];
                inf.familyName = json.String;

                json = jd["emSize"];
                inf.emSize = (float)json.RealNum;

                json = jd["style"];
                inf.style = (FontStyle)json.Integer;

                json = jd["Unit"];
                inf.unit = strToUnit(json.String);

                json = jd["GDI"];
                inf.gdiCharSet = (byte)json.Integer;

                json = jd["GDIVertica"];
                inf.gdiVerticalFont = json.Boolean;

                return true;
            }
            catch (Exception)
            {
                inf = default;
                return false;
            }
        }

        static GraphicsUnit strToUnit(string str)
        {
            switch (str)
            {
                case "World":
                    return GraphicsUnit.World;
                case "Display":
                    return GraphicsUnit.Display;
                case "Pixel":
                    return GraphicsUnit.Pixel;
                case "Point":
                    return GraphicsUnit.Point;
                case "Inch":
                    return GraphicsUnit.Inch;
                case "Document":
                    return GraphicsUnit.Document;
                case "Millimeter":
                    return GraphicsUnit.Millimeter;
                default:
                    return GraphicsUnit.Pixel;
            }
        }

        static string unitToString(GraphicsUnit unit)
        {
            switch (unit)
            {
                case GraphicsUnit.World:
                    return "World";
                case GraphicsUnit.Display:
                    return "Display";
                case GraphicsUnit.Pixel:
                    return "Pixel";
                case GraphicsUnit.Point:
                    return "Point";
                case GraphicsUnit.Inch:
                    return "Inch";
                case GraphicsUnit.Document:
                    return "Document";
                case GraphicsUnit.Millimeter:
                    return "Millimeter";
                default:
                    return "Pixel";
            }

        }

        #endregion

        #region font

        /// <summary>
        /// 使用字体信息创建一个字体
        /// </summary>
        /// <returns>错误返回null</returns>
        public Font CreateFont()
        {
            try
            {
                return new Font(this.familyName, emSize, this.style, this.unit, this.gdiCharSet, this.gdiVerticalFont);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 从字体创建字体信息
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static FontInf? CreateInfByFont(Font font)
        {
            try
            {
                if (font is null) return null;
                FontInf inf;
                inf.familyName = font.FontFamily.Name;
                inf.emSize = font.Size;
                inf.style = font.Style;
                inf.unit = font.Unit;
                inf.gdiCharSet = font.GdiCharSet;
                inf.gdiVerticalFont = font.GdiVerticalFont;
                return inf;
            }
            catch (Exception)
            {
                return null;
            }

        }

        #endregion

    }


    /// <summary>
    /// 配置缓存
    /// </summary>
    public class ConfigFile
    {

        #region

        /// <summary>
        /// 语言包位置
        /// </summary>
        public LanguageInf? languageInformation;

        /// <summary>
        /// 窗体字体
        /// </summary>
        public FontInf? formFontInformation;

        #endregion

        #region

        /// <summary>
        /// 将所有参数清空
        /// </summary>
        public void Clear()
        {
            languageInformation = null;
            formFontInformation = null;
        }

        #endregion

        #region json

        /// <summary>
        /// 将参数保存到json
        /// </summary>
        /// <returns></returns>
        public JsonDictionary SaveToJson()
        {
            JsonDictionary jd = new JsonDictionary();

            if (languageInformation.HasValue)
            {
                jd.Add("language", languageInformation.Value.SaveToJson());
            }

            if (formFontInformation.HasValue)
            {
                jd.Add("formFont", formFontInformation.Value.SaveToJson());
            }

            return jd;
        }

        /// <summary>
        /// 将json写入到配置缓存
        /// </summary>
        /// <param name="json"></param>
        public void LoadToJson(JsonVariable json)
        {

            if (json.DataType != JsonType.Dictionary)
            {
                return;
            }

            var jd = json.JsonObject;

            if (jd.TryGetValue("language", out var lan))
            {
                if (LanguageInf.CreateByJson(lan, out var lanInf))
                {
                    this.languageInformation = lanInf;
                }
            }

            if (jd.TryGetValue("formFont", out var formFont))
            {
                if (FontInf.CreateByJson(formFont, out var formFontInf))
                {
                    this.formFontInformation = formFontInf;
                }
            }

        }

        /// <summary>
        /// 将json文件写入到配置缓存
        /// </summary>
        /// <param name="jsonFilePath">要读取的json文件路径</param>
        /// <param name="parser">解析器</param>
        public void LoadToJsonFile(string jsonFilePath, IJsonParser parser)
        {
            JsonVariable json;

            try
            {

                using (FileStream file = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {

                    using (StreamReader sr = new StreamReader(file, Encoding.UTF8, false, 1024, true))
                    {
                        json = parser.ToJsonData(sr);
                    }

                }

                LoadToJson(json);
            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region

        #endregion

    }

}
