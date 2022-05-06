using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using UnityEngine;
//using WaveInfo = GClass1224; // search for: Difficulty and choose gclass with lower number which contains Role and Limit variables
//using BotsPresets = GClass552; // Method: GetNewProfile (higher GClass number)
using SIT.Tarkov.Core;
using System.Linq;
// Method: GetNewProfile (higher GClass number)

namespace SIT.Tarkov.SP.Raid
{
    public class BotTemplateLimit : ModulePatch
    {
        public static Type TypeWithGetNewProfile;
        public static MethodBase Method;

        public static Type WaveInfoType;

        public BotTemplateLimit()
        {
            // compile-time checks
            //_ = nameof(BotsPresets.CreateProfile);
            //_ = nameof(WaveInfo.Difficulty);

            WaveInfoType = PatchConstants.EftTypes.FirstOrDefault(x
                => x.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Any(y => y.Name == "Difficulty")
                && x.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Any(y => y.Name == "Limit")
                && x.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Any(y => y.Name == "Role"));

            if(WaveInfoType != null)
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} : BotTemplateLimit : WaveInfoType found {WaveInfoType.Name}");
            }


            if (TypeWithGetNewProfile == null)
            {
                TypeWithGetNewProfile = PatchConstants.EftTypes.LastOrDefault(x => x.GetMethods().Any(y => y.Name.Contains("GetNewProfile")));
            }

            if (TypeWithGetNewProfile != null && Method == null)
            {
                //Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} : BotTemplateLimit : TypeWithGetNewProfile found {TypeWithGetNewProfile.Name}");

                Method = TypeWithGetNewProfile.GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if(Method != null)
                {
                    Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} : BotTemplateLimit : Method found {Method.Name}");
                }
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            //return typeof(BotsPresets).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return Method;
        }

        [PatchPostfix]

        public static void PatchPostfix(object __result/*, List<WaveInfo> wavesProfiles*/, object delayed)
        {
            if(delayed != null && delayed is List<object>)
            {
                Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} : BotTemplateLimit : Delayed found");
                ((List<object>)delayed).Clear();
            }

            //delayed?.Clear();

            //foreach (object wave in __result)
            //{
            //    if (wave.GetType() == WaveInfoType)
            //    {
            //        //wave.Limit = Request(wave.Role);
            //    }
            //}
        }

        //private static int Request(WildSpawnType role)
        //{
        //    return 5;
        //    //var json = new Request(null, ClientAccesor.BackendUrl).GetJson($"/singleplayer/settings/bot/limit/{role}");

        //    //if (string.IsNullOrWhiteSpace(json))
        //    //{
        //    //    //Debug.LogError("[JET]: Received bot " + role.ToString() + " limit data is NULL, using fallback");
        //    //    return 1;
        //    //}
        //    ////Debug.LogError("[JET]: Successfully received bot " + role.ToString() + " limit data");
        //    //return Convert.ToInt32(json);
        //}
    }
}
