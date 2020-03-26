using BeatSaberMarkupLanguage.Attributes;
using BS_Utils.Utilities;

namespace BeatSaviorData
{
	class SettingsMenu : PersistentSingleton<SettingsMenu>
	{
		private static readonly Config config = new Config("BeatSaviorData");

		[UIValue("DisablePass")]
		public bool DisablePass
		{
			get => config.GetBool("BeatSaviorData", "DisablePass", false, true);
			set => config.SetBool("BeatSaviorData", "DisablePass", value);
		}

		[UIValue("DisableFail")]
		public bool DisableFails
		{
			get => config.GetBool("BeatSaviorData", "DisableFails", false, true);
			set => config.SetBool("BeatSaviorData", "DisableFails", value);
		}
	}
}
