using Cheng.Json;
using Cheng.LoopThreads;
using Cheng.Memorys;
using Cheng.Windows.Processes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using Cheng.Json.GeneratorNumbers;
using Cheng.Algorithm.Collections;
using System.Media;
using Cheng.Streams;

namespace Cheng.ModifyMaster
{

    public class AudioEffects : SafreleaseUnmanagedResources
    {

        #region

        public AudioEffects()
        {
            p_openToggle = null;
            p_closeToggle = null;
        }

        #endregion

        #region

        private SoundPlayer p_openToggle;

        private SoundPlayer p_closeToggle;

        #endregion

        #region 功能

        public void LoadAudioByDefPath()
        {
            ThrowObjectDisposeException();
            var args = InitArgs.Args;
            var filePath = Path.Combine(args.rootDirectory, "audio");
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (!Cheng.IO.IOoperations.File.CheckZIP(file))
                    {
                        args.DebugPrintLine("初始化音频时没有找到audio音效包");
                        return;
                    }

                    file.Seek(0, SeekOrigin.Begin);

                    MemoryStream openBuf, closeBuf;
                    openBuf = null;
                    closeBuf = null;

                    using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read, true))
                    {
                        var openEnt = zip.GetEntry("open.wav");
                        var closeEnt = zip.GetEntry("close.wav");
                       
                        long len;

                        if (openEnt != null)
                        {
                            using (var open = openEnt.Open())
                            {
                                len = openEnt.Length;
                                if (len < (1024 * 1024 * 32))
                                {
                                    byte[] bus = new byte[len];
                                    open.ReadBlock(bus, 0, bus.Length);
                                    openBuf = new MemoryStream(bus);
                                }
                                else
                                {
                                    args.DebugPrintLine("open音效文件过大");
                                }
                            }
                        }
                        else
                        {
                            args.DebugPrintLine("未找到open音效");
                        }

                        if (closeEnt != null)
                        {
                            using (var open = closeEnt.Open())
                            {
                                len = closeEnt.Length;
                                if (len < (1024 * 1024 * 32))
                                {
                                    byte[] bus = new byte[len];
                                    open.ReadBlock(bus, 0, bus.Length);
                                    closeBuf = new MemoryStream(bus);
                                }
                                else
                                {
                                    args.DebugPrintLine("close音效文件过大");
                                }
                            }
                        }
                        else
                        {
                            args.DebugPrintLine("未找到close音效");
                        }

                    }

                    LoadAudio(openBuf, closeBuf);
                }
            }
            catch (Exception ex)
            {
                args.DebugPrintLine("音效加载失败");
                args.DebugPrintLine(InitArgs.ExceptionText(ex));
            }

            args.DebugFlush();
        }

        /// <summary>
        /// 加载音效
        /// </summary>
        /// <param name="openToggleAudio">打开开关音效所在的wav数据流</param>
        /// <param name="closeToggleAudio">关闭开关音效所在的wav数据流</param>
        public void LoadAudio(Stream openToggleAudio, Stream closeToggleAudio)
        {
            ThrowObjectDisposeException();
            p_openToggle?.Dispose();
            p_closeToggle?.Dispose();
            if(openToggleAudio != null) p_openToggle = new SoundPlayer(openToggleAudio);
            if(closeToggleAudio != null) p_closeToggle = new SoundPlayer(closeToggleAudio);
        }

        public void PlayOnToggle(bool active)
        {
            if (active)
            {
                p_openToggle?.Play();
            }
            else
            {
                p_closeToggle?.Play();
            }
        }

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_openToggle?.Dispose();
                p_closeToggle?.Dispose();
            }
            p_openToggle = null;
            p_closeToggle = null;
            return true;
        }

        #endregion

    }

}
