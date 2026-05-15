using HarmonyLib;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(UI_RelativeTransformManipulator))]
    internal class PointerScale
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            if (XConfig.PointerScale.Value > 100)
                XSettingsManager.Instance.Settings.PointerScale = XConfig.PointerScale.Value / 100;
        }
    }
}
