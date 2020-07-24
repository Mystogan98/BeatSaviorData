using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using UnityEngine;
using System.Reflection.Emit;

// This will be used to get the pre-swing overswing stat as soon as I'll have figure out how to do it
namespace BeatSaviorData.HarmonyPatches
{
	/*[HarmonyPatch(typeof(SaberMovementData), "ComputeSwingRating")]
	public static class SaberMovementDataPatches
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> code = new List<CodeInstruction>(instructions);
			foreach(CodeInstruction c in code)
			{
				code.Add(new CodeInstruction(OpCodes.Call));
			}
		}

		private static void LoadVariable(float num)
		{
			Logger.log.Info("Overswing is : " + num);
		}
	}*/
}