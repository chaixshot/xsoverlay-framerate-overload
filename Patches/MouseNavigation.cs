using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;
using Valve.VR;
using WindowsInput;
using WindowsInput.Native;
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
        [HarmonyPatch(typeof(UpdateDateTime), "Awake")]
        [HarmonyPostfix]
        public static void InitializeEvents()
        {
            ApplySteamVRActionBinding();

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

        private static void SimulateBackNavigation(InputSimulator sim)
        {
            if (XConfig.MouseNavigationUseModifiedKey.Value)
                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.LEFT);
            else
                sim.Mouse.XButtonClick(1);
        }

        private static void SimulateForwardNavigation(InputSimulator sim)
        {
            if (XConfig.MouseNavigationUseModifiedKey.Value)
                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.RIGHT);
            else
                sim.Mouse.XButtonClick(2);
        }

        private static void ApplySteamVRActionBinding()
        {
            string filePath = @".\XSOverlay_Data\StreamingAssets\SteamVR\actions.json";

            string json = File.ReadAllText(filePath);
            JObject root = JObject.Parse(json);
            bool modified = false;

            // Update "actions" array
            JArray actions = (JArray)root["actions"];
            string[] actionNames = ["/actions/xsoverlay/in/MouseBack", "/actions/xsoverlay/in/MouseForward"];

            foreach (string name in actionNames)
            {
                if (!actions.Any(a => a["name"]?.ToString() == name))
                {
                    actions.Add(new JObject
                    {
                        ["name"] = name,
                        ["type"] = "boolean",
                        ["requirement"] = "optional"
                    });
                    modified = true;
                }
            }

            // Update "localization" object
            // Localization is an array of objects; we want the first one (usually en_US)
            JArray localization = (JArray)root["localization"];
            if (localization != null && localization.HasValues)
            {
                JObject langObject = (JObject)localization[0];

                if (langObject["/actions/xsoverlay/in/MouseBack"] == null)
                {
                    langObject["/actions/xsoverlay/in/MouseBack"] = "Mouse Back";
                    modified = true;
                }

                if (langObject["/actions/xsoverlay/in/MouseForward"] == null)
                {
                    langObject["/actions/xsoverlay/in/MouseForward"] = "Mouse Forward";
                    modified = true;
                }
            }

            // Save if changes were made
            if (modified)
            {
                File.WriteAllText(filePath, root.ToString(Formatting.Indented));
                Console.WriteLine("Manifest updated with actions and localization.");
            }
        }
    }
}