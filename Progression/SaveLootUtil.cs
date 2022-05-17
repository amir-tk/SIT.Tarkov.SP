using SIT.Tarkov.Core;

namespace SIT.Tarkov.SP
{
	public class SaveLootUtil
	{
		public static void SaveProfileProgress(string backendUrl, string session, EFT.ExitStatus exitStatus, EFT.Profile profileData, PlayerHealth currentHealth, bool isPlayerScav)
		{
			SaveProfileRequest request = new SaveProfileRequest
			{
				exit = exitStatus.ToString().ToLower(),
				profile = profileData,
				health = currentHealth,
				isPlayerScav = isPlayerScav
			};

			// ToJson() uses an internal converter which prevents loops and do other internal things
			new Request(session, backendUrl).PostJson("/raid/profile/save", request.ToJson());
		}

		internal class SaveProfileRequest
		{
			public string exit = "left";
			public EFT.Profile profile;
			public bool isPlayerScav;
			public PlayerHealth health;
		}
	}
}
