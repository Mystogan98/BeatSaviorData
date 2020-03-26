namespace BeatSaviorData
{
	class Utils
	{
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
	}
}
