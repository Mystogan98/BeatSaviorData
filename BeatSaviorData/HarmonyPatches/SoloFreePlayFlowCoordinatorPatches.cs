using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData.HarmonyPatches
{
	[HarmonyPatch(typeof(SoloFreePlayFlowCoordinator), "ProcessLevelCompletionResultsAfterLevelDidFinish")]
	class SoloFreePlayFlowCoordinatorPatches
	{
		static void Postfix(ref SoloFreePlayFlowCoordinator __instance, LevelCompletionResults levelCompletionResults)
		{
			// Show end of song UI
			if(levelCompletionResults.levelEndAction == LevelCompletionResults.LevelEndAction.None)
				EndOfLevelUICreator.Show(__instance);
		}
	}
}