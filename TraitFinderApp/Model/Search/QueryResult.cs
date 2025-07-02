using OniStarmapGenerator.Model.Search;
using System.Xml.Linq;
using TraitFinderApp.Client.Model;
using TraitFinderApp.Model.Mixing;

namespace TraitFinderApp.Model.Search
{
    public class QueryResult
    {
        public int seed;
        public ClusterLayout cluster;
        public List<QueryAsteroidResult> asteroidsWithTraits;
        public List<DistanceBand> distanceBands;
        public bool ShowStarmap = false;

        public override bool Equals(object? obj)
        {
            return obj is QueryResult t && t.GetHashCode() == this.GetHashCode();
        }
        public override string ToString()
        {
            return seed.ToString();
        }
        public override int GetHashCode() => seed.GetHashCode();

        public string GetCoordinate() => cluster.Prefix+"-"+seed+"-0-0-"+GameSettingsInstance.GetMixingSettingsCode();
    }
}
