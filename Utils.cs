using BepInEx.Configuration;

namespace xsoverlay_tweak
{
    internal class Utils
    {


    }

    internal class XConfig
    {
        public static ConfigEntry<int> RefreshRate;

        public static ConfigEntry<bool> AlwayUpdateCursor;
        public static ConfigEntry<bool> AlwayHideCursor;
        public static ConfigEntry<bool> ActivePointerColor;
        public static ConfigEntry<bool> DoubleClickDelay;

        public static ConfigEntry<bool> MouseNavigation;
        public static ConfigEntry<bool> MouseNavigationUseModifiedKey;

        public static void AllConfig(ConfigFile cfg)
        {
            RefreshRate = cfg.Bind("General", "RefreshRate", 0, @"Change the XSOverlay render frame rate.
The higher value means more responsive and more CPU usage
Set to -1 means unlimited
Set to 0 means do the same as your VR Headset refresh rate as default XSOverlay does
A value less than your VR Headset refresh rate means no effect");

            AlwayUpdateCursor = cfg.Bind("Cursor", "AlwayUpdateCursor", true, @"By default, XSOverlay displays the capture Desktop before sending new cursor position data to the actual cursor,
which means you are always seeing the previous cursor of the old frame
Enable this to update the actual Desktop cursor before getting captured in the next frame");
            AlwayHideCursor = cfg.Bind("Cursor", "AlwayHideCursor", false, "Alwalys hide Window captured cursor");
            ActivePointerColor = cfg.Bind("Cursor", "ActivePointerColor", true, "Determine the activated hand Pointer by color and opacity");
            DoubleClickDelay = cfg.Bind("Cursor", "DoubleClickDelay", true, "Apply a double-click delay setting to the Pointer itself, not just the cursor");

            MouseNavigation = cfg.Bind("Mouse Navigation", "Enable", true, "Enable custom keybinding to simulate the side mouse Forward/Back button");
            MouseNavigationUseModifiedKey = cfg.Bind("Mouse Navigation", "UseModifiedKey", false, @"Using [Alt]+[Left] and [Alt]+[Right] instead of Mouse Input
Due to Mouse Input has to hover the cursor over the target window to interact, but the modified key will interact with the current focused window instead");
        }
    }
}
