using BepInEx.Configuration;

namespace xsoverlay_tweak
{
    internal class XConfig
    {
        public static ConfigEntry<int> RefreshRate;

        public static ConfigEntry<bool> AlwayUpdateCursor;
        public static ConfigEntry<bool> AlwayHideCursor;

        public static ConfigEntry<bool> ActivePointerColor;
        public static ConfigEntry<float> ActivePointerOpacity;
        public static ConfigEntry<int> PointerScale;
        public static ConfigEntry<bool> PointerDoubleClickDelay;
        public static ConfigEntry<bool> PhysicalMouseDetector;

        public static ConfigEntry<bool> MouseNavigation;
        public static ConfigEntry<bool> MouseNavigationUseModifiedKey;

        public static void AllConfig(ConfigFile cfg)
        {
            //?? General
            RefreshRate = cfg.Bind("General", "RefreshRate", 0, @"Change the XSOverlay render frame rate.
The higher value means more responsive and more CPU usage.
Set to -1 means unlimited.
Set to 0 means do the same as your VR Headset refresh rate as default XSOverlay does.
A value less than your VR Headset refresh rate means no effect.");

            //?? Cursor
            AlwayUpdateCursor = cfg.Bind("Cursor", "AlwayUpdateCursor", true, @"By default, XSOverlay displays the captured Desktop before sending new cursor position data to the actual cursor
, which means you are always seeing the previous cursor of the old frame.
Enable this to update the actual Desktop cursor before getting capture in the next frame.");
            AlwayHideCursor = cfg.Bind("Cursor", "AlwayHideCursor", false, "Always hide Window Capture cursor.");
            PhysicalMouseDetector = cfg.Bind("Cursor", "PhysicalMouseDetector", true, "When a physical mouse is moving the desktop cursor, Pointer will no longer control the cursor until clicking.");

            //?? Pointer
            ActivePointerColor = cfg.Bind("Pointer", "ActivePointerColor", true, "Determine the activated hand Pointer by red color.");
            ActivePointerOpacity = cfg.Bind("Pointer", "ActivePointerOpacity", 0.5f, "Determine the deactivated hand Pointer by opacity.");
            PointerScale = cfg.Bind("Pointer", "PointerScale", 0, @"Change the Pointer scale over then 100% in common setting.
A value less then 100 means no effect.");
            PointerDoubleClickDelay = cfg.Bind("Pointer", "PointerDoubleClickDelay", true, "Apply a Double Click Delay setting to the Pointer itself, not just the cursor.");

            //?? Mouse Navigation
            MouseNavigation = cfg.Bind("Mouse Navigation", "Enable", false, @"Enable custom keybinding to simulate the side mouse Forward/Back button.
Change the button by clicking the Binding tab in the XSOverlay settings. Then edit the Current Binding, assign button action for Mouse Back/Forward");
            MouseNavigationUseModifiedKey = cfg.Bind("Mouse Navigation", "UseModifiedKey", false, @"Using Alt+Left and Alt+Right instead of Mouse Input.
Due to Mouse Input has to hover the cursor over the target window to interact, but the modified key will interact with the current focused window instead.");
        }
    }
}
