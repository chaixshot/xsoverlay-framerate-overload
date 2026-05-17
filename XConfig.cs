using BepInEx.Configuration;

namespace xsoverlay_tweak
{
    internal class XConfig
    {
        public static ConfigEntry<bool> EnableRefreshRate;
        public static ConfigEntry<int> RefreshRate;

        public static ConfigEntry<bool> AlwayUpdateCursor;
        public static ConfigEntry<bool> AlwaysHideCursor;

        public static ConfigEntry<bool> ActivesPointerColor;
        public static ConfigEntry<float> ActivePointerOpacity;
        public static ConfigEntry<float> PointerScaleMultiply;
        public static ConfigEntry<bool> PointerDoubleClickDelay;
        public static ConfigEntry<bool> PhysicalMouseDetector;

        public static ConfigEntry<bool> MouseNavigation;
        public static ConfigEntry<bool> MouseNavigationUseModifiedKey;

        public static void AllConfig(ConfigFile cfg)
        {
            //?? RefreshRate
            EnableRefreshRate = cfg.Bind("RefreshRate", "EnableRefreshRate", false, "Enable custom refresh rate override.");
            RefreshRate = cfg.Bind("RefreshRate", "RefreshRate", -1, @"Change the XSOverlay render frame rate.
The higher value means more responsive and more CPU usage.");

            //?? Cursor
            AlwayUpdateCursor = cfg.Bind("Cursor", "AlwayUpdateCursor", true, @"By default, XSOverlay displays the captured Desktop before sending new cursor position data to the actual cursor
, which means you are always seeing the previous cursor of the old frame.
Enable this to update the actual Desktop cursor before getting capture in the next frame.");
            AlwaysHideCursor = cfg.Bind("Cursor", "AlwaysHideCursor", false, "Always hide Window Capture cursor.");
            PhysicalMouseDetector = cfg.Bind("Cursor", "PhysicalMouseDetector", true, "When a physical mouse is moving the desktop cursor, Pointer will no longer control the cursor until clicking.");

            //?? Pointer
            ActivesPointerColor = cfg.Bind("Pointer", "ActivesPointerColor", true, "Determine the activated hand Pointer by red color.");
            ActivePointerOpacity = cfg.Bind("Pointer", "ActivePointerOpacity", 0.5f, "Determine the deactivated hand Pointer by opacity.");
            PointerScaleMultiply = cfg.Bind("Pointer", "PointerScaleMultiply", 1f, "Multiply the Pointer scale from the common setting.");
            PointerDoubleClickDelay = cfg.Bind("Pointer", "PointerDoubleClickDelay", true, "Apply a Double Click Delay setting to the Pointer itself, not just the cursor.");

            //?? Mouse Navigation
            MouseNavigation = cfg.Bind("Mouse Navigation", "MouseNavigation", false, @"Enable custom keybinding to simulate the side mouse Forward/Back button.
Change the button by clicking the Binding tab in the XSOverlay settings. Then edit the Current Binding, assign button action for Mouse Back/Forward");
            MouseNavigationUseModifiedKey = cfg.Bind("Mouse Navigation", "MouseNavigationUseModifiedKey", false, @"Using Alt+Left and Alt+Right instead of Mouse Input.
Due to Mouse Input has to hover the cursor over the target window to interact, but the modified key will interact with the current focused window instead.");
        }
    }
}
