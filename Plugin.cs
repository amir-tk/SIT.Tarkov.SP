using BepInEx;
using SinglePlayerMod.Patches.Progression;
using SinglePlayerMod.Patches.Raid;
using SIT.Tarkov.SP.Raid;

namespace SIT.Tarkov.SP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new OfflineSaveProfile().Enable();
            new BotTemplateLimit().Enable();
            new LoadBotDifficultyFromServer().Enable();
            new LoadBotTemplatesFromServer().Enable();
            new MaxBotCap().Enable();
            new RemoveUsedBotProfile().Enable();
            new SpawnPmc().Enable();
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
