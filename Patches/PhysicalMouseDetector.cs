using HarmonyLib;
using xsoverlay_tweak.Utils;

namespace xsoverlay_tweak.Patches
{
    internal class PhysicalMouseDetector
    {
        private static bool IsPhysicalMovement = false;

        [HarmonyPatch(typeof(UpdateDateTime), "Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            MouseInputDetector.Start();
        }

        [HarmonyPatch(typeof(UpdateDateTime), "Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            if (MouseInputDetector.IsPhysicalMovement)
                IsPhysicalMovement = true;
        }

        [HarmonyPatch(typeof(Raycaster)), HarmonyPatch("HandleClicksForDesktopWindows")]
        [HarmonyPostfix]
        public static void HandleClicksForDesktopWindows()
        {
            IsPhysicalMovement = false;
        }

        [HarmonyPatch(typeof(Raycaster)), HarmonyPatch("HandleTouchInputForDesktopWindows")]
        [HarmonyPostfix]
        public static void HandleTouchInputForDesktopWindows()
        {
            IsPhysicalMovement = false;
        }

        [HarmonyPatch(typeof(Raycaster), "SyncedOverlayUpdate")]
        [HarmonyPrefix]
        public static bool SyncedOverlayUpdate()
        {
            return !IsPhysicalMovement;
        }
    }
}
