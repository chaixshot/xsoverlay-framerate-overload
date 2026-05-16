using HarmonyLib;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(UI_RelativeTransformManipulator))]
    internal class PointerScaleMultiply
    {
        [HarmonyPatch("OnSetPointerScale")]
        [HarmonyPostfix]
        public static void OnSetPointerScale(ref float ___scaleMultiplier)
        {
            ___scaleMultiplier *= XConfig.PointerScaleMultiply.Value;
        }
    }
}
