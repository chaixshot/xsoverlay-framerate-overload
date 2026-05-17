using HarmonyLib;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(Raycaster))]
    internal class ActivePointerColor
    {
        [HarmonyPatch("UpdateHoveringOverlay")]
        [HarmonyPostfix]
        public static void UpdateHoveringOverlay(Raycaster __instance, ref Unity_Overlay ___VisualCursorElementOverlay)
        {
            if (!IsEnable()) return;
            if (__instance.HapticDeviceName == Raycaster.HapticDevice.None) return;

            if (DesktopCursorManager.Instance.GetCurrentInputDevice() != __instance && __instance.HoveringOverlay.IsDesktopOrWindowCapture)
                ___VisualCursorElementOverlay.colorTint = Color.red;
            else if (!__instance.HoveringOverlay.IsLocked)
                ___VisualCursorElementOverlay.colorTint = XSettingsManager.Instance.Settings.AccentColor;
        }

        [HarmonyPatch("DetermineCursorVisibility")]
        [HarmonyPostfix]
        public static void DetermineCursorVisibility(Raycaster __instance, ref Unity_Overlay ___VisualCursorElementOverlay)
        {
            if (!IsEnable()) return;
            if (__instance.HapticDeviceName == Raycaster.HapticDevice.None) return;

            if (DesktopCursorManager.Instance.GetCurrentInputDevice() != __instance && __instance.HoveringOverlay.IsDesktopOrWindowCapture)
                if (___VisualCursorElementOverlay.opacity.Equals(1))
                    ___VisualCursorElementOverlay.opacity = XConfig.ActivePointerOpacity.Value / 100f;
        }

        private static bool IsEnable()
        {
            return XConfig.ActivePointerColor.Value;
        }
    }
}
