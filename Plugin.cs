using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace xsoverlay_tweak;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static Plugin Instance;

    private static readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        XConfig.AllConfig(Config);

        if (XConfig.AlwayUpdateCursor.Value)
            harmony.PatchAll(typeof(Patches.AlwayUpdateCursor));

        if (!XConfig.RefreshRate.Value.Equals(0))
            harmony.PatchAll(typeof(Patches.RefreshRate));

        if (XConfig.ActivePointerColor.Value)
            harmony.PatchAll(typeof(Patches.ActivePointerColor));

        if (XConfig.AlwayHideCursor.Value)
            harmony.PatchAll(typeof(Patches.AlwayHideCursor));

        if (XConfig.MouseNavigation.Value)
            harmony.PatchAll(typeof(Patches.MouseNavigation));

        if (XConfig.PointerDoubleClickDelay.Value)
            harmony.PatchAll(typeof(Patches.PointerDoubleClickDelay));
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        Instance = this;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
