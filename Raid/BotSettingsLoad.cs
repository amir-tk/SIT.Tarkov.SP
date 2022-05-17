using HarmonyLib;
using SIT.Tarkov.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SIT.B.Tarkov.SP
{
    public class BotSettingsLoad : ModulePatch
    {

        protected override MethodBase GetTargetMethod()
        {
            var getBotDifficultyHandler = typeof(EFT.MainApplication).Assembly.GetTypes().Where(type => type.Name.StartsWith("GClass") && type.GetMethod("CheckOnExcude", BindingFlags.Public | BindingFlags.Static) != null).First();
            if (getBotDifficultyHandler == null)
                return null;
            return getBotDifficultyHandler.GetMethod("Load", BindingFlags.Public | BindingFlags.Static);
        }

        [PatchTranspiler]
        static IEnumerable<CodeInstruction> PatchTranspile(IEnumerable<CodeInstruction> instructions)
        {

            var codes = new List<CodeInstruction>(instructions);
            // that should be the fastest way cause its at index 3 and we need to remov e3 instructions from there
            for (var i = 0; i < 14; i++)
                codes.RemoveAt(6);

            if (codes.Count != 20)
            {
                Debug.LogError($"Patch Failed!! strange number of opcodes {codes.Count} [originalCode count is: {instructions.ToList().Count}]");
                return instructions;
            }
            return codes.AsEnumerable();
        }
    }
}
