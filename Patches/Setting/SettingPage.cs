using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Vuplex.WebView;
using XSOverlay;
using XSOverlay.WebApp;
using XSOverlay.Websockets.API;

namespace xsoverlay_tweak.Patches.Setting
{
    internal class SettingPage
    {
        [Serializable]
        public class TweakSettings
        {
            public int RefreshRate;
            public bool AlwayUpdateCursor;
            public bool AlwaysHideCursor;
            public bool PhysicalMouseDetector;
            public bool ActivesPointerColor;
            public float ActivePointerOpacity;
            public float PointerScaleMultiply;
            public bool PointerDoubleClickDelay;
            public bool MouseNavigation;
            public bool MouseNavigationUseModifiedKey;
        }

        [HarmonyPatch(typeof(Overlay_Manager), "OnRegisterWebviewOverlay")]
        [HarmonyPostfix]
        public static void WebviewOverlay(ref OverlayWebView wv)
        {
            if (wv.UserInterfaceSelection == OverlayWebView.UserInterfacePaths.Settings)
                InjectSettingsModule(wv);
        }

        [HarmonyPatch(typeof(ApiHandler), "OnRequestCurrentSettings")]
        [HarmonyPostfix]
        public static void OnRequestCurrentSettings(string sender)
        {
            TweakSettings settings = new()
            {
                RefreshRate = XConfig.RefreshRate.Value,
                AlwayUpdateCursor = XConfig.AlwayUpdateCursor.Value,
                AlwaysHideCursor = XConfig.AlwaysHideCursor.Value,
                PhysicalMouseDetector = XConfig.PhysicalMouseDetector.Value,
                ActivesPointerColor = XConfig.ActivesPointerColor.Value,
                ActivePointerOpacity = XConfig.ActivePointerOpacity.Value,
                PointerScaleMultiply = XConfig.PointerScaleMultiply.Value,
                PointerDoubleClickDelay = XConfig.PointerDoubleClickDelay.Value,
                MouseNavigation = XConfig.MouseNavigation.Value,
                MouseNavigationUseModifiedKey = XConfig.MouseNavigationUseModifiedKey.Value
            };

            var data = JsonUtility.ToJson(settings);
            ServerClientBridge.Instance.Api.SendMessage("UpdateSettings", data, null, sender);
        }

        [HarmonyPatch(typeof(XSettingsManager)), HarmonyPatch(nameof(XSettingsManager.SetSetting))]
        [HarmonyPrefix]
        public static bool SetSetting(string name, string value, string value1, bool sendAnalytics = true)
        {
            switch (name)
            {
                case "RefreshRate":
                    XConfig.RefreshRate.Value = int.Parse(value);
                    break;
                case "AlwayUpdateCursor":
                    XConfig.AlwayUpdateCursor.Value = bool.Parse(value);
                    break;
                case "AlwaysHideCursor":
                    XConfig.AlwaysHideCursor.Value = bool.Parse(value);
                    break;
                case "PhysicalMouseDetector":
                    XConfig.PhysicalMouseDetector.Value = bool.Parse(value);
                    break;
                case "ActivesPointerColor":
                    XConfig.ActivesPointerColor.Value = bool.Parse(value);
                    break;
                case "ActivePointerOpacity":
                    XConfig.ActivePointerOpacity.Value = float.Parse(value);
                    break;
                case "PointerScaleMultiply":
                    XConfig.PointerScaleMultiply.Value = float.Parse(value);
                    break;
                case "PointerDoubleClickDelay":
                    XConfig.PointerDoubleClickDelay.Value = bool.Parse(value);
                    break;
                case "MouseNavigation":
                    XConfig.MouseNavigation.Value = bool.Parse(value);
                    break;
                case "MouseNavigationUseModifiedKey":
                    XConfig.MouseNavigationUseModifiedKey.Value = bool.Parse(value);
                    break;
            }

            return true;
        }

        public static void InjectSettingsModule(OverlayWebView wv)
        {

            // JS for inserting the actual settings page
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("xsoverlay_tweak.Patches.Setting.setting.js");
            using var reader = new StreamReader(stream);
            var jsContent = reader.ReadToEnd();
            string jsCode = $"(function() {{ {jsContent} }})();";

            // Lisen for WebView loaded
            wv._webView.WebView.LoadProgressChanged += (sender, args) =>
            {
                if (args.Type == ProgressChangeType.Finished)
                {
                    wv._webView.WebView.ExecuteJavaScript(jsCode, (result) =>
                    {
                        //Plugin.Logger.LogError($"[{wv.UserInterfaceSelection}] {result}");
                    });
                }
            };

        }
    }
}
