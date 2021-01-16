using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaviorData.Trackers;
using BS_Utils.Utilities;
using HMUI;
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

        #pragma warning disable 0649    // Disables the "never assigned" warning
        [UIObject("titleCard")]
        private readonly GameObject titleCard;
        [UIObject("songCover")]
        private readonly GameObject songCover;
        [UIComponent("title")]
        private readonly TextMeshProUGUI title;
        [UIComponent("artist")]
        private readonly TextMeshProUGUI artist;
        [UIComponent("mapper")]
        private readonly TextMeshProUGUI mapper;
        [UIComponent("difficulty")]
        private readonly TextMeshProUGUI difficulty;

        [UIObject("upperBand")]
        private readonly GameObject upperBand;
        [UIObject("lowerBand")]
        private readonly GameObject lowerBand;
        [UIComponent("rank")]
        private readonly TextMeshProUGUI rank;
        [UIComponent("percent")]
        private readonly TextMeshProUGUI percent;
        [UIComponent("combo")]
        private readonly TextMeshProUGUI combo;
        [UIComponent("missLabel")]
        private readonly TextMeshProUGUI missLabel;
        [UIComponent("miss")]
        private readonly TextMeshProUGUI miss;
        [UIComponent("pauses")]
        private readonly TextMeshProUGUI pauses;

        [UIObject("leftCircle")]
        private readonly GameObject leftCircle;
        [UIComponent("leftAverage")]
        private readonly TextMeshProUGUI leftAverage;
        [UIComponent("leftBeforeCut")]
        private readonly TextMeshProUGUI leftBeforeCut;
        [UIComponent("leftAccuracy")]
        private readonly TextMeshProUGUI leftAccuracy;
        [UIComponent("leftAfterCut")]
        private readonly TextMeshProUGUI leftAfterCut;
        [UIComponent("leftDistance")]
        private readonly TextMeshProUGUI leftDistance;
        [UIComponent("leftSpeed")]
        private readonly TextMeshProUGUI leftSpeed;
        [UIComponent("leftBeforeSwing")]
        private readonly TextMeshProUGUI leftBeforeSwing;
        [UIComponent("leftAfterSwing")]
        private readonly TextMeshProUGUI leftAfterSwing;

        [UIObject("middleBand")]
        private readonly GameObject middleBand;

        [UIObject("rightCircle")]
        private readonly GameObject rightCircle;
        [UIComponent("rightAverage")]
        private readonly TextMeshProUGUI rightAverage;
        [UIComponent("rightBeforeCut")]
        private readonly TextMeshProUGUI rightBeforeCut;
        [UIComponent("rightAccuracy")]
        private readonly TextMeshProUGUI rightAccuracy;
        [UIComponent("rightAfterCut")]
        private readonly TextMeshProUGUI rightAfterCut;
        [UIComponent("rightDistance")]
        private readonly TextMeshProUGUI rightDistance;
        [UIComponent("rightSpeed")]
        private readonly TextMeshProUGUI rightSpeed;
        [UIComponent("rightBeforeSwing")]
        private readonly TextMeshProUGUI rightBeforeSwing;
        [UIComponent("rightAfterSwing")]
        private readonly TextMeshProUGUI rightAfterSwing;
        #pragma warning restore 0649

        private readonly Color SS = new Color32(0x00, 0xF0, 0xFF, 0xFF), S = new Color32(0xFF, 0xFF, 0xFF, 0xFF), A = new Color32(0x00, 0xFF, 0x00, 0xFF), B = new Color32(0xFF, 0xFF, 0x00, 0xFF), C = new Color32(0xFF, 0xA7, 0x00, 0xFF), DorE = new Color32(0xFF, 0x00, 0x00, 0xFF);
        private readonly Color expertPlus = new Color32(0x8f, 0x48, 0xdb, 0xFF), expert = new Color32(0xbf, 0x2a, 0x42, 0xFF), hard = new Color32(0xFF, 0xa5, 0x00, 0xFF), normal = new Color32(0x59, 0xb0, 0xf4, 0xFF), easy = new Color32(0x3c, 0xb3, 0x71, 0xFF);
        private readonly Color32 goldColor = new Color32(237, 201, 103, 255);
        private readonly List<(float, float)> curve = new List<(float, float)>() { (0, 0), (40, 8), (50, 15), (69, 25), (75, 42.5f), (82, 56), (84.5f, 63), (86, 72), (88, 76.6f), (90, 81.5f), (91, 85), (92, 88.5f), (93, 92), (94, 97.4f), (95, 103.6f), (100, 110) };

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

            ImageView img;
            bool fc = (ht.miss == 0 && ht.bombHit == 0 && ht.nbOfWallHit == 0);

            #region TitleCard
            // TitleCard
            titleCard.GetComponent<ContentSizeFitter>().enabled = false;
            titleCard.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            // SongCover
            Sprite s = data.GetGCSSD().difficultyBeatmap.level.GetCoverImageAsync(new System.Threading.CancellationToken()).Result;
            img = songCover.AddComponent<ImageView>();
            img.sprite = s;
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            // Title
            title.text = data.songName;
            title.enableAutoSizing = true;
            title.fontSizeMax = 15;
            title.fontSizeMin = 1;

            // Artist
            artist.text = data.songArtist;
            artist.enableAutoSizing = true;
            artist.fontSizeMax = 15;
            artist.fontSizeMin = 1;

            // Mapper
            mapper.text = data.songMapper;
            mapper.enableAutoSizing = true;
            mapper.fontSizeMax = 15;
            mapper.fontSizeMin = 1;

            // Difficulty
            difficulty.text = FormatSongDifficulty(data.songDifficulty);
            difficulty.color = SetColorBasedOnDifficulty(data.songDifficulty);
            difficulty.enableAutoSizing = true;
            difficulty.fontSizeMax = 15;
            difficulty.fontSizeMin = 1;
            #endregion

            #region StatBand
            // UpperBand
            img = upperBand.AddComponent<ImageView>();
            img.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            if (fc) img.color = goldColor;

            // Rank
            rank.text = wt.rank;
            rank.color = SetColorBasedOnRank(rank.text);

            // Score
            percent.text = (st.modifiedRatio * 100).ToString("F") + " %";

            // Combo
            combo.text = ht.maxCombo.ToString();

            // Misses
            miss.text = fc ? "FC" : ht.miss.ToString();
            if (fc) {
                miss.color = goldColor;
                missLabel.color = goldColor;
            }

            // Pauses
            pauses.text = wt.nbOfPause.ToString();

            // LowerBand
            img = lowerBand.AddComponent<ImageView>();
            img.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            if (fc) img.color = goldColor;
            #endregion

            #region LeftSaberStats

            // LeftCircle
            img = leftCircle.AddComponent<ImageView>();
            img.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "Circle").First();
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            img.color = colors.saberAColor;
            img.preserveAspect = true;
            img.transform.localScale *= 1.55f;
            img.type = Image.Type.Filled;
            img.fillOrigin = (int)Image.Origin360.Top;
            img.fillAmount = 0;
            img.StartCoroutine(AnimateCircle(img, GetCircleFillRatio(at.accLeft), 1.5f));

            // LeftAverageCut
            leftAverage.text = at.accLeft.ToString("0.##");
            leftAverage.color = colors.saberAColor;
            leftAverage.enableAutoSizing = true;
            leftAverage.fontSizeMin = 1;
            leftAverage.fontSizeMax = 12;

            // LeftBeforeCut
            leftBeforeCut.text = at.leftAverageCut[0].ToString("0.#");
            leftBeforeCut.color = colors.saberAColor;
            leftBeforeCut.overflowMode = TextOverflowModes.Overflow;

            // LeftAccuracy
            leftAccuracy.text = at.leftAverageCut[1].ToString("0.#");
            leftAccuracy.color = colors.saberAColor;
            leftAccuracy.overflowMode = TextOverflowModes.Overflow;

            // LeftAfterCut
            leftAfterCut.text = at.leftAverageCut[2].ToString("0.#");
            leftAfterCut.color = colors.saberAColor;
            leftAfterCut.overflowMode = TextOverflowModes.Overflow;

            // LeftDistance
            leftDistance.text = dt.leftHand.ToString("0.##") + " m";
            leftDistance.color = colors.saberAColor;
            leftDistance.overflowMode = TextOverflowModes.Overflow;

            // LeftSpeed
            leftSpeed.text = (at.leftSpeed * 3.6f).ToString("0.##") + " Km/h";
            leftSpeed.color = colors.saberAColor;
            leftSpeed.overflowMode = TextOverflowModes.Overflow;

            // LeftBeforeSwing
            leftBeforeSwing.text = (at.leftPreswing * 100).ToString("0.##") + " %";
            leftBeforeSwing.color = colors.saberAColor;
            leftBeforeSwing.overflowMode = TextOverflowModes.Overflow;

            // LeftAfterSwing
            leftAfterSwing.text = (at.leftPostswing * 100).ToString("0.##") + " %";
            leftAfterSwing.color = colors.saberAColor;
            leftAfterSwing.overflowMode = TextOverflowModes.Overflow;

			#endregion

			#region MiddleSeparator

			// MiddleBand
			img = middleBand.AddComponent<ImageView>();
            img.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            img.color = new Color32(255, 255, 255, 100);

            #endregion

            #region RightSaberStats

            // rightCircle
            img = rightCircle.AddComponent<ImageView>();
            img.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "Circle").First();
            img.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            img.color = colors.saberBColor;
            img.preserveAspect = true;
            img.transform.localScale *= 1.55f;
            img.type = Image.Type.Filled;
            img.fillOrigin = (int)Image.Origin360.Top;
            img.fillAmount = 0;
            img.StartCoroutine(AnimateCircle(img, GetCircleFillRatio(at.accRight), 1.5f));

            // rightAverageCut
            rightAverage.text = at.accRight.ToString("0.##");
            rightAverage.color = colors.saberBColor;
            rightAverage.enableAutoSizing = true;
            rightAverage.fontSizeMin = 1;
            rightAverage.fontSizeMax = 12;

            // rightBeforeCut
            rightBeforeCut.text = at.rightAverageCut[0].ToString("0.#");
            rightBeforeCut.color = colors.saberBColor;
            rightBeforeCut.overflowMode = TextOverflowModes.Overflow;

            // rightAccuracy
            rightAccuracy.text = at.rightAverageCut[1].ToString("0.#");
            rightAccuracy.color = colors.saberBColor;
            rightAccuracy.overflowMode = TextOverflowModes.Overflow;

            // rightAfterCut
            rightAfterCut.text = at.rightAverageCut[2].ToString("0.#");
            rightAfterCut.color = colors.saberBColor;
            rightAfterCut.overflowMode = TextOverflowModes.Overflow;

            // rightDistance
            rightDistance.text = dt.rightHand.ToString("0.##") + " m";
            rightDistance.color = colors.saberBColor;
            rightDistance.overflowMode = TextOverflowModes.Overflow;

            // rightSpeed
            rightSpeed.text = (at.rightSpeed * 3.6f).ToString("0.##") + " Km/h";
            rightSpeed.color = colors.saberBColor;
            rightSpeed.overflowMode = TextOverflowModes.Overflow;

            // rightBeforeSwing
            rightBeforeSwing.text = (at.rightPreswing * 100).ToString("0.##") + " %";
            rightBeforeSwing.color = colors.saberBColor;
            rightBeforeSwing.overflowMode = TextOverflowModes.Overflow;

            // rightAfterSwing
            rightAfterSwing.text = (at.rightPostswing * 100).ToString("0.##") + " %";
            rightAfterSwing.color = colors.saberBColor;
            rightAfterSwing.overflowMode = TextOverflowModes.Overflow;

            #endregion
        }

        private Color32 SetColorBasedOnRank(string rank)
        {
            switch (rank)
            {
                case "SSS": case "SS": return SS;
                case "S": default: return S;
                case "A": return A;
                case "B": return B;
                case "C": return C;
                case "D": case "E": return DorE;
            }
        }

        private Color32 SetColorBasedOnDifficulty(string diffName)
        {
            switch (diffName)
            {
                case "easy": return easy;
                case "normal": return normal;
                case "hard": return hard;
                case "expert": return expert;
                default: case "expertplus": return expertPlus;
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

        private float GetCircleFillRatio(float accuracy)
        {
            float maxPPPercent = 110;    // The max number of pp (in %) (yes it's not 100%, look at the curve)
            float accRatio = (accuracy / 115) * 100, curveRatio;
            float max, min, maxValue, minValue;

            for(int i = 0; i < curve.Count; i++)
            {
                if(curve[i].Item1 >= accRatio)
                {
                    max = curve[i].Item1; maxValue = curve[i].Item2;
                    min = curve[i - 1].Item1; minValue = curve[i - 1].Item2;
                    curveRatio = (accRatio - min) / (max - min);

                    return (minValue + (maxValue - minValue) * curveRatio) / maxPPPercent;
                }
            }

            return 1;
        }

        private IEnumerator AnimateCircle(Image img, float final, float totalTime)
        {
            float timeLeft = totalTime;

            yield return new WaitForSeconds(1.5f);

            while(timeLeft > Time.deltaTime)
            {
                timeLeft -= Time.deltaTime;
                img.fillAmount = Mathf.SmoothStep(0, final, 1 - timeLeft / totalTime);
                yield return null;
            }

            img.fillAmount = final;
        }
    }
}
