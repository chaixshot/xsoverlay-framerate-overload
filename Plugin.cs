using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace xsoverlay_framerate_overload;

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
            harmony.PatchAll(typeof(Patches.UpdateCursor));

        if (!XConfig.RefreshRate.Value.Equals(0))
            harmony.PatchAll(typeof(Patches.RefreshRate));

        if (XConfig.ActivePointerColor.Value)
            harmony.PatchAll(typeof(Patches.ActivePointerColor));

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        Instance = this;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is started!");
    }
}
