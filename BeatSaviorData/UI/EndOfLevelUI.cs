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
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaviorData
{
	public class EndOfLevelUI : BSMLResourceViewController
	{
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
        [UIComponent("rankLabel")]
        private readonly TextMeshProUGUI rankLabel;
        [UIComponent("rank")]
        private readonly TextMeshProUGUI rank;
        [UIComponent("percentLabel")]
        private readonly TextMeshProUGUI percentLabel;
        [UIComponent("percent")]
        private readonly TextMeshProUGUI percent;
        [UIComponent("comboLabel")]
        private readonly TextMeshProUGUI comboLabel;
        [UIComponent("combo")]
        private readonly TextMeshProUGUI combo;
        [UIComponent("missLabel")]
        private readonly TextMeshProUGUI missLabel;
        [UIComponent("miss")]
        private readonly TextMeshProUGUI miss;
        [UIComponent("pausesLabel")]
        private readonly TextMeshProUGUI pausesLabel;
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
        [UIComponent("leftTD")]
        private readonly TextMeshProUGUI leftTD;
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
        [UIComponent("rightTD")]
        private readonly TextMeshProUGUI rightTD;
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
        private bool postParseDone = false;
        private SongData tmpData;
        private ImageView songCoverImg, upperBandImg, lowerBandImg, leftCircleImg, rightCircleImg;

        private List<string> lyrics = new List<string>()
        {
            "Never", "gonna", "give", "you", "up",
            "Never", "gonna", "let", "you", "down",
        };

        [UIAction("#post-parse")]
        public void SetDataToUI()
        {
            PlayerData playerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
            ColorScheme colors = playerData.colorSchemesSettings.GetSelectedColorScheme();

            ImageView img;

            #region TitleCard
            // TitleCard
            titleCard.GetComponent<ContentSizeFitter>().enabled = false;
            titleCard.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            // SongCover
            songCoverImg = songCover.AddComponent<ImageView>();
            songCoverImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            // Title
            title.enableAutoSizing = true;
            title.fontSizeMax = 15;
            title.fontSizeMin = 1;

            // Artist
            artist.enableAutoSizing = true;
            artist.fontSizeMax = 15;
            artist.fontSizeMin = 1;

            // Mapper
            mapper.enableAutoSizing = true;
            mapper.fontSizeMax = 15;
            mapper.fontSizeMin = 1;

            // Difficulty
            difficulty.enableAutoSizing = true;
            difficulty.fontSizeMax = 15;
            difficulty.fontSizeMin = 1;
            #endregion

            #region StatBand
            // UpperBand
            upperBandImg = upperBand.AddComponent<ImageView>();
            upperBandImg.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            upperBandImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;

            // LowerBand
            lowerBandImg = lowerBand.AddComponent<ImageView>();
            lowerBandImg.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.WhitePixel;
            lowerBandImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            #endregion

            #region LeftSaberStats
            // LeftCircle
            leftCircleImg = leftCircle.AddComponent<ImageView>();
            leftCircleImg.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "Circle").First();
            leftCircleImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            leftCircleImg.color = colors.saberAColor;
            leftCircleImg.preserveAspect = true;
            leftCircleImg.transform.localScale *= 1.55f;
            leftCircleImg.type = Image.Type.Filled;
            leftCircleImg.fillOrigin = (int)Image.Origin360.Top;
            leftCircleImg.fillAmount = 0;

            // LeftAverageCut
            leftAverage.color = colors.saberAColor;
            leftAverage.enableAutoSizing = true;
            leftAverage.fontSizeMin = 1;
            leftAverage.fontSizeMax = 12;

            // LeftBeforeCut
            leftBeforeCut.color = colors.saberAColor;
            leftBeforeCut.overflowMode = TextOverflowModes.Overflow;

            // LeftAccuracy
            leftAccuracy.color = colors.saberAColor;
            leftAccuracy.overflowMode = TextOverflowModes.Overflow;

            // LeftAfterCut
            leftAfterCut.color = colors.saberAColor;
            leftAfterCut.overflowMode = TextOverflowModes.Overflow;

            // LeftDistance
            leftTD.color = colors.saberAColor;
            leftTD.overflowMode = TextOverflowModes.Overflow;

            // LeftSpeed
            leftSpeed.color = colors.saberAColor;
            leftSpeed.overflowMode = TextOverflowModes.Overflow;

            // LeftBeforeSwing
            leftBeforeSwing.color = colors.saberAColor;
            leftBeforeSwing.overflowMode = TextOverflowModes.Overflow;

            // LeftAfterSwing
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
            rightCircleImg = rightCircle.AddComponent<ImageView>();
            rightCircleImg.sprite = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "Circle").First();
            rightCircleImg.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            rightCircleImg.color = colors.saberBColor;
            rightCircleImg.preserveAspect = true;
            rightCircleImg.transform.localScale *= 1.55f;
            rightCircleImg.type = Image.Type.Filled;
            rightCircleImg.fillOrigin = (int)Image.Origin360.Top;
            rightCircleImg.fillAmount = 0;

            // rightAverageCut
            rightAverage.color = colors.saberBColor;
            rightAverage.enableAutoSizing = true;
            rightAverage.fontSizeMin = 1;
            rightAverage.fontSizeMax = 12;

            // rightBeforeCut
            rightBeforeCut.color = colors.saberBColor;
            rightBeforeCut.overflowMode = TextOverflowModes.Overflow;

            // rightAccuracy
            rightAccuracy.color = colors.saberBColor;
            rightAccuracy.overflowMode = TextOverflowModes.Overflow;

            // rightAfterCut
            rightAfterCut.color = colors.saberBColor;
            rightAfterCut.overflowMode = TextOverflowModes.Overflow;

            // rightDistance
            rightTD.color = colors.saberBColor;
            rightTD.overflowMode = TextOverflowModes.Overflow;

            // rightSpeed
            rightSpeed.color = colors.saberBColor;
            rightSpeed.overflowMode = TextOverflowModes.Overflow;

            // rightBeforeSwing
            rightBeforeSwing.color = colors.saberBColor;
            rightBeforeSwing.overflowMode = TextOverflowModes.Overflow;

            // rightAfterSwing
            rightAfterSwing.color = colors.saberBColor;
            rightAfterSwing.overflowMode = TextOverflowModes.Overflow;
            #endregion

            postParseDone = true;
            if(tmpData != null)
                Refresh(tmpData);
        }

        public void Refresh(SongData data)
        {
            if (!postParseDone) {
                tmpData = data;
                return;
            }

            AccuracyTracker at = (data.trackers["accuracyTracker"] as AccuracyTracker);
            HitTracker ht = (data.trackers["hitTracker"] as HitTracker);
            WinTracker wt = (data.trackers["winTracker"] as WinTracker);
            ScoreTracker st = (data.trackers["scoreTracker"] as ScoreTracker);
            //DistanceTracker dt = (data.trackers["distanceTracker"] as DistanceTracker);
            bool fc = (ht.miss == 0 && ht.bombHit == 0 && ht.nbOfWallHit == 0);
            int index = 0;

            #region TitleCard
            // SongCover
            Sprite s = data.GetGCSSD().difficultyBeatmap.level.GetCoverImageAsync(new System.Threading.CancellationToken()).Result;
            songCoverImg.sprite = s;

            // Title
            title.text = data.songName;

            // Artist
            artist.text = data.songArtist;

            // Mapper
            mapper.text = data.songMapper;

            // Difficulty
            difficulty.text = FormatSongDifficulty(data.songDifficulty);
            difficulty.color = SetColorBasedOnDifficulty(data.songDifficulty);
            #endregion

            #region StatBand
            // UpperBand
            if (fc) upperBandImg.color = goldColor;
            else upperBandImg.color = Color.white;

            // Rank
            if(!Plugin.fish && true != false && (true || !false) && 1+3 != 5 || 42 == 69)
                rank.text = wt.rank;
            else
            {
                rankLabel.text = lyrics[index];
                rank.text = lyrics[index + 5];
                index++;
            }
            rank.color = SetColorBasedOnRank(rank.text);

            // Score
            if(!Plugin.fish)
                percent.text = (st.modifiedRatio * 100).ToString("F") + " %";
            else
            {
                percentLabel.text = lyrics[index];
                percent.text = lyrics[index + 5];
                index++;
            }

            // Combo
            if(!Plugin.fish)
                combo.text = ht.maxCombo.ToString();
            else
            {
                comboLabel.text = lyrics[index];
                combo.text = lyrics[index + 5];
                index++;
            }

            // Misses
            if(!Plugin.fish)
                miss.text = fc ? "FC" : ht.miss.ToString();
            else
            {
                missLabel.text = lyrics[index];
                miss.text = lyrics[index + 5];
                index++;
            }
            if (fc) {
                miss.color = goldColor;
                missLabel.color = goldColor;
            } else
            {
                miss.color = Color.white;
                missLabel.color = Color.white;
            }

            // Pauses
            if (!Plugin.fish)
                if (wt.nbOfPause != 999)
                    pauses.text = wt.nbOfPause.ToString();
                else
                    pauses.text = "-";
            else
            {
                pausesLabel.text = lyrics[index];
                pauses.text = lyrics[index + 5];
            }

            // LowerBand
            if (fc) lowerBandImg.color = goldColor;
            else lowerBandImg.color = Color.white;
            #endregion

            #region LeftSaberStats
            // LeftCircle
            SharedCoroutineStarter.instance.StartCoroutine(AnimateCircle(leftCircleImg, GetCircleFillRatio(at.accLeft), 1.5f));

            // LeftAverageCut
            leftAverage.text = at.accLeft.ToString("0.##");

            // LeftBeforeCut
            leftBeforeCut.text = at.leftAverageCut[0].ToString("0.#");

            // LeftAccuracy
            leftAccuracy.text = at.leftAverageCut[1].ToString("0.#");

            // LeftAfterCut
            leftAfterCut.text = at.leftAverageCut[2].ToString("0.#");

            // LeftDistance
            leftTD.text = at.leftTimeDependence.ToString("0.###");

            // LeftSpeed
            leftSpeed.text = (at.leftSpeed * 3.6f).ToString("0.##") + " Km/h";

            // LeftBeforeSwing
            leftBeforeSwing.text = (at.leftPreswing * 100).ToString("0.##") + " %";

            // LeftAfterSwing
            leftAfterSwing.text = (at.leftPostswing * 100).ToString("0.##") + " %";
            #endregion

            #region RightSaberStats
            // rightCircle
            SharedCoroutineStarter.instance.StartCoroutine(AnimateCircle(rightCircleImg, GetCircleFillRatio(at.accRight), 1.5f));

            // rightAverageCut
            rightAverage.text = at.accRight.ToString("0.##");

            // rightBeforeCut
            rightBeforeCut.text = at.rightAverageCut[0].ToString("0.#");

            // rightAccuracy
            rightAccuracy.text = at.rightAverageCut[1].ToString("0.#");

            // rightAfterCut
            rightAfterCut.text = at.rightAverageCut[2].ToString("0.#");

            // rightDistance
            rightTD.text = at.rightTimeDependence.ToString("0.###");

            // rightSpeed
            rightSpeed.text = (at.rightSpeed * 3.6f).ToString("0.##") + " Km/h";

            // rightBeforeSwing
            rightBeforeSwing.text = (at.rightPreswing * 100).ToString("0.##") + " %";

            // rightAfterSwing
            rightAfterSwing.text = (at.rightPostswing * 100).ToString("0.##") + " %";
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

        private IEnumerator AnimateCircle(ImageView img, float final, float totalTime)
        {
            float timeLeft = totalTime;
            img.fillAmount = 0;

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
