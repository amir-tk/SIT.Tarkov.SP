using EFT;
using SIT.Tarkov.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SIT.Tarkov.SP.Raid
{
    public class BotSpawnPatch : ModulePatch
    {
        public static Type LocalGameType;
        public BotSpawnPatch()
        {
            LocalGameType = SIT.Tarkov.Core.PatchConstants.EftTypes.FirstOrDefault(x => x.FullName.StartsWith("EFT.BaseLocalGame`"));
            if (LocalGameType == null)
                Logger.LogInfo($"BotSpawnPatch:Type is NULL");
        }

        protected override MethodBase GetTargetMethod()
        {
            foreach(var m in PatchConstants.GetAllMethodsForType(LocalGameType))
            {
                Logger.LogInfo(m.Name);
            }

            return PatchConstants.GetAllMethodsForType(LocalGameType)
                .Single(x => x.Name == "method_8");
        }

        [PatchPrefix]
        //public static async void Patch(object __instance, Task<EFT.LocalPlayer> __result, Profile profile, Vector3 position)
        public static async Task Patch(object __instance, Task<EFT.LocalPlayer> __result, Profile profile, Vector3 position)
        {
            await __result.ContinueWith((x) =>
            {
                var p = x.Result;
                Logger.LogInfo($"BotSpawnPatch:PatchPostfix:{p.GetType()}");

            });
            //if (profile.Info.Settings.Role == WildSpawnType.pmcBot)
            //    profile.Info.Side = EPlayerSide.Usec;

        }
    }
}
