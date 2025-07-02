using TraitFinderApp.Model.KleiClasses.Util;

namespace TraitFinderApp.Model.KleiClasses
{

	[Serializable]
	public struct MinMaxI
	{
		public int min { get; set; }

		public int max { get; set; }

		public MinMaxI(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public int GetRandomValueWithinRange(SeededRandom rnd)
		{
			return rnd.RandomRange(min, max);
		}

		public int GetAverage()
		{
			return (min + max) / 2;
		}

		public void Mod(MinMaxI mod)
		{
			min += mod.min;
			max += mod.max;
		}

		public override string ToString()
		{
			return $"min:{min} max:{max}";
		}
	}
}
