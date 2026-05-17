using HarmonyLib;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    internal class RefreshRate
    {
        private static float FrameInterval = 1f / 90f;

        private static float LastGrabTime;
        private static float GrabbedDistance = 0f;

        private static float LastScrollingTime;

        [HarmonyPatch(typeof(DeviceManager), "Start")]
        [HarmonyPostfix]
        public static void Start(DeviceManager __instance)
        {
            if (__instance.HMDRefreshRate > 0)
                FrameInterval = 1f / (float)__instance.HMDRefreshRate;
        }

        [HarmonyPatch(typeof(DeviceManager), "GetHMDRefreshRate")]
        [HarmonyPrefix]
        public static bool GetHMDRefreshRate(DeviceManager __instance)
        {
            if (!IsEnable()) return true;

            XSTools.ExecuteOnMainThread(delegate
            {
                Application.targetFrameRate = XConfig.RefreshRate.Value;
                Time.fixedDeltaTime = 1f / (float)XConfig.RefreshRate.Value;
            });

            return false;
        }

        // Fix Push/Pull speed
        [HarmonyPatch(typeof(Raycaster), "Grab")]
        [HarmonyPostfix]
        public static void Grab(ref float ___GrabbedDistance)
        {
            if (!IsEnable()) return;

            float currentTime = Time.unscaledTime;
            if (currentTime - LastGrabTime < FrameInterval)
                ___GrabbedDistance = GrabbedDistance;
            else
            {
                GrabbedDistance = ___GrabbedDistance;
                LastGrabTime = currentTime;
            }
        }

        // Fix Scrolling speed
        [HarmonyPatch(typeof(Raycaster), "HandleScrolling")]
        [HarmonyPrefix]
        public static bool HandleScrolling(Raycaster __instance, ref MouseInputDevice ___InputDevice, ref int ___ScrollClicksPerSecond, ref float ____tickAccumulator, ref Vector2 ___CursorUVNormalized)
        {
            if (!IsEnable()) return true;

            float currentTime = Time.unscaledTime;
            if (currentTime - LastScrollingTime < FrameInterval)
            {
                float num = 0.2f;
                float ScrollAxis = ___InputDevice.NormalizedScrollAxis.y;
                float num2 = Mathf.Abs(ScrollAxis);

                if (num2 <= num || (float)___ScrollClicksPerSecond <= 0f)
                    return false;

                if (__instance.HoveringOverlay.IsDesktopOrWindowCapture)
                {
                    ____tickAccumulator += num2 * (float)___ScrollClicksPerSecond * XSettingsManager.Instance.Settings.ScrollSpeed * Time.deltaTime;

                    int num3 = (int)____tickAccumulator;
                    if (num3 <= 0)
                        return false;

                    ____tickAccumulator -= (float)num3;

                    if (__instance.HoveringOverlay.IsDesktopOrWindowCapture)
                    {
                        MouseOperations.Scroll(((ScrollAxis > 0f) ? 1 : (-1)) * num3, XInputManager.sim);
                        return false;
                    }
                }
                else if (__instance.HoveringOverlay.IsPluginApplication)
                {
                    ____tickAccumulator += num2 * (float)___ScrollClicksPerSecond * XSettingsManager.Instance.Settings.ScrollSpeed * Time.deltaTime;

                    int num3 = (int)____tickAccumulator;
                    if (num3 <= 0)
                        return false;

                    ____tickAccumulator -= (float)num3;

                    float num4 = ScrollAxis * (num3 * 0.05f);
                    __instance.HoveringOverlay.WebViewHandler.WebView.Scroll(new Vector2(0f, -num4), ___CursorUVNormalized);
                }
            }
            else
                LastScrollingTime = currentTime;

            return false;
        }

        private static bool IsEnable()
        {
            return XConfig.EnableRefreshRate.Value;
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
