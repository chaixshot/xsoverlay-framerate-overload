using HarmonyLib;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_framerate_overload.Patches
{
    internal class TargetFPS
    {
        /*[HarmonyPatch(typeof(Unity_Overlay), "DetermineOverlayFramerate")]
        [HarmonyPostfix]
        public static void Update(Unity_Overlay __instance)
        {
            if (!__instance.IsDesktopOrWindowCapture) return;

            __instance.ActiveUpdateRateFPS = XConfig.RefreshRate.Value;
        }*/

        [HarmonyPatch(typeof(Unity_Overlay), "UpdateTexture")]
        [HarmonyPrefix]
        public static bool UpdateTexture(Unity_Overlay __instance, ref int ___UpdateRateFPS)
        {
            if (!__instance.IsDesktopOrWindowCapture) return true;

            ___UpdateRateFPS = -1;

            return true;
        }

        [HarmonyPatch(typeof(DeviceManager), "GetHMDRefreshRate")]
        [HarmonyPrefix]
        public static bool GetHMDRefreshRate(DeviceManager __instance)
        {
            if (XConfig.RefreshRate.Value > __instance.HMDRefreshRate || XConfig.RefreshRate.Value.Equals(-1))
            {
                XSTools.ExecuteOnMainThread(delegate
                {
                    Application.targetFrameRate = XConfig.RefreshRate.Value;
                    Time.fixedDeltaTime = 1f / (float)XConfig.RefreshRate.Value;
                });

                return false;
            }

            return true;
        }

        //!! Test game fps
        /*private static float _lastFrameTime;
        public static float CurrentFPS;
        [HarmonyPatch(typeof(Raycaster), "Update")]
        [HarmonyPostfix]
        public static void GetFPS(Raycaster __instance)
        {
            //Application.targetFrameRate = -1;
            //QualitySettings.vSyncCount = 0;
            //Time.fixedDeltaTime = 1f / 240f;

            if (!(__instance.DeviceIdx is Raycaster.DeviceIdxFetch.RightHand)) return;

            // Time.unscaledTime is best to avoid issues if the game is slowed down
            float currentTime = UnityEngine.Time.unscaledTime;
            float deltaTime = currentTime - _lastFrameTime;

            if (deltaTime > 0)
            {
                CurrentFPS = 1.0f / deltaTime;
            }

            _lastFrameTime = currentTime;

            Plugin.Logger.LogError($"Current RefreshRate: {CurrentFPS}");
        }*/
    }
}
