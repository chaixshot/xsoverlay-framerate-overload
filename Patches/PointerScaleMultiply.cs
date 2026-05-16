using HarmonyLib;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(UI_RelativeTransformManipulator))]
    internal class PointerScaleMultiply
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(UI_RelativeTransformManipulator __instance)
        {
            XConfig.PointerScaleMultiply.SettingChanged += (sender, args) =>
            {
                AccessTools.Field(typeof(UI_RelativeTransformManipulator), "scaleMultiplier").SetValue(__instance, GetScale());
            };
        }

        [HarmonyPatch("OnSetPointerScale")]
        [HarmonyPostfix]
        public static void OnSetPointerScale(ref float ___scaleMultiplier)
        {
            ___scaleMultiplier = GetScale();
        }

        private static float GetScale()
        {
            return XSettingsManager.Instance.Settings.PointerScale * XConfig.PointerScaleMultiply.Value;
        }
    }
}
