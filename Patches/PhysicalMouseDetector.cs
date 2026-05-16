using HarmonyLib;
using xsoverlay_tweak.Utils;

namespace xsoverlay_tweak.Patches
{
    internal class PhysicalMouseDetector
    {
        private static bool IsPhysicalMovement = false;
        private static MouseInputDetector mouseDetector;

        [HarmonyPatch(typeof(UpdateDateTime), "Awake")]
        [HarmonyPostfix]
        public static void InitializeEvents()
        {
            mouseDetector = new MouseInputDetector();
            mouseDetector.PhysicalMouseMoved += (x, y) =>
            {
                if (IsEnable())
                    IsPhysicalMovement = true;
            };
        }

        [HarmonyPatch(typeof(Raycaster)), HarmonyPatch("HandleClicksForDesktopWindows")]
        [HarmonyPrefix]
        public static bool HandleClicksForDesktopWindows()
        {
            if (IsPhysicalMovement)
            {
                IsPhysicalMovement = false;
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Raycaster)), HarmonyPatch("HandleTouchInputForDesktopWindows")]
        [HarmonyPrefix]
        public static bool HandleTouchInputForDesktopWindows()
        {
            if (IsPhysicalMovement)
            {
                IsPhysicalMovement = false;
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Raycaster), "SyncedOverlayUpdate")]
        [HarmonyPrefix]
        public static bool SyncedOverlayUpdate()
        {
            return !IsPhysicalMovement;
        }

        private static bool IsEnable()
        {
            return XConfig.PhysicalMouseDetector.Value;
        }
    }
}
