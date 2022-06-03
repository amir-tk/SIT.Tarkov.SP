using EFT;
using SIT.Tarkov.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Tarkov.SP.Raid
{
    internal class CheckAndAddEnemyPatch : ModulePatch
    {
        private static Type _targetType;
        private static FieldInfo _sideField;
        private static FieldInfo _enemiesField;

        public CheckAndAddEnemyPatch()
        {
            _targetType = PatchConstants.EftTypes.Single(IsTargetType);
            _sideField = _targetType.GetField("Side");
            _enemiesField = _targetType.GetField("Enemies");
        }

        private bool IsTargetType(Type type)
        {
            if (PatchConstants.GetAllMethodsForType(type).Any(x => x.Name == "CheckAndAddEnemy"))
            {
                Logger.LogInfo($"CheckAndAddEnemyPatch: {type.FullName}");
                return true;
            }

            return false;
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatchConstants.GetAllMethodsForType(_targetType).Single(x => x.Name == "CheckAndAddEnemy");
        }

        /// </summary>
        [PatchPrefix]
        private static bool PatchPrefix(object __instance, object player, bool ignoreAI)
        {
            //Logger.LogInfo($"CheckAndAddEnemyPatch:PatchPrefix:__instance:Type: {__instance.GetType()}");
            ignoreAI = false;
            return true;
        }
    }
}
