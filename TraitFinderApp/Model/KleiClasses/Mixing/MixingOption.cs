using TraitFinderApp.Client.Model;

namespace TraitFinderApp.Model.KleiClasses.Mixing
{
	public class MixingOption<T> : IComparable<MixingOption<T>>
	{
		public Asteroid worldgenPath;

		public T mixingSettings;

		public int minCount;

		public int maxCount;

		public bool IsExhausted => maxCount <= 0;

		public bool IsSatisfied => minCount <= 0;

		public void Consume()
		{
			minCount--;
			maxCount--;
		}

		public int CompareTo(MixingOption<T> other)
		{
			int num = other.minCount.CompareTo(minCount);
			if (num != 0)
			{
				return num;
			}

			return other.maxCount.CompareTo(maxCount);
		}
	}

}
