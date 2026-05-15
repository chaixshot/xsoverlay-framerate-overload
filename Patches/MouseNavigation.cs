using HarmonyLib;
using System.Runtime.InteropServices;
using Valve.VR;
using WindowsInput;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    internal class MouseNavigation
    {
        private static ulong ActionHandleBack = 0;
        private static ulong ActionHandleForward = 0;

        private static bool BackWasPressedLastFrame = false;
        private static bool ForwardWasPressedLastFrame = false;

        private static bool IsDesktopHover = false;
        private static Raycaster CurrentRaycaster;

        // Get current active hand
        [HarmonyPatch(typeof(DesktopCursorManager), "Awake")]
        [HarmonyPostfix]
        public static void InitializeEvents()
        {
            XSOEventSystem.OnTakeControlOfDesktopCursor += raycaster =>
            {
                CurrentRaycaster = raycaster;
            };
        }

        // Is active hand hover desktop overlay
        [HarmonyPatch(typeof(Raycaster), "Update")]
        [HarmonyPostfix]
        public static void RaycasterUpdate()
        {
            IsDesktopHover = CurrentRaycaster.HoveringOverlay != null && CurrentRaycaster.HoveringOverlay.IsDesktopOrWindowCapture;
        }

        // SteamVR input listen
        [HarmonyPatch(typeof(MouseInputDevice), "Update")]
        [HarmonyPostfix]
        public static void MouseInputDeviceUpdate()
        {
            // Back Navigation
            if (CheckActionTriggered("/actions/xsoverlay/in/MouseBack", ref ActionHandleBack, ref BackWasPressedLastFrame))
                if (IsDesktopHover)
                    SimulateBackNavigation(XInputManager.sim);

            // Forward Navigation
            if (CheckActionTriggered("/actions/xsoverlay/in/MouseForward", ref ActionHandleForward, ref ForwardWasPressedLastFrame))
                if (IsDesktopHover)
                    SimulateForwardNavigation(XInputManager.sim);
        }

        /// <summary>
        /// Generalized trigger check for any boolean SteamVR action
        /// </summary>
        private static bool CheckActionTriggered(string path, ref ulong handle, ref bool lastState)
        {
            if (handle == 0)
                OpenVR.Input.GetActionHandle(path, ref handle);

            InputDigitalActionData_t data = new();
            uint size = (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t));

            var error = OpenVR.Input.GetDigitalActionData(handle, ref data, size, 0);

            if (error != EVRInputError.None || !data.bActive) return false;

            bool isPressedNow = data.bState;
            bool triggered = isPressedNow && !lastState;
            lastState = isPressedNow;

            return triggered;
        }

        public static void SimulateBackNavigation(InputSimulator sim)
        {
            //sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.LEFT);
            sim.Mouse.XButtonClick(1);
        }

        public static void SimulateForwardNavigation(InputSimulator sim)
        {
            //sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.RIGHT);
            sim.Mouse.XButtonClick(2);
        }
    }
}