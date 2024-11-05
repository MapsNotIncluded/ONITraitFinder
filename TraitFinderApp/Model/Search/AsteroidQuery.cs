using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using TraitFinderApp.Client.Model.KleiClasses;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TraitFinderApp.Client.Model.Search
{
    public class AsteroidQuery
    {
        int worldIndex;
        Asteroid targetAsteroid;

        public IEnumerable<WorldTrait> Guarantee { get =>_guarantee; set {
                _guarantee = value;
                //Console.WriteLine("Guarantee changed");
                ReevaluateAvailableTraits();
            } }

        private IEnumerable<WorldTrait> _guarantee = new HashSet<WorldTrait>();

        public IEnumerable<WorldTrait> Prohibit { 
            get => _prohibit; 
            set {
                _prohibit = value;
                //Console.WriteLine("Prohibited changed");
                ReevaluateAvailableTraits();
            }
        }
        private IEnumerable<WorldTrait> _prohibit = new HashSet<WorldTrait>();

        public AsteroidQuery(Asteroid target, int index)
        {
            targetAsteroid = target;
            Guarantee = new HashSet<WorldTrait>();
            Prohibit = new HashSet<WorldTrait>();
            worldIndex = index;
        }

        public bool CanToggleGuaranteedTrait(WorldTrait trait)
        {
            if (HasGuaranteedTrait(trait))
                return true;
            else
            {
               return !HasProhibitedTrait(trait)
               && Guarantee.Count() < targetAsteroid.TraitRule.max
               && AvailableTraits.Contains(trait);
            }
        
        
        }
        public int GetMaxCount() => targetAsteroid.TraitRule.max;

        public bool CanToggleProhibitedTrait(WorldTrait trait) => trait != null && !HasGuaranteedTrait(trait);
        public bool HasGuaranteedTrait(WorldTrait trait) => Guarantee.Contains(trait);
        public bool HasProhibitedTrait(WorldTrait trait) => Prohibit.Contains(trait);
        public bool CannotHaveTraits() => targetAsteroid.DisableWorldTraits;
        public bool HasFixedTraits() => targetAsteroid.TraitRule.specificTraits != null && targetAsteroid.TraitRule.specificTraits.Count > 0;

        public void ResetAll()
        {
            if (targetAsteroid?.TraitRule?.specificTraits?.Count > 0)
                return;

            Guarantee = new HashSet<WorldTrait>();
            Prohibit = new HashSet<WorldTrait>();
        }

        public List<WorldTrait> GetAllWorldCompatibleTraits() => DataImport.GetCompatibleTraits(targetAsteroid);
        private HashSet<WorldTrait> _availableTraits = new();
        public HashSet<WorldTrait> AvailableTraits => _availableTraits;


        public void ReevaluateAvailableTraits()
        {
            _availableTraits = GetAllCurrentlyAvailableTraits();
        }


        public HashSet<WorldTrait> GetAllCurrentlyAvailableTraits()
        {
            var allTraits = new List<WorldTrait>(GetAllWorldCompatibleTraits());

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
                        || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.Id)));
                }
            }
            allTraits.RemoveAll((WorldTrait trait) =>
                 !trait.IsValid(targetAsteroid, logErrors: true)
                || trait.exclusiveWithTags.Any(x => ExclusiveWithTags.Any(y => y == x))
                || allSelected.Contains(trait)
                || trait.exclusiveWith.Any(x => allSelected.Any(y => y.Id == x))
                );

            return allTraits.ToHashSet();
        }
    }
}
