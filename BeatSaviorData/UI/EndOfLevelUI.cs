using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaviorData.Trackers;
using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaviorData
{
	public class EndOfLevelUI : BSMLResourceViewController
	{
        public SongData data;

        [UIComponent("note")]
        private TextMeshProUGUI note;
        [UIComponent("FC")]
        private TextMeshProUGUI FC;
        [UIComponent("percent")]
        private TextMeshProUGUI percent;
        [UIComponent("title")]
        private TextMeshProUGUI title;
        [UIComponent("difficulty")]
        private TextMeshProUGUI difficulty;
        [UIComponent("combo")]
        private TextMeshProUGUI combo;
        [UIComponent("miss")]
        private TextMeshProUGUI miss;
        [UIComponent("pauses")]
        private TextMeshProUGUI pauses;
        [UIComponent("leftLabel")]
        private TextMeshProUGUI leftLabel;
        [UIComponent("leftAverage")]
        private TextMeshProUGUI leftAverage;
        [UIComponent("leftCut")]
        private TextMeshProUGUI leftCut;
        [UIComponent("leftDistance")]
        private TextMeshProUGUI leftDistance;
        [UIComponent("leftSpeed")]
        private TextMeshProUGUI leftSpeed;
        [UIComponent("rightLabel")]
        private TextMeshProUGUI rightLabel;
        [UIComponent("rightAverage")]
        private TextMeshProUGUI rightAverage;
        [UIComponent("rightCut")]
        private TextMeshProUGUI rightCut;
        [UIComponent("rightDistance")]
        private TextMeshProUGUI rightDistance;
        [UIComponent("rightSpeed")]
        private TextMeshProUGUI rightSpeed;

        private Color SS = new Color32(0x00, 0xF0, 0xFF, 0xFF), S = new Color32(0xFF, 0xFF, 0xFF, 0xFF), A = new Color32(0x00, 0xFF, 0x00, 0xFF), B = new Color32(0xFF, 0xFF, 0x00, 0xFF), C = new Color32(0xFF, 0xA7, 0x00, 0xFF), DorE = new Color32(0xFF, 0x00, 0x00, 0xFF);

        public override string ResourceName => "BeatSaviorData.UI.Views.EndOfLevelView.bsml";
	
        [UIAction("#post-parse")]
        public void SetDataToUI()
        {
            PlayerData playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
            ColorScheme colors = playerData.colorSchemesSettings.GetSelectedColorScheme();

            AccuracyTracker at = (data.trackers["accuracyTracker"] as AccuracyTracker);
            HitTracker ht = (data.trackers["hitTracker"] as HitTracker);
            WinTracker wt = (data.trackers["winTracker"] as WinTracker);
            ScoreTracker st = (data.trackers["scoreTracker"] as ScoreTracker);
            DistanceTracker dt = (data.trackers["distanceTracker"] as DistanceTracker);

            note.text = wt.rank;
            note.color = SetColorBasedOnRank(note.text);

            FC.gameObject.SetActive(ht.miss == 0);

            percent.text = (st.modifiedRatio * 100).ToString("F") + " %";

            title.text = data.songName;
            title.enableAutoSizing = true;
            title.fontSizeMax = 11;
            title.fontSizeMin = 5;

            difficulty.text = FormatSongDifficulty(data.songDifficulty);

            combo.text = "Combo : " + ht.maxCombo;
            miss.text = "Misses : " + ht.miss;
            pauses.text = "Pauses : " + wt.nbOfPause;

            leftLabel.color = colors.saberAColor;
            leftAverage.text = at.accLeft.ToString("F");
            leftCut.text = at.leftAverageCut[0].ToString("0.0") + '/' + at.leftAverageCut[1].ToString("0.0") + '/' + at.leftAverageCut[2].ToString("0.0");
            leftDistance.text = dt.leftHand.ToString("F") + " m";
            leftSpeed.text = (at.leftSpeed * 3.6f).ToString("F") + " Km/h";

            rightLabel.color = colors.saberBColor;
            rightAverage.text = at.accRight.ToString("F");
            rightCut.text = at.rightAverageCut[0].ToString("0.0") + '/' + at.rightAverageCut[1].ToString("0.0") + '/' + at.rightAverageCut[2].ToString("0.0");
            rightDistance.text = dt.rightHand.ToString("F") + " m";
            rightSpeed.text = (at.rightSpeed * 3.6f).ToString("F") + " Km/h";
        }

        private Color32 SetColorBasedOnRank(string rank)
        {
            switch(rank)
            {
                case "SSS": case "SS": return SS;
                case "S": default: return S;
                case "A": return A;
                case "B": return B;
                case "C": return C;
                case "D": case "E": return DorE;
            }
        }

        private string FormatSongDifficulty(string diffName)
        {
            switch(diffName)
            {
                case "easy": return "Easy";
                case "normal": return "Normal";
                case "hard": return "Hard";
                case "expert": return "Expert";
                default: case "expertplus": return "Expert+";
            }
        }
    }
}
