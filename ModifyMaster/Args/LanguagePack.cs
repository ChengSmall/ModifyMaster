using Cheng.DataStructure.Collections;
using Cheng.Xmls.StandardItemText;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cheng.ModifyMaster
{

    /// <summary>
    /// 语言包
    /// </summary>
    public class LanguagePack
    {

        #region 构造

        public LanguagePack()
        {
            p_lans = new Dictionary<string, string>(100, new BinaryStringEqualComparer());
        }

        #endregion

        #region 参数

        #region 语言包默认参数

        #region title

        /// <summary>
        /// 窗体标题
        /// </summary>
        public const string def_title = "万能改";

        /// <summary>
        /// 修改器信息名默认值
        /// </summary>
        public const string def_title_information_defname = "未打开修改配置";

        /// <summary>
        /// 存在修改器名时的作者前缀
        /// </summary>
        public const string def_title_information_author = "作者";

        /// <summary>
        /// 修改器进程未打开时默认值
        /// </summary>
        public const string def_title_processNotOpen = "未打开进程";

        /// <summary>
        /// 打开进程时的进程前缀
        /// </summary>
        public const string def_title_openPro_name = "进程";

        #endregion

        #region 项

        /// <summary>
        /// 列 标题
        /// </summary>
        public const string def_column_name = "项";

        /// <summary>
        /// 列 开关标题
        /// </summary>
        public const string def_column_toggle = "开关";

        /// <summary>
        /// 列 修改类型标题
        /// </summary>
        public const string def_column_moftype = "值类型";

        /// <summary>
        /// 修改类型显示文本
        /// </summary>
        public const string def_column_moftype_byte = "字节值";

        /// <summary>
        /// 修改类型显示文本 int16
        /// </summary>
        public const string def_column_moftype_int16 = "16位整数";

        /// <summary>
        /// 修改类型显示文本 uint16
        /// </summary>
        public const string def_column_moftype_uint16 = "16位正整数";

        /// <summary>
        /// 修改类型显示文本
        /// </summary>
        public const string def_column_moftype_int32 = "32位整数";

        /// <summary>
        /// 修改类型显示文本 
        /// </summary>
        public const string def_column_moftype_uint32 = "32位正整数";

        /// <summary>
        /// 修改类型显示文本 
        /// </summary>
        public const string def_column_moftype_int64 = "64位整数";

        /// <summary>
        /// 修改类型显示文本 
        /// </summary>
        public const string def_column_moftype_float = "单浮点";

        /// <summary>
        /// 修改类型显示文本
        /// </summary>
        public const string def_column_moftype_double = "双浮点";

        /// <summary>
        /// 列 值显示标题
        /// </summary>
        public const string def_column_value = "值";

        /// <summary>
        /// 列 条件显示标题
        /// </summary>
        public const string def_column_condition = "条件";

        /// <summary>
        /// 条件未达成时的显示
        /// </summary>
        public const string def_column_condition_off = "条件未达成";

        /// <summary>
        /// 条件达成时的显示
        /// </summary>
        public const string def_column_condition_on = "条件达成";

        #endregion

        #region 菜单栏

        /// <summary>
        /// 菜单栏 - 文件选项
        /// </summary>
        public const string def_menuItem_file = "文件";

        /// <summary>
        /// 菜单栏 - 文件 - 打开配置项
        /// </summary>
        public const string def_menuItem_file_openSetup = "打开";

        /// <summary>
        /// 菜单栏 - 文件 - 打开进程
        /// </summary>
        public const string def_menuItem_file_openProcess = "选择进程";

        /// <summary>
        /// 菜单栏 - 文件 - 输入进程id
        /// </summary>
        public const string def_menuItem_file_inputProID = "输入进程ID";

        /// <summary>
        /// 菜单栏 - 文件 - 退出程序
        /// </summary>
        public const string def_menuItem_file_exit = "退出";

        /// <summary>
        /// 输入进程ID时的窗体标题
        /// </summary>
        public const string def_menuItem_file_openPro_formTitle = "输入要打开的进程ID";

        /// <summary>
        /// 菜单栏 - 选项
        /// </summary>
        public const string def_menuItem_setup = "选项";

        /// <summary>
        /// 菜单栏 - 设置 - 选择字体
        /// </summary>
        public const string def_menuItem_setup_selectFont = "选择字体";

        /// <summary>
        /// 菜单栏 - 设置 - 选择语言
        /// </summary>
        public const string def_menuItem_setup_selectLanguage = "选择语言";

        #endregion

        #region 通用项

        /// <summary>
        /// 确定按钮文本
        /// </summary>
        public const string def_button_ok = "确定";

        /// <summary>
        /// 取消按钮文本
        /// </summary>
        public const string def_button_cancel = "取消";

        /// <summary>
        /// 对话框筛选器 - 修改器配置文件
        /// </summary>
        public const string def_OpenFileDialpgFilter_modconfig = "修改器配置文件";

        /// <summary>
        /// 对话框筛选器 - 所有文件
        /// </summary>
        public const string def_OpenFileDialpgFilter_AllFile = "所有文件";

        /// <summary>
        /// 对话框 - 打开文件对话框标题
        /// </summary>
        public const string def_OpenFileDialpg_title = "选择一个修改配置";

        /// <summary>
        /// 对话框 - 打开字体对话框 - 标题
        /// </summary>
        public const string def_OpenFontDialpg_title = "选择应用字体";

        #endregion

        #endregion

        #region 语言包参数

        private Dictionary<string, string> p_lans;

        /// <summary>
        /// 语言包字典
        /// </summary>
        public Dictionary<string, string> LanguageDict
        {
            get => p_lans;
        }

        /// <summary>
        /// 获取某个key的文本
        /// </summary>
        /// <param name="key"></param>
        /// <returns>null表示没有该项或key是null</returns>
        public string GetLan(string key)
        {
            if (key is null) return null;
            string value;
            if(p_lans.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        #endregion

        #endregion

        #region 功能

        /// <summary>
        /// 使用默认文本初始化
        /// </summary>
        public void InitDefault()
        {
            p_lans["title"] = def_title;
            p_lans["title_information_defname"] = def_title_information_defname;
            p_lans["title_information_author"] = def_title_information_author;
            p_lans["title_processNotOpen"] = def_title_processNotOpen;
            p_lans["title_openPro_name"] = def_title_openPro_name;
            p_lans["menuItem_setup"] = def_menuItem_setup;
            p_lans["menuItem_setup_selectFont"] = def_menuItem_setup_selectFont;
            p_lans["menuItem_setup_selectLanguage"] = def_menuItem_setup_selectLanguage;

            p_lans["column_name"] = def_column_name;
            p_lans["column_toggle"] = def_column_toggle;
            p_lans["column_moftype"] = def_column_moftype;
            p_lans["column_value"] = def_column_value;
            p_lans["column_moftype"] = def_column_moftype;

            p_lans["menuItem_file_openPro_formTitle"] = def_menuItem_file_openPro_formTitle;
            p_lans["menuItem_file"] = def_menuItem_file;
            p_lans["menuItem_file_openSetup"] = def_menuItem_file_openSetup;
            p_lans["menuItem_file_openProcess"] = def_menuItem_file_openProcess;
            p_lans["menuItem_file_inputProID"] = def_menuItem_file_inputProID;
            p_lans["menuItem_file_exit"] = def_menuItem_file_exit;

            p_lans["column_condition_off"] = def_column_condition_off;
            p_lans["column_condition_on"] = def_column_condition_on;

            p_lans["column_moftype_byte"] = def_column_moftype_byte;
            p_lans["column_moftype_int16"] = def_column_moftype_int16;
            p_lans["column_moftype_uint16"] = def_column_moftype_uint16;
            p_lans["column_moftype_int32"] = def_column_moftype_int32;
            p_lans["column_moftype_uint32"] = def_column_moftype_uint32;
            p_lans["column_moftype_int64"] = def_column_moftype_int64;
            p_lans["column_moftype_float"] = def_column_moftype_float;
            p_lans["column_moftype_double"] = def_column_moftype_double;

            p_lans["button_ok"] = def_button_ok;
            p_lans["button_cancel"] = def_button_cancel;

            p_lans["OpenFileDialpg_title"] = def_OpenFileDialpg_title;
            //p_lans["OpenFontDialpg_title"] = def_OpenFontDialpg_title;
        }

        static string getText(XmlStandardItemText parser, XmlNode node, StringBuilder buffer, string defText)
        {
            if (node is null) return defText;
            buffer.Clear();
            parser.XmlToStandardItemText(node, buffer);
            return buffer.ToString();
        }

        /// <summary>
        /// 从xml初始化本地化语言包
        /// </summary>
        /// <param name="xml">xml文档</param>
        /// <param name="parser">文本解析器</param>
        /// <exception cref="Exception">无异常处理</exception>
        public void InitByXml(XmlDocument xml, XmlStandardItemText parser)
        {
            var root = xml?.DocumentElement;
            if (root is null || parser is null) throw new ArgumentNullException();

            StringBuilder sb = new StringBuilder();

            #region 修改标题

            p_lans["title"] = getText(parser, root["title"], sb, def_title);

            p_lans["title_information_defname"] = getText(parser, root["title_information_defname"],
                sb, def_title_information_defname);

            p_lans["title_information_author"] = getText(parser, root["title_information_author"],
                sb, def_title_information_author);

            p_lans["title_processNotOpen"] = getText(parser, root["title_processNotOpen"],
                sb, def_title_processNotOpen);

            p_lans["title_openPro_name"] = getText(parser, root["title_openPro_name"],
                sb, def_title_openPro_name);

            #endregion

            #region 菜单栏

            /*
            <menuItem_file>文件</menuItem_file>
	        <menuItem_file_openSetup>打开</menuItem_file_openSetup>
	        <menuItem_file_openProcess>选择进程</menuItem_file_openProcess>
	        <menuItem_file_openPro_formTitle>输入进程ID</menuItem_file_openPro_formTitle>
	        <menuItem_file_inputProID>打开进程ID</menuItem_file_inputProID>
	        <menuItem_file_exit>退出</menuItem_file_exit>

            <menuItem_setup>设置</menuItem_setup>
	        <menuItem_setup_selectFont>选择字体</menuItem_setup_selectFont>
	        <menuItem_setup_selectLanguage>选择语言</menuItem_setup_selectLanguage>
            */

            p_lans["menuItem_file"] = getText(parser, root["menuItem_file"],
                sb, def_menuItem_file);
            p_lans["menuItem_file_openSetup"] = getText(parser, root["menuItem_file_openSetup"],
                sb, def_menuItem_file_openSetup);
            p_lans["menuItem_file_openProcess"] = getText(parser, root["menuItem_file_openProcess"],
                sb, def_menuItem_file_openProcess);

            p_lans["menuItem_file_openPro_formTitle"] = getText(parser,
                root["menuItem_file_openPro_formTitle"],
                sb, def_menuItem_file_openPro_formTitle);

            p_lans["menuItem_file_inputProID"] = getText(parser,
                root["menuItem_file_inputProID"],
                sb, def_menuItem_file_inputProID);

            p_lans["menuItem_file_exit"] = getText(parser, root["menuItem_file_exit"],
                sb, def_menuItem_file_exit);

            p_lans["menuItem_setup"] = getText(parser, root["menuItem_setup"], 
                sb, def_menuItem_setup);
            p_lans["menuItem_setup_selectFont"] = getText(parser, root["menuItem_setup_selectFont"],
                sb, def_menuItem_setup_selectFont);
            p_lans["menuItem_setup_selectLanguage"] = getText(parser, root["menuItem_setup_selectLanguage"],
                sb, def_menuItem_setup_selectLanguage);

            #endregion

            #region 值类型参数

            /*
            <column_moftype_byte>字节值</column_moftype_byte>
	        <column_moftype_int16>16位整数</column_moftype_int16>
	        <column_moftype_uint16>16位正整数</column_moftype_uint16>
	        <column_moftype_int32>32位整数</column_moftype_int32>
	        <column_moftype_uint32>32位正整数</column_moftype_uint32>
	        <column_moftype_int64>64位整数</column_moftype_int64>
	        <column_moftype_float>单浮点</column_moftype_float>
	        <column_moftype_double>双浮点</column_moftype_double>
            */

            p_lans["column_moftype_byte"] = getText(parser, root["column_moftype_byte"],
              sb, def_column_moftype_byte);

            p_lans["column_moftype_int16"] = getText(parser, root["column_moftype_int16"],
              sb, def_column_moftype_int16);

            p_lans["column_moftype_uint16"] = getText(parser, root["column_moftype_uint16"],
              sb, def_column_moftype_uint16);

            p_lans["column_moftype_int32"] = getText(parser, root["column_moftype_int32"],
              sb, def_column_moftype_int32);

            p_lans["column_moftype_uint32"] = getText(parser, root["column_moftype_uint32"],
              sb, def_column_moftype_uint32);

            p_lans["column_moftype_int64"] = getText(parser, root["column_moftype_int64"],
              sb, def_column_moftype_int64);

            p_lans["column_moftype_float"] = getText(parser, root["column_moftype_float"],
              sb, def_column_moftype_float);

            p_lans["column_moftype_double"] = getText(parser, root["column_moftype_double"],
              sb, def_column_moftype_double);

            #endregion

            #region 对话框
            /*
            <OpenFileDialpgFilter_modconfig>修改器配置文件</OpenFileDialpgFilter_modconfig>
            <OpenFileDialpgFilter_AllFile>所有文件</OpenFileDialpgFilter_AllFile>
            <OpenFileDialpg_title>选择一个修改配置</OpenFileDialpg_title>
            */

            p_lans["OpenFileDialpgFilter_modconfig"] = getText(parser, root["OpenFileDialpgFilter_modconfig"],
              sb, def_OpenFileDialpgFilter_modconfig);

            p_lans["OpenFileDialpgFilter_AllFile"] = getText(parser, root["OpenFileDialpgFilter_AllFile"],
              sb, def_OpenFileDialpgFilter_AllFile);

            p_lans["OpenFileDialpg_title"] = getText(parser, root["OpenFileDialpg_title"],
              sb, def_OpenFileDialpg_title);

            //p_lans["OpenFontDialpg_title"] = getText(parser, root["OpenFontDialpg_title"],
              //sb, def_OpenFontDialpg_title);
            #endregion

            #region 按钮

            p_lans["button_ok"] = getText(parser, root["button_ok"],
              sb, def_button_ok);

            p_lans["button_cancel"] = getText(parser, root["button_cancel"],
              sb, def_button_cancel);

            #endregion

        }

        /// <summary>
        /// 从指定xml文档文件初始化语言包，如果出错则采用默认初始化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="parser">xml文本解析提取器</param>
        /// <param name="exception">引发的异常对象</param>
        /// <returns>true表示可以初始化，false表示引发了异常后使用默认初始化</returns>
        public bool InitByXmlFileOrDef(string filePath, XmlStandardItemText parser, out Exception exception)
        {
            try
            {
                XmlDocument xml;

                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.UTF8, false, 1024 * 2, true))
                    {
                        xml = new XmlDocument();
                        xml.Load(sr);
                    }
                }

                InitByXml(xml, parser);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                InitDefault();
                return false;
            }
        }

        #endregion

    }


}
