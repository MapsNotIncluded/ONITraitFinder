namespace TraitFinderApp.Client.Model
{
	public class WorldPlacement
	{
		public Asteroid Asteroid;
		public enum LocationType
		{
			Cluster,
			Startworld,
			InnerCluster
		}
		public string world;
		public WorldMixing worldMixing { get; set; }
		public int buffer { get; set; }
		public LocationType locationType { get; set; }

		public int x { get; set; }

		public int y { get; set; }

		public int width { get; set; }

		public int height { get; set; }

		public bool startWorld { get; set; }

		public int hiddenY { get; set; }

		public WorldPlacement() { }

		public void InitBindings(Data data)
		{
			if (data.asteroidsDict.TryGetValue(world, out var asteroid))
			{
				Asteroid = asteroid;
			}
			else
			{
				Console.WriteLine($"WorldPlacement: Could not find asteroid for world {world}");
			}
		}
		public bool IsMixingPlacement()
		{
			if (worldMixing.requiredTags.Count == 0)
			{
				return worldMixing.forbiddenTags.Count != 0;
			}

			return true;
		}
	}
}
