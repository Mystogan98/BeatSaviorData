using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaviorData
{
	public class SwingHolder
	{
		public SaberSwingRatingCounter ssrc;
		public float preswing, postswing;

		public SwingHolder(SaberSwingRatingCounter _ssrc)
		{
			ssrc = _ssrc;
		}

		public void AddPostSwing(float value)
		{
			if (value < 1)
				postswing = value;
			else
			{
				if (postswing < 1) 
					postswing = 1;
				postswing += (value - 1);
			}
		}
	}

	public static class SwingTranspilerHandler
	{
		public static MethodInfo PreparePreswingMethodInfo = SymbolExtensions.GetMethodInfo(() => PreparePreswing(null));
		public static MethodInfo AddPreswingMethodInfo = SymbolExtensions.GetMethodInfo(() => AddPreswing(0));
		public static MethodInfo AddPostswingMethodInfo = SymbolExtensions.GetMethodInfo(() => AddPostswing(null, 0));

		public static List<SwingHolder> swings = new List<SwingHolder>();
		private static bool takeNextPreswing = false;

		public static void Reset()
		{
			swings = new List<SwingHolder>();
			takeNextPreswing = false;
		}

		public static void PreparePreswing(SaberSwingRatingCounter ssrc)
		{
			if (!swings.Any((s) => s.ssrc == ssrc))
			{
				swings.Add(new SwingHolder(ssrc));
				takeNextPreswing = true;
			}
		}

		public static void AddPreswing(float swing)
		{
			if (takeNextPreswing) {
				swings.Last().preswing = swing;
				takeNextPreswing = false;
			}
		}

		public static void AddPostswing(SaberSwingRatingCounter ssrc, float swing)
		{
			SwingHolder sh = swings.Find((s) => s.ssrc == ssrc);

			if (sh != null)
				sh.AddPostSwing(swing);
		}

		public static SwingHolder GetSwing(SaberSwingRatingCounter ssrc)
		{
			SwingHolder sh = swings.Find((s) => s.ssrc == ssrc);
			swings.Remove(sh);
			return sh;
		}
	}
}
