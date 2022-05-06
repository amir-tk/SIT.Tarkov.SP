using SIT.Tarkov.Core;
using System.Linq;
using System.Reflection;

namespace SinglePlayerMod.Patches.Raid
{
    /// <summary>
    /// This patch enables the ability to change max cap of bots in match from 20 to whatever you desire, if not set or error occured it will be set to 20
    /// </summary>
    public class MaxBotCap : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var methodName = "SetSettings";
            return PatchConstants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null && IsTargetMethod(x.GetMethod(methodName, flags)))
                .GetMethod(methodName, flags);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return parameters.Length == 3
                && parameters[0].Name == "maxCount"
                && parameters[1].Name == "botPresets"
                && parameters[2].Name == "botScatterings";
        }

        [PatchPrefix]
        public static bool PatchPrefix(ref int maxCount)
        {
            maxCount = 1;
            return true;
        }
    }
}