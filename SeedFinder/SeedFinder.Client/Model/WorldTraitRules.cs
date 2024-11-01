using SeedFinder.Client.Model.KleiClasses;

namespace SeedFinder.Client.Model
{
    public class WorldTraitRules
    {
        public int min, max;
        public List<string> requiredTags, specificTraits, forbiddenTags, forbiddenTraits;
    }
}
