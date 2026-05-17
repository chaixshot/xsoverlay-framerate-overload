using HarmonyLib;
using System;
using System.Collections.Generic;
using XSOverlay;

namespace xsoverlay_tweak.Patches
{
    [HarmonyPatch(typeof(Raycaster))]
    internal class AlwayUpdateCursor
    {
        private delegate void SyncedUpdateDelegate(Raycaster instance, Unity_Overlay overlay);
        private struct HandData
        {
            public Raycaster Instance;
            public SyncedUpdateDelegate SyncedOverlayUpdate;
        }
        private static readonly List<HandData> _handArray = [];

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(Raycaster __instance)
        {
            if (!IsController(__instance)) return;

            // Add to Update loop array
            var SyncedOverlayUpdate = AccessTools.Method(typeof(Raycaster), "SyncedOverlayUpdate");
            _handArray.Add(new HandData
            {
                Instance = __instance,
                SyncedOverlayUpdate = AccessTools.MethodDelegate<SyncedUpdateDelegate>(SyncedOverlayUpdate)
            });

            // Setting changed
            XConfig.AlwayUpdateCursor.SettingChanged += (sender, args) =>
            {
                if (IsEnable())
                    RemoveUpdatedOverlay(__instance);
                else
                    AddUpdatedOverlay(__instance);
            };
        }

        [HarmonyPatch("SubscribeToEvents"), HarmonyPatch("UnsubscribeFromEvents")]
        [HarmonyPostfix]
        public static void SubscribeToEvents(Raycaster __instance)
        {
            if (!IsEnable()) return;
            if (!IsController(__instance)) return;

            RemoveUpdatedOverlay(__instance);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            if (!IsEnable()) return;

            for (int i = _handArray.Count - 1; i >= 0; i--)
            {
                var data = _handArray[i];

                // Cleanup if destroyed
                if (data.Instance == null)
                {
                    _handArray.RemoveAt(i);
                    continue;
                }

                // Invoke the delegate stored in the array
                data.SyncedOverlayUpdate?.Invoke(data.Instance, new Unity_Overlay());
            }
        }

        private static void AddUpdatedOverlay(Raycaster __instance)
        {
            // Add listener from overlay update 
            var SyncedOverlayUpdate = AccessTools.Method(typeof(Raycaster), "SyncedOverlayUpdate");
            var handler = (Action<Unity_Overlay>)Delegate.CreateDelegate(typeof(Action<Unity_Overlay>), __instance, SyncedOverlayUpdate);
            XSOEventSystem.OnUpdatedOverlay += handler;
        }

        private static void RemoveUpdatedOverlay(Raycaster __instance)
        {
            // Remove listener from overlay update 
            var SyncedOverlayUpdate = AccessTools.Method(typeof(Raycaster), "SyncedOverlayUpdate");
            var handler = (Action<Unity_Overlay>)Delegate.CreateDelegate(typeof(Action<Unity_Overlay>), __instance, SyncedOverlayUpdate);
            XSOEventSystem.OnUpdatedOverlay -= handler;
        }

        private static bool IsEnable()
        {
            return XConfig.AlwayUpdateCursor.Value;
        }
        private static bool IsController(Raycaster __instance)
        {
            return __instance.HapticDeviceName != Raycaster.HapticDevice.None;
        }
    }
}
