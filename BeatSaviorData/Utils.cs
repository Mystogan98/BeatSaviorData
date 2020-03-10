using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
