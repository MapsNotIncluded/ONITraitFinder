using TraitFinderApp.Client.Model;

namespace TraitFinderApp.Model.Search
{
    public class QueryAsteroidResult
    {
        public Asteroid Asteroid;
        public List<WorldTrait> Traits;

        public QueryAsteroidResult() { }
        public QueryAsteroidResult(Asteroid _asteroid, List<WorldTrait> _traits)
        {
            Asteroid = _asteroid;
            Traits = _traits;
        }
    }
}
