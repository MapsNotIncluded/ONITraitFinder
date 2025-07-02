using TraitFinderApp.Client.Model.KleiClasses;

namespace TraitFinderApp.Client.Model
{
    public class WorldTraitRule
    {
        public int min, max;
        public List<string> requiredTags, specificTraits, forbiddenTags, forbiddenTraits;
        public WorldTraitRule(int _min, int _max)
        {
            min = _min;
            max = _max;
            requiredTags = new List<string>();
            specificTraits = new List<string>();
            forbiddenTags = new List<string>();
            forbiddenTraits = new List<string>();
        }
    }
}
