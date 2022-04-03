using UnityEngine;

namespace BeatSaviorData
{
	class Utils
	{
		public static System.Random random = new System.Random();
		public static float SafeDivide(float a, float b)
		{
			try
			{
				return a / b;
			}
			catch
			{
				return 0;
			}
		}

		public static float SafeAverage(float a, float nbA, float b, float nbB)
		{
			if (!float.IsNaN(a) && !float.IsNaN(b))
				return (a * nbA + b * nbB) / (nbA + nbB);
			else if (float.IsNaN(b))
				return a;
			else
				return b;
		}

		public static float[] FloatArrayFromVector(Vector3 v)
		{
			return new float[] { v.x, v.y, v.z };
		}

		public static int MaxRawScoreForNumberOfNotes(int noteCount)
		{
			int num = 0;
			int i;
			for (i = 1; i < 8; i *= 2)
			{
				if (noteCount < i * 2)
				{
					num += i * noteCount;
					noteCount = 0;
					break;
				}
				num += i * i * 2 + i;
				noteCount -= i * 2;
			}
			num += noteCount * i;
			return num * 115;
		}
	}
}
