using HarmonyLib;
using System;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(UpdateDateTime))]
    internal class PointerScaleMultiply
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            XSOEventSystem.OnSetPointerScale += scale =>
            {
                XSettingsManager.Instance.Settings.PointerScale = scale * XConfig.PointerScaleMultiply.Value;
            };
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            XSSettings settings = XSettingsManager.Instance.Settings;
            settings.PointerScale = Math.Min(settings.PointerScale, 1f) * XConfig.PointerScaleMultiply.Value;
        }
    }
}
