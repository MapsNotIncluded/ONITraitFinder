using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using SeedFinder.Client.Model.KleiClasses;
using System.Collections.Generic;
using System.Linq;

namespace SeedFinder.Client.Model.Search
{
    public class AsteroidQuery
    {
        int worldIndex;
        Asteroid targetAsteroid;

        public IEnumerable<WorldTrait> Guarantee { get; set; }

        public IEnumerable<WorldTrait> Prohibit { get; set; }

        public AsteroidQuery(Asteroid target, int index)
        {
            targetAsteroid = target;
            Guarantee = new HashSet<WorldTrait>();
            Prohibit = new HashSet<WorldTrait>();
            worldIndex = index;
        }

        public bool CanAddGuaranteedTrait(WorldTrait trait)
        {
            return !Prohibit.Contains(trait);
        }
        public bool CanAddProhibitedTrait(WorldTrait trait)
        {
            return !Guarantee.Contains(trait);
        }

        public bool CannotHaveTraits() => targetAsteroid.DisableWorldTraits;

        public List<WorldTrait> GetAllWorldCompatibleTraits()
        {
            var allTraits = new List<WorldTrait>(WorldTrait.Values);


            if (targetAsteroid.DisableWorldTraits)
                return new();

            List<string> ExclusiveWithTags = new List<string>();

            
            if (targetAsteroid.TraitRules != null)
            {
                foreach (var rule in targetAsteroid.TraitRules)
                {
                    TagSet? requiredTags = (rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null;
                    TagSet? forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

                    allTraits.RemoveAll((WorldTrait trait) =>
                          (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
                        || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
                        || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.Id))
                        || !trait.IsValid(targetAsteroid, logErrors: true));
                }
            }
            allTraits.RemoveAll((WorldTrait trait) => !trait.IsValid(targetAsteroid, logErrors: true)
                );

            return allTraits;
        }

        public List<WorldTrait> GetAllCurrentlyAvailableTraits()
        {
            var allTraits = WorldTrait.Values;

            var allSelected = Guarantee.Concat(Prohibit);

            if (targetAsteroid.DisableWorldTraits)
                return new();

            List<string> ExclusiveWithTags = new List<string>();

            foreach (var trait in allSelected)
            {
                ExclusiveWithTags.AddRange(trait.exclusiveWithTags);
            }
            if (targetAsteroid.TraitRules != null)
            {
                foreach (var rule in targetAsteroid.TraitRules)
                {
                    TagSet requiredTags = (rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null;
                    TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

                    allTraits.RemoveAll((WorldTrait trait) =>
                        (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
                        || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
                        || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.Id))
                        || !trait.IsValid(targetAsteroid, logErrors: true));
                }
            }
            allTraits.RemoveAll((WorldTrait trait) =>
                 !trait.IsValid(targetAsteroid, logErrors: true)
                || trait.exclusiveWithTags.Any(x => ExclusiveWithTags.Any(y => y == x))
                || allSelected.Contains(trait)
                || trait.exclusiveWith.Any(x => allSelected.Any(y => y.Id == x))
                );

            return allTraits;
        }
    }
}
