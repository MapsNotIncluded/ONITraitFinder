namespace OniStarmapGenerator.Model.Search
{
	public class DistanceBand
	{
		public int Distance;
		public List<VanillaStarmapLocation> Destinations=new();
        public DistanceBand(int i)
        {
			Distance = ++i; 
        }

        public bool HasLocations => Destinations.Count > 0;
		public string GetDistanceText() => Distance*10 + "k km:";
	}
}
