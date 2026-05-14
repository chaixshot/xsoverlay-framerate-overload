using HarmonyLib;
using System;
using System.Collections.Generic;
using XSOverlay;

namespace xsoverlay_framerate_overload.Patches
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

            // Remove listener from overlay update 
            var SyncedOverlayUpdate = AccessTools.Method(typeof(Raycaster), "SyncedOverlayUpdate");
            var handler = (Action<Unity_Overlay>)Delegate.CreateDelegate(typeof(Action<Unity_Overlay>), __instance, SyncedOverlayUpdate);
            XSOEventSystem.OnUpdatedOverlay -= handler;

            _handArray.Add(new HandData
            {
                Instance = __instance,
                SyncedOverlayUpdate = AccessTools.MethodDelegate<SyncedUpdateDelegate>(SyncedOverlayUpdate)
            });
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update()
        {
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
    }
}
