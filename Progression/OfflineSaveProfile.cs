﻿using Comfort.Common;
using EFT;
using HarmonyLib;
using Newtonsoft.Json;
using SIT.Tarkov.Core;
using System;
using System.Linq;
using System.Reflection;

namespace SIT.Tarkov.SP
{
    public class OfflineSaveProfile : ModulePatch
    {
        private static readonly JsonConverter[] _defaultJsonConverters;

        static OfflineSaveProfile()
        {
            // compile-time check
            //_ = nameof(ClientMetrics.Metrics);


            //_defaultJsonConverters = Traverse.Create(converterClass).Field<JsonConverter[]>("Converters").Value;
        }

        protected override MethodBase GetTargetMethod()
        {
            foreach (var method in Constants.Instance.MainApplicationType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (method.Name.StartsWith("method") &&
                    method.GetParameters().Length == 6 &&
                    method.GetParameters()[0].ParameterType.Name == "String" &&
                    method.GetParameters()[3].Name == "isLocal" &&
                    method.GetParameters()[3].ParameterType.Name == "Boolean")
                {
                    //Logger.Log(BepInEx.Logging.LogLevel.Info, method.Name);
                    return method;
                }
            }
            Logger.Log(BepInEx.Logging.LogLevel.Error, "OfflineSaveProfile::Method is not found!");

            return null;
        }

        [PatchPrefix]
        public static bool PatchPrefix(
            ref ESideType ___esideType_0
            , ref Result<EFT.ExitStatus, TimeSpan, object> result)
        //    [PatchPostfix]
        //public static void PatchPostfix(ref ESideType ___esideType_0, ref object result)
        {
            Logger.Log(BepInEx.Logging.LogLevel.Info, "OfflineSaveProfile::PatchPrefix");
            var session = ClientAccesor.GetClientApp().GetClientBackEndSession();
            var isPlayerScav = false;
            var profile = session.Profile;

            if (___esideType_0 == ESideType.Savage)
            {
                profile = session.ProfileOfPet;
                isPlayerScav = true;
            }

            var currentHealth = new PlayerHealth();
            //currentHealth.Energy = 100;
            //currentHealth.Hydration = 100;
            //var currentHealth = 400;

            //SaveProfileProgress(SIT.Tarkov.Core.PatchConstants.GetBackendUrl(), session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);

            var beUrl = SIT.Tarkov.Core.PatchConstants.GetBackendUrl();
            var sessionId = SIT.Tarkov.Core.PatchConstants.GetPHPSESSID();
            Logger.LogInfo(beUrl);
            Logger.LogInfo(sessionId);

            SaveProfileProgress(beUrl
                , sessionId
                , EFT.ExitStatus.Survived
                , profile
                , currentHealth
                , isPlayerScav);
            //Utility.Progression.SaveLootUtil.SaveProfileProgress(ClientAccesor.BackendUrl, session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);
            return true;
        }

        public static void SaveProfileProgress(
            string backendUrl
            , string session, EFT.ExitStatus exitStatus, EFT.Profile profileData, PlayerHealth currentHealth, bool isPlayerScav)
        {
            SaveProfileRequest request = new SaveProfileRequest
            {
                exit = exitStatus.ToString().ToLower(),
                profile = profileData,
                health = currentHealth,
                isPlayerScav = isPlayerScav
            };

            var convertedJson = request.SITToJson();
            new Request(session, backendUrl).PostJson("/raid/profile/save", convertedJson);
           
        }

        public class SaveProfileRequest
        {
            public string exit { get; set; }
            public EFT.Profile profile { get; set; }
            public bool isPlayerScav { get; set; }
            public PlayerHealth health { get; set; }
        }
    }
}
