//using MainMenuController = GClass1504; // SelectedDateTime
//using IHealthController = GInterface195; // CarryingWeightAbsoluteModifier
using System.Reflection;
using SIT.Tarkov.Core;
using System.Linq;

namespace SIT.Tarkov.SP
{
    public class ReplaceInMainMenuController : ModulePatch
    {
        static ReplaceInMainMenuController()
        {
            //_ = nameof(IHealthController.HydrationChangedEvent);
            //_ = nameof(MainMenuController.HealthController);
        }

        public ReplaceInMainMenuController() { }

        protected override MethodBase GetTargetMethod()
        {
            var mmc = SIT.Tarkov.Core.PatchConstants.EftTypes.Single(x =>
                (SIT.Tarkov.Core.PatchConstants.GetFieldFromType(x, "HealthController") != null
                || SIT.Tarkov.Core.PatchConstants.GetPropertyFromType(x, "HealthController") != null)
                &&
                 (SIT.Tarkov.Core.PatchConstants.GetFieldFromType(x, "SelectedDateTime") != null
                || SIT.Tarkov.Core.PatchConstants.GetPropertyFromType(x, "SelectedDateTime") != null)
                );
            var m = mmc.GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
            return m;

        }

        [PatchPostfix]
        public static void PatchPostfix(object __instance)
        {
            var healthController = SIT.Tarkov.Core.PatchConstants.GetFieldOrPropertyFromInstance<object>(__instance, "HealthController");// __instance.HealthController;
            if (healthController != null)
            {
                Logger.LogInfo("ReplaceInMainMenuController.PatchPostfix.HealthController found!");
                var listener = HealthListener.Instance;
                listener.Init(healthController, false);
            }
            else
            {
                Logger.LogInfo("ReplaceInMainMenuController.PatchPostfix.HealthController not found!");
            }
        }
    }
}
