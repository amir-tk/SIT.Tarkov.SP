using EFT;
using SIT.Tarkov.Core;
using SIT.B.Tarkov.SP;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SIT.B.Tarkov.SP
{
    class LoadBotDifficultyFromServer : ModulePatch
    {
        public LoadBotDifficultyFromServer()
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var getBotDifficultyHandler = typeof(EFT.MainApplication).Assembly.GetTypes().Where(type => type.Name.StartsWith("GClass") && type.GetMethod("CheckOnExcude", BindingFlags.Public | BindingFlags.Static) != null).First();
            if (getBotDifficultyHandler == null)
                return null;
            return getBotDifficultyHandler.GetMethod("LoadDifficultyStringInternal", BindingFlags.Public | BindingFlags.Static);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = Request(role, botDifficulty);
            return string.IsNullOrWhiteSpace(__result);
        }

        private static string Request(WildSpawnType role, BotDifficulty botDifficulty)
        {
            var json = new Request(null, SIT.Tarkov.Core.PatchConstants.GetBackendUrl()).GetJson("/singleplayer/settings/bot/difficulty/" + role.ToString() + "/" + botDifficulty.ToString());

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("[JET]: Received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data is NULL, using fallback");
                return null;
            }

            //Debug.LogError("[JET]: Successfully received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data");
            return json;
        }
    }
}
