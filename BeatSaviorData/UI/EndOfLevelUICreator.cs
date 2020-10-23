using BeatSaberMarkupLanguage;
using BeatSaviorData;
using BS_Utils.Utilities;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaviorData
{
	public class EndOfLevelUICreator : MonoBehaviour
	{
		public Plugin plugin;

		private static EndOfLevelUICreator instance;

		private SongData lastData;
		private EndOfLevelUI leftUi;
		private ScoreGraphUI rightUi;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}

		public static void Show(FlowCoordinator fc)
		{
			fc.InvokeMethod("SetLeftScreenViewController", new object[] { instance.leftUi, false });
			fc.InvokeMethod("SetRightScreenViewController", new object[] { instance.rightUi, false});
		}

		public static void Create()
		{
			if(SettingsMenu.instance.EnableUI)
				instance.StartCoroutine(instance.WaitForData());
		}

		private IEnumerator WaitForData()
		{
			while (!plugin.IsComputeFinished())
				yield return null;

			lastData = plugin.GetSongData();

			leftUi = BeatSaberUI.CreateViewController<EndOfLevelUI>();
			rightUi = BeatSaberUI.CreateViewController<ScoreGraphUI>();

			leftUi.data = lastData;
			rightUi.data = lastData;
		}
	}
}
