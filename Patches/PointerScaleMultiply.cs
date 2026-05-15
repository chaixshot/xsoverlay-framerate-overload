using HarmonyLib;
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
                XSettingsManager.Instance.Settings.PointerScale *= XConfig.PointerScaleMultiply.Value;
            };
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start()
        {
            XSettingsManager.Instance.Settings.PointerScale *= XConfig.PointerScaleMultiply.Value;
        }
    }
}
