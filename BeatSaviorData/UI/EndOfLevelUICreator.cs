using BeatSaberMarkupLanguage;
using BeatSaviorData;
using IPA.Utilities;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BeatSaviorData.UI;

namespace BeatSaviorData
{
	public class EndOfLevelUICreator : MonoBehaviour
	{
		public Plugin plugin;

		private static EndOfLevelUICreator instance;

		private SongData lastData;
		private EndOfLevelUI leftUi;
		private ScoreGraphUI rightUi;
		private SpeedonsUI speedonsUI;

		private void Start()
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}

		public static void Show(FlowCoordinator fc)
		{
			fc.InvokeMethod<object , FlowCoordinator>("SetLeftScreenViewController", new object[] { instance.leftUi, ViewController.AnimationType.None });

			/* Temporary */
			if (instance.speedonsUI != null)
			{
				fc.InvokeMethod<object, FlowCoordinator>("SetRightScreenViewController", new object[] { instance.speedonsUI, ViewController.AnimationType.None });
				return;
			}

			if (!SettingsMenu.instance.DisableGraphPanel)
				fc.InvokeMethod<object, FlowCoordinator>("SetRightScreenViewController", new object[] { instance.rightUi, ViewController.AnimationType.None });
		}

		public static void Create()
		{
			if (SettingsMenu.instance.EnableUI)
				instance.StartCoroutine(instance.WaitForData());
		}

		private IEnumerator WaitForData()
		{
			while (!plugin.IsComputeFinished())
				yield return null;

			lastData = plugin.GetSongData();

			if(leftUi == null)
				leftUi = BeatSaberUI.CreateViewController<EndOfLevelUI>();
			leftUi.Refresh(lastData);

			if (!SettingsMenu.instance.DisableGraphPanel)
			{
				if(rightUi == null)
					rightUi = BeatSaberUI.CreateViewController<ScoreGraphUI>();
				rightUi.Refresh(lastData);
			}

			/* Temporary */
			if(DateTime.Today >= new DateTime(2022, 4, 13) && DateTime.Today <= new DateTime(2022, 4, 18))
            {
				if (speedonsUI == null)
					speedonsUI = BeatSaberUI.CreateViewController<SpeedonsUI>();
				speedonsUI.Refresh();
            }
		}
	}
}
