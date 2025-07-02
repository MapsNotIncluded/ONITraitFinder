namespace TraitFinderApp.Model.KleiClasses
{
	public class SpaceMapPOIPlacement
	{
		public List<string> pois { get; set; }

		public int numToSpawn { get; set; }

		public MinMaxI allowedRings { get; set; }

		public bool avoidClumping { get; set; }

		public bool canSpawnDuplicates { get; set; }

		public bool guarantee { get; set; }

		public SpaceMapPOIPlacement()
		{
			allowedRings = new MinMaxI(0, 9999);
		}
	}
}
