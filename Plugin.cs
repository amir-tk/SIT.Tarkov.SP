using BepInEx;
using SIT.B.Tarkov.SP.MatchMaker;
using SIT.B.Tarkov.SP.Progression;
using SIT.Tarkov.SP;
using SIT.Tarkov.SP.Progression;
using SIT.Tarkov.SP.Raid;
using SIT.Tarkov.SP.Raid.Aki;

namespace SIT.Tarkov.SP
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(SIT.A.Tarkov.Core.PluginInfo.PLUGIN_GUID)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // -------------------------------------
            // Matchmaker
            new AutoSetOfflineMatch().Enable();
            new BringBackInsuranceScreen().Enable();
            new DisableReadyButtonOnFirstScreen().Enable();
            new DisableReadyButtonOnSelectLocation().Enable();

            // -------------------------------------
            // Progression
            new OfflineSaveProfile().Enable();
            new ExperienceGainFix().Enable();

            // -------------------------------------
            // Quests
            new ItemDroppedAtPlace_Beacon().Enable();


            // -------------------------------------
            // Raid
            new LoadBotDifficultyFromServer().Enable();
            //new RemoveUsedBotProfile().Enable();
            //new BotSettingsLoad().Enable();

            // 
            //new ReplaceInMainMenuController().Enable();

            // --------------------------------------
            // Health stuff
            new ReplaceInPlayer().Enable();

            new ChangeHealthPatch().Enable();
            new ChangeEnergyPatch().Enable();
            new ChangeHydrationPatch().Enable();

            // --------------------------------------
            // Air Drop
            new AirdropBoxPatch().Enable();
            new AirdropPatch().Enable();


            // --------------------------------------
            // Bots
            new IsEnemyPatch().Enable();
            new IsEnemyBossPatch().Enable();
            //new CheckAndAddEnemyPatch().Enable();
            new RemoveUsedBotProfile().Enable();
            //new BotSpawnPatch().Enable();


            Instance = this;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static Plugin Instance;
    }
}
