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
            if (__instance.HapticDeviceName == Raycaster.HapticDevice.None) return;

            var SyncedOverlayUpdate = AccessTools.Method(typeof(Raycaster), "SyncedOverlayUpdate");
            _handArray.Add(new HandData
            {
                Instance = __instance,
                SyncedOverlayUpdate = AccessTools.MethodDelegate<SyncedUpdateDelegate>(SyncedOverlayUpdate)
            });
        }

        [HarmonyPatch("SubscribeToEvents"), HarmonyPatch("UnsubscribeFromEvents")]
        [HarmonyPostfix]
        public static void SubscribeToEvents(Raycaster __instance)
        {
            if (__instance.HapticDeviceName == Raycaster.HapticDevice.None) return;

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
    }
}
