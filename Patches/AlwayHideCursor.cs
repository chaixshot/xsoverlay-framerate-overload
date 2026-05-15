using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace xsoverlay_tweak.Patches
{
    internal class AlwayHideCursor
    {
        // Static list to track all active window managers
        private static readonly List<WindowComponentManager> instanceRefs = [];

        // Cache the private field once for high-speed access
        private static readonly FieldInfo WindowCursorField = AccessTools.Field(typeof(WindowComponentManager), "WindowCanShowDesktopCursor");

        [HarmonyPatch(typeof(WindowComponentManager), "Start")]
        [HarmonyPostfix]
        public static void Start(WindowComponentManager __instance)
        {
            if (!instanceRefs.Contains(__instance))
                instanceRefs.Add(__instance);
        }

        [HarmonyPatch(typeof(WindowComponentManager), "OnSwitchHoveringOverlay")]
        [HarmonyPostfix]
        public static void OnSwitchHoveringOverlay()
        {
            // Loop backwards through all managers
            for (int i = instanceRefs.Count - 1; i >= 0; i--)
            {
                WindowComponentManager manager = instanceRefs[i];

                // If window was destroyed, remove from list and skip
                if (manager == null)
                {
                    instanceRefs.RemoveAt(i);
                    continue;
                }

                // Set the private boolean to false for EVERY manager
                WindowCursorField.SetValue(manager, false);
            }
        }

        // Clean up list when windows are destroyed to save memory
        [HarmonyPatch(typeof(WindowComponentManager), "OnDestroy")]
        [HarmonyPostfix]
        public static void OnDestroy(WindowComponentManager __instance)
        {
            instanceRefs.Remove(__instance);
        }
    }
}