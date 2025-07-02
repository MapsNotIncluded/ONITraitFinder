using static MudBlazor.Colors;
using TraitFinderApp.Client.Model.KleiClasses;

namespace TraitFinderApp.Model.KleiClasses.Util
{
	public class SeededRandom
	{
		public KRandom rnd;

		public int seed { get; set; }

		public SeededRandom(int seed)
		{
			if (seed == int.MinValue)
			{
				seed = 0;
			}

			this.seed = seed;
			rnd = new KRandom(seed);
		}

		public KRandom RandomSource()
		{
			return rnd;
		}

		public float RandomValue()
		{
			return (float)rnd.NextDouble();
		}

		public double NextDouble()
		{
			return rnd.NextDouble();
		}

		public float RandomRange(float rangeLow, float rangeHigh)
		{
			float num = rangeHigh - rangeLow;
			return rangeLow + (float)(rnd.NextDouble() * (double)num);
		}

		public int RandomRange(int rangeLow, int rangeHigh)
		{
			int num = rangeHigh - rangeLow;
			return rangeLow + (int)(rnd.NextDouble() * (double)num);
		}
	}
}
