using EFT;
using SIT.Tarkov.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace SIT.Tarkov.SP
{
    public class ReplaceInPlayer : ModulePatch
    {
        private static string _playerAccountId;

        public ReplaceInPlayer()  { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        static async void PatchPostfix(Player __instance, Task __result)
        {
            if (_playerAccountId == null)
            {
                var backendSession = ClientAccesor.GetClientApp().GetClientBackEndSession();
                var profile = backendSession.Profile;
                _playerAccountId = profile.AccountId;
            }

            if (__instance.Profile.AccountId != _playerAccountId)
            {
                return;
            }

            await __result;

            //var listener = Utility.Progression.HealthListener.Instance;
            //listener.Init(__instance.HealthController, true);
        }
    }
}
