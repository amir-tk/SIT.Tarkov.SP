using EFT;
using SIT.Tarkov.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/***
 * Full Credit for this patch goes to SPT-Aki team
 * Original Source is found here - https://dev.sp-tarkov.com/SPT-AKI/Modules
 * Paulov. Made changes to have better reflection, less hardcoding and add that AI should always attack players
 */

/// <summary>
/// 
/// </summary>
namespace SIT.Tarkov.SP.Raid.Aki
{
    public class IsEnemyPatch : ModulePatch
    {
        private static Type _targetType;
        private static FieldInfo _sideField;
        private static FieldInfo _enemiesField;

        public IsEnemyPatch()
        {
            _targetType = PatchConstants.EftTypes.Single(IsTargetType);
            _sideField = _targetType.GetField("Side");
            _enemiesField = _targetType.GetField("Enemies");
        }

        private bool IsTargetType(Type type)
        {
            if (type.GetMethod("AddEnemy") != null && type.GetMethod("AddEnemyGroupIfAllowed") != null)
            {
                Logger.LogInfo($"IsEnemyPatch: {type.FullName}");
                return true;
            }

            return false;
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("IsEnemy");
        }

        private static bool DumpedProperties;

        /// <summary>
        /// IsEnemy()
        /// Goal: if bot not found in enemy dictionary, we manually choose if they're an enemy or friend
        /// Check enemy cache list first, if not found, choose a value
        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result, object __instance, object requester)
        {
            var otherPerson = requester;
            //Logger.LogInfo($"IsEnemyPatch:PatchPrefix:__instance:Type: {__instance.GetType()}");
            //Logger.LogInfo($"IsEnemyPatch:PatchPrefix:requester:Type: {requester.GetType()}");

            WildSpawnType myWildSpawnType = (WildSpawnType)__instance.GetType().GetProperty("InitialBotType", BindingFlags.Public | BindingFlags.Instance).GetValue(__instance);
            var side = (EPlayerSide)_sideField.GetValue(__instance);
            var enemies = _enemiesField.GetValue(__instance);
            if (!DumpedProperties)
            {
                DumpedProperties = true;
                //foreach (var f in requester.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                //{
                //    Logger.LogInfo($"IsEnemyPatch:PropTests:{f}");
                //}
            }

            WildSpawnType otherPersonWildSpawnType = WildSpawnType.pmcBot;
            var requesterSideObj = requester.GetType().GetProperty("Side", BindingFlags.Public | BindingFlags.Instance).GetValue(requester);
            var requesterSide = (EPlayerSide)requesterSideObj;
            var requesterIsAI = (bool)requester.GetType().GetProperty("IsAI", BindingFlags.Public | BindingFlags.Instance).GetValue(requester);
            if (requesterIsAI)
            {
                var requesterProfile = (EFT.Profile)requester.GetType().GetProperty("Profile", BindingFlags.Public | BindingFlags.Instance).GetValue(requester);
                if (requesterProfile != null)
                {
                    if(requesterProfile.Info != null && requesterProfile.Info.Settings != null)
                    {
                        otherPersonWildSpawnType = requesterProfile.Info.Settings.Role;
                    }
                    //var requesterProfileInfo = PatchConstants.GetFieldOrPropertyFromInstance<object>(requesterProfile, "Info", false);
                    //if (requesterProfileInfo != null)
                    //{
                    //    var requesterProfileInfoSettings = PatchConstants.GetFieldOrPropertyFromInstance<object>(requesterProfileInfo, "Settings", false);
                    //    if (requesterProfileInfoSettings != null)
                    //        otherPersonWildSpawnType = (WildSpawnType)PatchConstants.GetFieldFromType(requesterProfileInfoSettings.GetType(), "Role").GetValue(requesterProfileInfoSettings);
                    //}
                }
            }

            // If PMC Bot / Player / A different Side , then KILL
            var result = side != requesterSide 
                || (!requesterIsAI) 
                || otherPersonWildSpawnType == WildSpawnType.pmcBot
                || myWildSpawnType != otherPersonWildSpawnType; // default side that doesn't equal mine OR is Player = kill
            //var result = side != requesterSide; // default side that doesn't equal mine OR is Player = kill

            __result = result;

            return false;
        }
    }
}
