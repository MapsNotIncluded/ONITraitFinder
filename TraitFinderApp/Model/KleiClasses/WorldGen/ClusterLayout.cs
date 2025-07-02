using TraitFinderApp.Client.Model.Search;
using TraitFinderApp.Model.KleiClasses;

namespace TraitFinderApp.Client.Model
{
    public class ClusterLayout
    {
        public string Id;
        public string Name;
        public string Prefix;
        public int menuOrder;
        public int startWorldIndex;
        public int numRings;
		public List<string> RequiredDlcsIDs;
        public List<string> ForbiddenDlcIDs;
        public List<WorldPlacement> worldPlacements;
        public List<SpaceMapPOIPlacement> poiPlacements;
		public int clusterCategory;
		public bool disableStoryTraits;
		public int fixedCoordinate;
        public string[] clusterTags;



		public string DisplayName() => Name;
        
        public string Image() => worldPlacements[startWorldIndex].Asteroid.Image;
        public Asteroid StarterAsteroid() => worldPlacements[startWorldIndex].Asteroid;


		public List<Dlc> RequiredDlcs;
        public List<Dlc> ForbiddenDlcs;


        public ClusterCategory ClusterCategory;

        public bool HasContentDlcRequirement() => RequiredDlcs != null && RequiredDlcs.Any(dlc => !dlc.IsMainVersion);
        public Dlc? GetContentDlcRequirement() => HasContentDlcRequirement() ? RequiredDlcs.First(dlc => !dlc.IsMainVersion) : null;

		public bool HasFixedCoordinate() => fixedCoordinate > 0;


        public bool DlcRequirementsFulfilled(List<Dlc> requirements) => !RequiredDlcs.Except(requirements).Any() && !ForbiddenDlcs.Intersect(requirements).Any();

        public bool AllowedWithCurrentQuery(SearchQuery query) => query.ActiveMode == ClusterCategory;
        public void InitBindings(Data data)
        {
            foreach(var placement in worldPlacements)
			{
				placement.InitBindings(data);
			}

			RequiredDlcs = new();
            if (RequiredDlcs != null)
                foreach (var dlc in RequiredDlcsIDs)
                {
                    RequiredDlcs.Add(Dlc.KeyValues[dlc]);
                }
            ForbiddenDlcs = new();
            if (ForbiddenDlcIDs != null)
                foreach (var dlc in ForbiddenDlcIDs)
                {
                    ForbiddenDlcs.Add(Dlc.KeyValues[dlc]);
                }
            switch (clusterCategory)
            {
                case 0:
                    ClusterCategory = ClusterCategory.BASEGAME_STANDARD;
                    break;
                case 1:
                    ClusterCategory = ClusterCategory.SPACEDOUT_CLASSIC;
                    break;
                case 2:
                    ClusterCategory = ClusterCategory.SPACEDOUT_SPACEDOUT;
                    break;
                case 3:
                    ClusterCategory = RequiredDlcsIDs.Contains(Dlc.SPACEDOUT_ID) ? ClusterCategory.SPACEDOUT_THELAB : ClusterCategory.BASEGAME_THELAB;
                    break;

            }

		}
		public bool HasAnyTags(List<string> tags)
		{
			foreach (string tag in tags)
			{
				if (clusterTags.Contains(tag))
				{
					return true;
				}
			}

			return false;
		}
		public static List<ClusterLayout> Values => DataImport.GetActive().clusters.OrderBy(i=>i.menuOrder).ToList();
        public static Dictionary<string, ClusterLayout> KeyValues => DataImport.GetActive().clustersDict;       
    }
}
