using EFT;
using EFT.Bots;
using EFT.UI;
using SIT.Tarkov.Core;
using System;
using System.Reflection;
using UnityEngine;

namespace SIT.Tarkov.SP.MatchMaker
{
    /// <summary>
    /// This patch sets matches to offline on screen enter also sets other variables directly from server settings
    /// URL called:"/singleplayer/settings/raid/menu"
    /// </summary>
    class AutoSetOfflineMatch : ModulePatch
    {
        public AutoSetOfflineMatch()
        {
        }

        [PatchPostfix]
        public static void PatchPostfix(UpdatableToggle ____offlineModeToggle, UpdatableToggle ____botsEnabledToggle,
            DropDownBox ____aiAmountDropdown, DropDownBox ____aiDifficultyDropdown, UpdatableToggle ____enableBosses,
            UpdatableToggle ____scavWars, UpdatableToggle ____taggedAndCursed)
        {
            ____offlineModeToggle.isOn = true;
            ____offlineModeToggle.gameObject.SetActive(false);
            ____botsEnabledToggle.isOn = true;

            var defaultRaidSettings = Request();

            if (defaultRaidSettings != null)
            {
                ____aiAmountDropdown.UpdateValue((int)defaultRaidSettings.AiAmount, false);
                ____aiDifficultyDropdown.UpdateValue((int)defaultRaidSettings.AiDifficulty, false);
                ____enableBosses.isOn = defaultRaidSettings.BossEnabled;
                ____scavWars.isOn = defaultRaidSettings.ScavWars;
                ____taggedAndCursed.isOn = defaultRaidSettings.TaggedAndCursed;
            }

            var warningPanel = GameObject.Find("Warning Panel");
            UnityEngine.Object.Destroy(warningPanel);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.Matchmaker.MatchmakerOfflineRaid).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static DefaultRaidSettings Request()
        {
            var json = new Request(null, ClientAccesor.BackendUrl).GetJson("/singleplayer/settings/raid/menu");

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JET]: Received NULL response for DefaultRaidSettings. Defaulting to fallback.");
                return null;
            }

            Debug.LogError("[JET]: Successfully received DefaultRaidSettings");

            try
            {
                return json.ParseJsonTo<DefaultRaidSettings>();
            }
            catch (Exception exception)
            {
                Debug.LogError("[JET]: Failed to deserialize DefaultRaidSettings from server. Check your gameplay.json config in your server. Defaulting to fallback. Exception: " + exception);
                return null;
            }
        }
    }

    public class DefaultRaidSettings
    {
        public EBotAmount AiAmount;
        public EBotDifficulty AiDifficulty;
        public bool BossEnabled;
        public bool ScavWars;
        public bool TaggedAndCursed;

        public DefaultRaidSettings(EBotAmount aiAmount, EBotDifficulty aiDifficulty, bool bossEnabled, bool scavWars, bool taggedAndCursed)
        {
            AiAmount = aiAmount;
            AiDifficulty = aiDifficulty;
            BossEnabled = bossEnabled;
            ScavWars = scavWars;
            TaggedAndCursed = taggedAndCursed;
        }
    }
}