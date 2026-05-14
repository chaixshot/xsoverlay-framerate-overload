using BepInEx.Configuration;

namespace xsoverlay_framerate_overload
{
    internal class Utils
    {


    }

    internal class XConfig
    {
        public static ConfigEntry<int> RefreshRate;
        public static ConfigEntry<bool> AlwayUpdateCursor;
        public static ConfigEntry<bool> ActivePointerColor;

        public static void AllConfig(ConfigFile cfg)
        {
            RefreshRate = cfg.Bind("General", "RefreshRate", 0, @"Change the XSOverlay render frame rate.
The higher value is more responsive and more CPU usage.
Set to -1 means unlimited.
Set to 0 means same as your VR Headset refresh rate as default XSOverlay does.
A value less than your VR Headset means no effect.");
            AlwayUpdateCursor = cfg.Bind("General", "AlwayUpdateCursor", true, @"By default, XSOverlay displays the capture Desktop before sending new cursor position data to the actual cursor,
which means you are always seeing the previous cursor of the old frame.
Enable this to update the actual Desktop cursor before getting captured in the next frame");
            ActivePointerColor = cfg.Bind("General", "ActivePointerColor", true, @"Determine the active hand Pointer by color.");
        }
    }
}
