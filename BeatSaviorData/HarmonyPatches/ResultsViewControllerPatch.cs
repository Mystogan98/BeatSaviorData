using HarmonyLib;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BS_Utils.Utilities;

namespace BeatSaviorData.HarmonyPatches
{
	[HarmonyPatch(typeof(ResultsViewController), "SetDataToUI")]
	class ResultsViewControllerPatches
	{
		static void Postfix(ref ResultsViewController __instance)
		{
			// Create end of song UI
			EndOfLevelUICreator.Create();
		}
	}
}
