using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;

namespace BeatSaviorData.HarmonyPatches
{
	[HarmonyPatch(typeof(SaberMovementData), "ComputeSwingRating", new Type[] { typeof(bool), typeof(float) })]
	public static class SaberMovementDataPatches
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> tmp = instructions.ToList(); ;

			List<CodeInstruction> code = new List<CodeInstruction>()
			{
				new CodeInstruction(OpCodes.Ldloc_S, 4),
				new CodeInstruction(OpCodes.Call, SwingTranspilerHandler.AddPreswingMethodInfo)
			};

			tmp.InsertRange(111, code);

			tmp[113].MoveLabelsTo(tmp[111]);

			return tmp;
		}
	}
}