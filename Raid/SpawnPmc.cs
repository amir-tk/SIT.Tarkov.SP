using EFT;
using HarmonyLib;
using SIT.Tarkov.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SinglePlayerMod.Patches.Raid
{
    public class SpawnPmc : ModulePatch
    {
        private static Type targetInterface;
        private static Type targetType;

        private static AccessTools.FieldRef<object, WildSpawnType> wildSpawnTypeField;
        private static AccessTools.FieldRef<object, BotDifficulty> botDifficultyField;

        // gclass364 (the one containing method_1

        public SpawnPmc() 
        {
            targetInterface = PatchConstants.EftTypes.Single(IsTargetInterface);
            targetType = PatchConstants.EftTypes.Single(IsTargetType);
            wildSpawnTypeField = AccessTools.FieldRefAccess<WildSpawnType>(targetType, "wildSpawnType_0"); // Type
            botDifficultyField = AccessTools.FieldRefAccess<BotDifficulty>(targetType, "botDifficulty_0"); // LoadBotDifficultyFromServer
        }

        private static bool IsTargetInterface(Type type) =>
            type.IsInterface &&
            type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) != null;

        private bool IsTargetType(Type type)
        {
            //if (!targetInterface.IsAssignableFrom(type))
            //    return false;

            //if (!type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //         .Any(m => m.Name == "SameSide"))
            //    return false;

            //if (!type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            //         .Any(f => f.FieldType == typeof(WildSpawnType) ||
            //                   f.FieldType == typeof(BotDifficulty)))
            //    return false;


            if (targetInterface.IsAssignableFrom(type)
            &&
            type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                     .Any(m => m.Name == "SameSide")
            &&
            type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                     .Any(f => f.FieldType == typeof(WildSpawnType) ||
                               f.FieldType == typeof(BotDifficulty))
                               )
                return true;

            //return true;
            return false;
        }

        protected override MethodBase GetTargetMethod()
        {
            return targetType.GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        [PatchPrefix]

        public static bool PatchPrefix(object __instance, ref bool __result, Profile x)
        {
            var botType = wildSpawnTypeField(__instance);
            var botDifficulty = botDifficultyField(__instance);

            //__result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;

            EPlayerSide side = x.Info.Side;
            //EPlayerSide? side2 = this.Side;
            __result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;
            x.Info.Side = x.Info.Side == EPlayerSide.Savage ? EPlayerSide.Usec : side;


            //UnityEngine.Debug.LogError("SpawnPmc::Patch::Patched");

            return false;
        }
    }
}
