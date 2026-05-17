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
            EnableRefreshRate = cfg.Bind("RefreshRate", "EnableRefreshRate", false, "Enable overriding the XSOverlay render refresh rate.");
            RefreshRate = cfg.Bind("RefreshRate", "RefreshRate", -1, "The target frame rate for XSOverlay rendering. Higher values improve responsiveness but increase CPU usage.");

            //?? Cursor
            AlwayUpdateCursor = cfg.Bind("Cursor", "AlwayUpdateCursor", true, "Reduces perceived cursor latency by ensuring the system cursor is updated immediately before the desktop frame is captured. Without this, the cursor often appears to lag one frame behind the actual pointer position.");
            AlwaysHideCursor = cfg.Bind("Cursor", "AlwaysHideCursor", false, "Forcefully hide the system cursor in Window Capture overlays.");
            PhysicalMouseDetector = cfg.Bind("Cursor", "PhysicalMouseDetector", true, "Automatically release VR pointer control when physical mouse movement is detected. Control is regained upon clicking.");

            //?? Pointer
            ActivesPointerColor = cfg.Bind("Pointer", "ActivesPointerColor", true, "Highlight the non-active hand's pointer in red for easier identification.");
            ActivePointerOpacity = cfg.Bind("Pointer", "ActivePointerOpacity", 0.5f, "Set the opacity of the non-active hand's pointer.");
            PointerScaleMultiply = cfg.Bind("Pointer", "PointerScaleMultiply", 1f, "Multiplier for the pointer scale relative to the global XSOverlay settings.");
            PointerDoubleClickDelay = cfg.Bind("Pointer", "PointerDoubleClickDelay", true, "Apply a Double Click Delay setting to the Pointer itself, not just the cursor.");

            //?? Mouse Navigation
            MouseNavigation = cfg.Bind("Mouse Navigation", "MouseNavigation", false, "Enable custom keybindings for Mouse Forward/Back navigation. Configuration is done via the SteamVR 'Binding' tab in XSOverlay settings.");
            MouseNavigationUseModifiedKey = cfg.Bind("Mouse Navigation", "MouseNavigationUseModifiedKey", false, "Use Alt+Left/Right keyboard shortcuts for navigation instead of mouse clicks. This targets the focused window instead of the hovered window.");
        }
    }
}
