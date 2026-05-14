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
            if (DesktopCursorManager.Instance.GetCurrentInputDevice() == __instance)
                ___VisualCursorElementOverlay.colorTint = XSettingsManager.Instance.Settings.AccentColor;
            else
                ___VisualCursorElementOverlay.colorTint = Color.red;
        }
    }
}
