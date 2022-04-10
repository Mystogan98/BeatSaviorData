using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System.Net.Http;
using TMPro;
using UnityEngine;

namespace BeatSaviorData.UI
{
    internal class SpeedonsUI: BSMLResourceViewController
    {
        public override string ResourceName => "BeatSaviorData.UI.Views.SpeedonsView.bsml";

        #pragma warning disable 0649    // Disables the "never assigned" warning
        [UIComponent("donations")]
        private readonly TextMeshProUGUI donationsText;
        #pragma warning restore 0649

        private bool postParseDone = false;

        [UIAction("#post-parse")]
        public void SetDataToUI()
        {
            donationsText.enableAutoSizing = true;
            donationsText.fontSizeMin = 1;
            donationsText.fontSizeMax = 15;
            donationsText.color = new Color32(0, 88, 90, 255);

            postParseDone = true;
            Refresh();
        }

        public void Refresh()
        {
            if (!postParseDone)
                return;

            // Get data here
            HttpResponseMessage res = HTTPManager.client.GetAsync("https://mystogan.omedan.me/leaderboards/API/speedons").Result;
            string donations = res.Content.ReadAsStringAsync().Result;

            if (donations == "0")
                donationsText.text = "Begins on the 15th April !";
            else
                donationsText.text = donations + " €";
        }
    }
}
