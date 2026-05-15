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
            if (__instance.HapticDeviceName == Raycaster.HapticDevice.None) return;

            if (DesktopCursorManager.Instance.GetCurrentInputDevice() != __instance && __instance.HoveringOverlay.IsDesktopOrWindowCapture)
                ___VisualCursorElementOverlay.colorTint = Color.red;
            else if (!__instance.HoveringOverlay.IsLocked)
                ___VisualCursorElementOverlay.colorTint = XSettingsManager.Instance.Settings.AccentColor;
        }
        }
    }
}
