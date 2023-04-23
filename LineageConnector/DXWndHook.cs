using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LineageConnector
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CWPSTRUCT
    {
        public IntPtr lParam;
        public IntPtr wParam;
        public int message;
        public IntPtr hwnd;
        public string lpszClass;
        public int dwExtraInfo;
    }

    internal class DXWndHook
    {
        private const int WH_CALLWNDPROC = 4;
        private const int WH_CALLWNDPROCRET = 12;

        private readonly IntPtr dxWndHandle;
        private IntPtr callWndProcHookHandle;
        private IntPtr callWndRetProcHookHandle;

        private const int HC_ACTION = 0;
        private const int WM_CREATE = 0x0001;
        private const int GWL_STYLE = -16;
        private const int WS_POPUP = unchecked((int)0x80000000);
        private const int WS_OVERLAPPEDWINDOW = unchecked((int)0x00CF0000);
        private const int SW_NORMAL = 1;
        private const int SW_SHOW = 5;

        public DXWndHook(IntPtr dxWndHandle)
        {
            this.dxWndHandle = dxWndHandle;
        }

        public void Hook()
        {
            SetWindowLong(this.dxWndHandle, GWL_STYLE, GetWindowLong(this.dxWndHandle, WS_POPUP));
            //SetWindowLong(this.dxWndHandle, GWL_STYLE, GetWindowLong(this.dxWndHandle, GWL_STYLE) & ~(WS_POPUP));
            // SetWindowLong(this.dxWndHandle, GWL_STYLE, GetWindowLong(this.dxWndHandle, GWL_STYLE) | WS_OVERLAPPEDWINDOW);
            ShowWindow(this.dxWndHandle, SW_SHOW);

            // WH_CALLWNDPROC 후크를 설치합니다.
            callWndProcHookHandle = SetWindowsHookEx(WH_CALLWNDPROC, CallWndProcHook, IntPtr.Zero, GetWindowThreadProcessId(dxWndHandle, IntPtr.Zero));

            // WH_CALLWNDPROCRET 후크를 설치합니다.
            callWndRetProcHookHandle = SetWindowsHookEx(WH_CALLWNDPROCRET, CallWndRetProcHook, IntPtr.Zero, GetWindowThreadProcessId(dxWndHandle, IntPtr.Zero));
        }

        public void Unhook()
        {
            // WH_CALLWNDPROC 후크를 해제합니다.
            if (callWndProcHookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(callWndProcHookHandle);
                callWndProcHookHandle = IntPtr.Zero;
            }

            // WH_CALLWNDPROCRET 후크를 해제합니다.
            if (callWndRetProcHookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(callWndRetProcHookHandle);
                callWndRetProcHookHandle = IntPtr.Zero;
            }
        }

        private static IntPtr CallWndProcHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // connector.exe가 실행되면 윈도우 모드로 변경합니다.
            if (nCode == HC_ACTION && wParam.ToInt32() == WM_CREATE)
            {
                CWPSTRUCT cwp = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
                if (cwp.lpszClass == "lin")
                {
                    IntPtr hwnd = cwp.hwnd;
                    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~(WS_POPUP));
                    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) | WS_OVERLAPPEDWINDOW);
                    ShowWindow(hwnd, SW_NORMAL);
                }
            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr CallWndRetProcHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

    }
}
