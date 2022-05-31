﻿using EFT;
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

        /// <summary>
        /// IsEnemy()
        /// Goal: if bot not found in enemy dictionary, we manually choose if they're an enemy or friend
        /// Check enemy cache list first, if not found, choose a value
        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result, object __instance, object requester)
        {
            //Logger.LogInfo($"IsEnemyPatch:PatchPrefix:__instance:Type: {__instance.GetType()}");
            //Logger.LogInfo($"IsEnemyPatch:PatchPrefix:requester:Type: {requester.GetType()}");

            var side = (EPlayerSide)_sideField.GetValue(__instance);
            var enemies = _enemiesField.GetValue(__instance);
            //foreach(var f in requester.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            //{
            //    Logger.LogInfo($"{f}");
            //}
            var requesterSideObj = requester.GetType().GetProperty("Side", BindingFlags.Public | BindingFlags.Instance).GetValue(requester);
            var requesterSide = (EPlayerSide)requesterSideObj;
            var requesterIsAI = (bool)requester.GetType().GetProperty("IsAI", BindingFlags.Public | BindingFlags.Instance).GetValue(requester);

            var result = side != requesterSide || (!requesterIsAI); // default side that doesn't equal mine OR is Player = kill

            __result = result;

            return false;
        }
    }
}