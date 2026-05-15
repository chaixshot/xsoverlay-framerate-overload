using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace xsoverlay_tweak.Utils
{
    public static class MouseInputDetector
    {
        private static long _lastPhysicalMoveTime;
        private const long StopThresholdMs = 100; // Time in ms to consider the mouse "stopped"

        // Public property: True if physical movement happened within the last 100ms
        public static bool IsPhysicalMovement
        {
            get
            {
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                return (currentTime - _lastPhysicalMoveTime) < StopThresholdMs;
            }
        }

        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int LLMHF_INJECTED = 0x01;

        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static void Start()
        {
            if (_hookID == IntPtr.Zero)
                _hookID = SetHook(_proc);
        }

        public static void Stop()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEMOVE)
            {
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                // Check if the movement is from hardware (not injected)
                bool isHardware = (hookStruct.flags & LLMHF_INJECTED) == 0;

                if (isHardware)
                {
                    // Update the timestamp to 'now'
                    _lastPhysicalMoveTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int x; public int y; }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}