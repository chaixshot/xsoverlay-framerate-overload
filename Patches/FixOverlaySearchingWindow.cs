using HarmonyLib;

namespace xsoverlay_tweak.Patches
{
    internal class FixOverlaySearchingWindow
    {
        [HarmonyPatch(typeof(LayoutHandler)), HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(LayoutHandler __instance)
        {
        }

        [HarmonyPatch(typeof(LayoutHandler), nameof(LayoutHandler.LoadLayout), [])]
        [HarmonyPrefix]
        public static bool LoadLayout(LayoutHandler __instance)
        {

            return true;
        }
    }
}
