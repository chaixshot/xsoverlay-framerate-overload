using HarmonyLib;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(UI_RelativeTransformManipulator))]
    internal class PointerScale
    {
        [HarmonyPatch("OnSetPointerScale")]
        [HarmonyPostfix]
        public static void UpdateHoveringOverlay(UI_RelativeTransformManipulator __instance, ref float ___scaleMultiplier)
        {
            ___scaleMultiplier = 2f;
        }
    }
}
