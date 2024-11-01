using SeedFinder.Client.Model.KleiClasses;

namespace SeedFinder.Client.Model
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
        public WorldTraitRule SpecificTag(string tag)
        {
            specificTraits.Add(tag);
            return this;
        }
        public WorldTraitRule RequiredTag(string tag)
        {
            requiredTags.Add(tag);
            return this;
        }
        public WorldTraitRule ForbiddenTag(string tag)
        {
            forbiddenTags.Add(tag);
            return this;
        }
        public WorldTraitRule ForbiddenTrait(string tag)
        {
            specificTraits.Add(tag);
            return this;
        }

    }
}
