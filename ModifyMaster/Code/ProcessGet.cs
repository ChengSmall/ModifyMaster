using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Linq;
using System.Threading.Tasks;

namespace Cheng
{

    public static unsafe class ProcessCheckGets
    {
        // WinAPI声明
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr TokenHandle, uint TokenInformationClass,
            IntPtr TokenInformation, int TokenInformationLength,
            out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        // 常量定义
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TokenUser = 1;
        private const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        /// <summary>
        /// 获取所有用户进程
        /// </summary>
        /// <param name="checkMainWindows">true需要额外判断进程拥有主窗体才会返回true；false忽略进程是否有窗体</param>
        /// <returns></returns>
        public static IEnumerable<Process> GetUserProcesses(bool checkMainWindows)
        {
            return Process.GetProcesses()
                //.AsParallel()
                .Where(func);

            bool func(Process p)
            {
                try
                {
                    //过滤
                    bool bb;

                    bb = p.SessionId != 0 && IsUserProcess(p);

                    if (!bb)
                    {
                        p.Close();
                        return false;
                    }

                    if (checkMainWindows)
                    {
                        bb = p.MainWindowHandle != IntPtr.Zero;
                    }

                    if (!bb) p.Close();
                    return bb;
                }
                catch
                {
                    p.Close();
                    return false; // 跳过无权限进程
                }
            }
        }

        private static bool IsUserProcess(Process process)
        {
            var proID = process.Id;

            IntPtr hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, proID);
            if (hProcess == IntPtr.Zero) return false;

            try
            {
                if (!OpenProcessToken(hProcess, TOKEN_QUERY, out IntPtr tokenHandle))
                    return false;

                try
                {
                    // 获取令牌信息长度
                    int tokenInfoLength;
                    GetTokenInformation(tokenHandle, TokenUser, IntPtr.Zero, 0, out tokenInfoLength);
                    if (tokenInfoLength == 0) return false;

                    // 分配内存
                    IntPtr tokenInfo;
                    byte* tokenMemory = stackalloc byte[tokenInfoLength];
                    //Console.WriteLine(tokenInfoLength);
                    //IntPtr tokenInfo = Marshal.AllocHGlobal(tokenInfoLength);
                    tokenInfo = new IntPtr(tokenMemory);

                    if (!GetTokenInformation(tokenHandle, TokenUser, tokenInfo, tokenInfoLength, out _))
                        return false;

                    //转化结构
                    var tokenUser = (TOKEN_USER)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_USER));
                    var sid = new SecurityIdentifier(tokenUser.User.Sid);

                    return !IsSystemAccount(sid);
                }
                finally
                {
                    if(tokenHandle != IntPtr.Zero) CloseHandle(tokenHandle);
                }
            }
            catch(Exception)
            {
                return false;
            }
            finally
            {
                if (hProcess != IntPtr.Zero) CloseHandle(hProcess);
            }
        }

        private static bool IsSystemAccount(SecurityIdentifier sid)
        {
            try
            {
                return sid.IsWellKnown(WellKnownSidType.LocalSystemSid) ||
                       sid.IsWellKnown(WellKnownSidType.LocalServiceSid) ||
                       sid.IsWellKnown(WellKnownSidType.NetworkServiceSid);
            }
            catch
            {
                return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_USER
        {
            public SID_AND_ATTRIBUTES User;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }

    }

}
