using SeedFinder.Client.Model.Search;

namespace SeedFinder.Client.Model
{
    public class ClusterLayout
    {
        public string Id;
        public string Name;
        public string Prefix;
        public string Image => WorldPlacements.First().Asteroid.Image;
        public List<Dlc> RequiredDlcs;
        public List<Dlc> ForbiddenDlcs;
        public List<WorldPlacement> WorldPlacements;
        public ClusterCategory clusterCategory;
        public int startWorldIndex { get; set; }

        public bool DlcRequirementsFulfilled(List<Dlc> requirements) => !requirements.Except(RequiredDlcs).Any() && requirements.Intersect(ForbiddenDlcs).Any();

        public bool AllowedWithCurrentQuery(SearchQuery query) => query.ActiveMode == clusterCategory && DlcRequirementsFulfilled(query.ActiveDlcs);

        public static List<ClusterLayout> GetValues()
        {
            var values = new List<ClusterLayout>();

            return values;
        }
    }
}
