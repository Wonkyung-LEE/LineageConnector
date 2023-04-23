using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Resources;
using System.Runtime.InteropServices;
using System.Net;
using LineageConnector.WindowsAPI;
using static LineageConnector.WindowsAPI.Kernel32;

namespace LineageConnector
{
    public class DXWND
    {
        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const Int32 PROCESS_TERMINATE = 0x1;

        private string DXWND_PATH = ""; //exe 파일의 경로는 포함하지 않음
        private string DXWND_NAME = "dxwnd.exe"; //기본값
        private string DXWND_CONFIG = "dxwnd.ini";

        //private string START_FILE_NAME = Path.GetFullPath("D:\\GAMES\\Lineage_270\\") + "lin.exe";
        // private string START_FILE_NAME = Path.GetFullPath("D:\\lineage_kr_client_270\\") + "lin.exe";
        private string START_FILE_NAME = Path.GetFullPath("C:\\Windows\\System32\\") + "notepad.exe";
        private string SERVER_ADDRESS = "122.222.80.14";
        private int SERVER_PORT = 2000;

        private Process DXWND_PROCESS = null;

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32")]
        public static extern IntPtr OpenProcess(Int32 Access, Boolean InheritHandle, Int32 ProcessId);
        [DllImport("kernel32")]
        public static extern Int32 TerminateProcess(IntPtr hProcess, Int32 ExitCode);

        [DllImport("kernel32")]
        public static extern void CloseHandle(IntPtr hProcess);


        public DXWND(string Path, string FileName = "dxwnd.exe", string ConfigName = "dxwnd.ini")
        {
            this.DXWND_NAME = FileName;
            this.DXWND_CONFIG = ConfigName;
            this.DXWND_PATH = Path;
        }


        public bool StartDXWND()
        {
            Process[] ExistProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(DXWND_NAME));
            if (ExistProcess == null || ExistProcess.Length == 0) //꺼져있으면 실행
            {
                if (!File.Exists(Path.Combine(DXWND_PATH, DXWND_NAME))) return false;
                // DXWND 실행
                ProcessStartInfo info = new ProcessStartInfo(Path.Combine(DXWND_PATH, DXWND_NAME));
                info.CreateNoWindow = true;
                info.WorkingDirectory = DXWND_PATH;
                info.UseShellExecute = false;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                DXWND_PROCESS = Process.Start(info);
                HideDXWND();
                // DXWND 핸들러
                IntPtr dxWndHandle = DXWND_PROCESS.MainWindowHandle;


                // 리니지 실행정보
                System.Threading.Thread.Sleep(800);
                ProcessStartInfo connectorStartInfo = new ProcessStartInfo(START_FILE_NAME);
                connectorStartInfo.UseShellExecute = false;
                connectorStartInfo.RedirectStandardOutput = true;
                connectorStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                DXWND_PROCESS.WaitForInputIdle();

                // 인젝션 컨트롤러
                DLLInjectionHelper dll_injector = new DLLInjectionHelper();

                // 리니지 실행
                Process connectorProcess = Process.Start(connectorStartInfo);
                connectorProcess.WaitForInputIdle();
                int linProcessId = connectorProcess.Id;

                // 리니지 핸들러
                IntPtr linProcessHandle = OpenProcess(ProcessAccessFlagsInt.All, false, linProcessId);
                // DXWND 인젝션
                dll_injector.Inject(linProcessId, Path.Combine(DXWND_PATH, "\\dxwnd.dll"));

                // 후킹
                /*
                DXWndHook hook = new DXWndHook(linProcessHandle);
                hook.Hook();
                // connector.exe가 종료될 때까지 기다립니다.
                connectorProcess.WaitForExit();
                // 후킹을 해제합니다.
                hook.Unhook();
                */
                // RunProcessFromDXWND(processId);
            }
            else DXWND_PROCESS = ExistProcess[0];
            return true;
        }

        public void HideDXWND()
        {
            if (DXWND_PROCESS != null)
                ShowWindow(DXWND_PROCESS.MainWindowHandle, SW_HIDE);
        }

        public bool ExitDXWND()
        {
            bool result = false;
            Process[] ExistProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(DXWND_NAME));
            if (ExistProcess != null && ExistProcess.Length > 0 && DXWND_PROCESS != null)
            {
                IntPtr hProcess = OpenProcess(PROCESS_TERMINATE, false, DXWND_PROCESS.Id);
                if (hProcess != IntPtr.Zero)
                {
                    if (TerminateProcess(hProcess, 0) != 0)
                        result = true;
                    CloseHandle(hProcess);
                }
            }

            return result;
        }

        private string ReadINI()
        {
            string inipath = Path.Combine(DXWND_PATH, DXWND_CONFIG);
            if (!File.Exists(inipath)) return "";
            string s = File.ReadAllText(Path.Combine(DXWND_PATH, DXWND_CONFIG));
            return s;
        }

        public bool EditINIStringForLineage(string IP, int PORT, string LineageFullPath, string SizeX, string SizeY, string TitleName = "lineage")
        {
            string INIString = ReadINI();
            if (INIString == null || INIString == "") return false;

            const string exepath_key = "exepath";
            const string path0_key = "path0";
            string inipath = Path.Combine(DXWND_PATH, DXWND_CONFIG);
            string[] sss = INIString.Split('\n');
            StringBuilder sb = new StringBuilder();
            if (sss != null && sss.Length > 0)
            {
                for (int i = 0; i < sss.Length; i++)
                {
                    string s = sss[i];
                    if (s == null || s.Length == 0) continue;
                    s = s.Replace("\r", "");
                    string[] attribute = s.Split('=');
                    if (attribute == null) continue;
                    if (attribute.Length > 1)
                    {
                        string Key = attribute[0];
                        string Val = attribute[1];
                        if (Key == exepath_key)
                            Val = Path.GetDirectoryName(LineageFullPath);
                        else if (Key == path0_key)
                            Val = LineageFullPath; //+ " " + IP + " " + PORT;
                        else if (Key == "sizx0" || Key == "initresw0")
                            Val = SizeX;
                        else if (Key == "sizy0" || Key == "initresh0")
                            Val = SizeY;
                        else if (Key == "title0")
                            Val = TitleName;
                        else if (Key == "cmdline0")
                            Val = Path.GetFileName(LineageFullPath) + " " + IP + " " + PORT;
                        sss[i] = Key + "=" + Val;
                    }
                    sb.Append(sss[i] + "\n");
                }
            }

            try
            {
                File.WriteAllText(inipath, sb.ToString());
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void RunProcessFromDXWND(uint processId, int StartDelay = 550)
        {
            // DXWnd 프로세스에서 외부 프로그램 실행
            /*
            Process externalProcess = new Process();
            externalProcess.StartInfo.FileName = START_FILE_NAME;
            externalProcess.StartInfo.UseShellExecute = false;
            externalProcess.StartInfo.RedirectStandardOutput = true;
            externalProcess.StartInfo.Arguments = SERVER_ADDRESS;
            externalProcess.Start();
            */

            // DXWND 핸들러
            /*
            IntPtr dxWndHandle = DXWND_PROCESS.MainWindowHandle;
            ProcessModule curModule = connectorProcess.MainModule;

            // 후킹
            DXWndHook hook = new DXWndHook(dxWndHandle);
            hook.Hook();
            // connector.exe가 종료될 때까지 기다립니다.
            connectorProcess.WaitForExit();
            // 후킹을 해제합니다.
            hook.Unhook();
            */
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            Terminate = 0x0001,
            CreateThread = 0x0002,
            VirtualMemoryOperation = 0x0008,
            VirtualMemoryRead = 0x0010,
            VirtualMemoryWrite = 0x0020,
            DuplicateHandle = 0x0040,
            CreateProcess = 0x0080,
            SetQuota = 0x0100,
            SetInformation = 0x0200,
            QueryInformation = 0x0400,
            SuspendResume = 0x0800,
            All = 0x001F0FFF
        }

        public static class ProcessAccessFlagsInt
        {
            public const int Terminate = 0x0001;
            public const int CreateThread = 0x0002;
            public const int VirtualMemoryOperation = 0x0008;
            public const int VirtualMemoryRead = 0x0010;
            public const int VirtualMemoryWrite = 0x0020;
            public const int DuplicateHandle = 0x0040;
            public const int CreateProcess = 0x0080;
            public const int SetQuota = 0x0100;
            public const int SetInformation = 0x0200;
            public const int QueryInformation = 0x0400;
            public const int SuspendResume = 0x0800;
            public const int All = 0x001F0FFF;
        }

        public const uint MEM_COMMIT = 0x00001000;
        public const uint PAGE_READWRITE = 0x04;


    }
}
