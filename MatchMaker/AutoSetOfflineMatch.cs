﻿using EFT;
using EFT.Bots;
using EFT.UI;
using SIT.Tarkov.Core;
using System;
using System.Reflection;
using UnityEngine;

namespace SIT.B.Tarkov.SP.MatchMaker
{
    /// <summary>
    /// This patch sets matches to offline on screen enter also sets other variables directly from server settings
    /// URL called:"/singleplayer/settings/raid/menu"
    /// </summary>
    public class AutoSetOfflineMatch : ModulePatch
    {
        public AutoSetOfflineMatch()
        {
        }

        private static DefaultRaidSettings raidSettings = null;


        [PatchPostfix]
        public static void PatchPostfix(UpdatableToggle ____offlineModeToggle, UpdatableToggle ____botsEnabledToggle,
            DropDownBox ____aiAmountDropdown, DropDownBox ____aiDifficultyDropdown, UpdatableToggle ____enableBosses,
            UpdatableToggle ____scavWars, UpdatableToggle ____taggedAndCursed)
        {

            Logger.LogInfo("AutoSetOfflineMatch.PatchPostfix");

            var warningPanel = GameObject.Find("Warning Panel");
            UnityEngine.Object.Destroy(warningPanel);

            // Do a force of these, just encase it breaks
            ____offlineModeToggle.isOn = true;
            ____offlineModeToggle.gameObject.SetActive(false);
            ____offlineModeToggle.interactable = false;
            ____botsEnabledToggle.isOn = true;
            ____enableBosses.isOn = true;

            ____aiAmountDropdown.UpdateValue((int)EBotAmount.Medium, false);
            ____aiDifficultyDropdown.UpdateValue((int)EBotDifficulty.Medium, false);

            Request();

            if (raidSettings != null)
            {
                ____aiAmountDropdown.UpdateValue((int)raidSettings.AiAmount, false);
                ____aiDifficultyDropdown.UpdateValue((int)raidSettings.AiDifficulty, false);
                ____enableBosses.isOn = raidSettings.BossEnabled;
                ____scavWars.isOn = raidSettings.ScavWars;
                ____taggedAndCursed.isOn = raidSettings.TaggedAndCursed;
            }
            else
            {
                Logger.LogInfo("AutoSetOfflineMatch.PatchPostfix : Raid Settings are Null!");
            }

          
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(EFT.UI.Matchmaker.MatchmakerOfflineRaid).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
        }


        public static DefaultRaidSettings Request()
        {
            try
            {
                //if (raidSettings == null)
                //{
                Logger.LogInfo("AutoSetOfflineMatch.Request");

                var json = new Request(null, SIT.Tarkov.Core.PatchConstants.GetBackendUrl()).GetJson("/singleplayer/settings/raid/menu");

                if (string.IsNullOrWhiteSpace(json))
                {
                    Logger.LogError("Received NULL response for DefaultRaidSettings. Defaulting to fallback.");
                    return null;
                }

                try
                {
                    raidSettings = json.ParseJsonTo<DefaultRaidSettings>();
                    Logger.LogInfo("Obtained DefaultRaidSettings from Server");
                }
                catch (Exception exception)
                {
                    Logger.LogError("Failed to deserialize DefaultRaidSettings from server. Check your gameplay.json config in your server. Defaulting to fallback. Exception: " + exception);
                    return null;
                }
                //}
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
            finally
            {

            }


            return raidSettings;

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