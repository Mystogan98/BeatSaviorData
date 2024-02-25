using BeatSaberMarkupLanguage.Attributes;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatSaviorData
{
    internal class SettingsMenu
    {
        public static SettingsMenu instance { get; set; }

		[UIValue("DisablePass")]
        public bool DisablePass { get; set; } = false;
       
		[UIValue("DisableFail")]
		public bool DisableFails { get; set; } = false;

		[UIValue("HideNbOfPauses")]
		public bool HideNbOfPauses { get; set; } = false;

		[UIValue("EnableUI")]
		public bool EnableUI { get; set; } = true;

		[UIValue("DisableGraphPanel")]
		public bool DisableGraphPanel { get; set; } = false;

        [UIValue("DisableBeatSaviorUpload")]
		public bool DisableBeatSaviorUpload { get; set; } = false;

        public bool EnableCustomUrlUpload { get; set; } = false;

        public string CustomUploadUrl { get; set; } = "";

        public int MaxStatsFiles { get; set; } = 30;

    }
}
