using HarmonyLib;
using System;
using UnityEngine;
using XSOverlay;

namespace xsoverlay_framerate_overload.Patches
{
    [HarmonyPatch(typeof(Raycaster))]
    internal class UpdateCursor
    {
        private static Raycaster R_Raycaster;
        private static Raycaster L_Raycaster;

        private static LayerMask R_DesktopCursorCastMask;
        private static LayerMask L_DesktopCursorCastMask;

        private delegate void SyncedUpdateDelegate(Raycaster instance, Unity_Overlay overlay);
        private static SyncedUpdateDelegate R_SyncedUpdate;
        private static SyncedUpdateDelegate L_SyncedUpdate;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start(Raycaster __instance)
        {
            if (!(__instance.DeviceIdx is Raycaster.DeviceIdxFetch.RightHand or Raycaster.DeviceIdxFetch.LeftHand)) return;

            ref Raycaster raycaster = ref (IsRightHand(__instance) ? ref R_Raycaster : ref L_Raycaster);
            ref LayerMask DesktopCursorCastMask = ref (IsRightHand(__instance) ? ref R_DesktopCursorCastMask : ref L_DesktopCursorCastMask);
            ref SyncedUpdateDelegate SyncedUpdate = ref (IsRightHand(__instance) ? ref R_SyncedUpdate : ref L_SyncedUpdate);

            // Remove listener from overlay update 
            var SyncedOverlayUpdate = AccessTools.Method(typeof(Raycaster), "SyncedOverlayUpdate");
            var handler = (Action<Unity_Overlay>)Delegate.CreateDelegate(typeof(Action<Unity_Overlay>), raycaster, SyncedOverlayUpdate);
            XSOEventSystem.OnUpdatedOverlay -= handler;

            raycaster = __instance;
            DesktopCursorCastMask = (LayerMask)AccessTools.Field(typeof(Raycaster), "DesktopCursorCastMask").GetValue(raycaster);
            SyncedUpdate = AccessTools.MethodDelegate<SyncedUpdateDelegate>(SyncedOverlayUpdate);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update(Raycaster __instance)
        {
            if (!(__instance.DeviceIdx is Raycaster.DeviceIdxFetch.RightHand or Raycaster.DeviceIdxFetch.LeftHand)) return;

            ref Raycaster raycaster = ref (IsRightHand(__instance) ? ref R_Raycaster : ref L_Raycaster);
            ref SyncedUpdateDelegate SyncedUpdate = ref (IsRightHand(__instance) ? ref R_SyncedUpdate : ref L_SyncedUpdate);

            SyncedUpdate?.Invoke(raycaster, new Unity_Overlay());
        }

        private static bool IsRightHand(Raycaster raycaster)
        {
            return raycaster.DeviceIdx is Raycaster.DeviceIdxFetch.RightHand;
        }
    }
}
