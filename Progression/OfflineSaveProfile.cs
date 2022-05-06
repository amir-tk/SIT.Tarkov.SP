using Comfort.Common;
using JET.Utility;
using JET.Utility.Patching;
using SinglePlayerMod.Utility.Progression;
using SIT.Tarkov.Core;
using System;
using System.Reflection;

namespace SinglePlayerMod.Patches.Progression
{
    public class OfflineSaveProfile : ModulePatch
    {

        public OfflineSaveProfile()
        {
            // compile-time check
            //_ = nameof(ClientMetrics.Metrics);
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
                    Logger.Log(BepInEx.Logging.LogLevel.Info, method.Name);
                    return method;
                }
            }
            Logger.Log(BepInEx.Logging.LogLevel.Error, "OfflineSaveProfile::Method is not found!");

            return null;
        }

        [PatchPrefix]
        public static void PatchPrefix(ESideType ___esideType_0, Result<EFT.ExitStatus, TimeSpan, object> result)
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
            currentHealth.Energy = 100;
            currentHealth.Hydration = 100;
            //var currentHealth = 400;

            SaveProfileProgress(ClientAccesor.BackendUrl, session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);
            //Utility.Progression.SaveLootUtil.SaveProfileProgress(ClientAccesor.BackendUrl, session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);
        }

        public static void SaveProfileProgress(string backendUrl, string session, EFT.ExitStatus exitStatus, EFT.Profile profileData, PlayerHealth currentHealth, bool isPlayerScav)
        {
            SaveProfileRequest request = new SaveProfileRequest
            {
                exit = exitStatus.ToString().ToLower(),
                profile = profileData,
                health = currentHealth,
                isPlayerScav = isPlayerScav
            };

            // ToJson() uses an internal converter which prevents loops and do other internal things
            Logger.LogInfo("SaveProfileProgress::" + request.ToJson().ToString());
            new Request(session, backendUrl).PostJson("/raid/profile/save", request.ToJson());
        }

        public class SaveProfileRequest
        {
            public string exit = "left";
            public EFT.Profile profile;
            public bool isPlayerScav;
            public PlayerHealth health;
        }
    }
}
