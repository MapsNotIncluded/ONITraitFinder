using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using TraitFinderApp.Client.Model.KleiClasses;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TraitFinderApp.Client.Model.Search
{
	public class AsteroidQuery : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			parent.OnAsteroidChanged();
		}
		bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
			{
				return false;
			}

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		int worldIndex;
		Asteroid targetAsteroid;
		SearchQuery parent;

		public IEnumerable<WorldTrait> Guarantee
		{
			get => _guarantee; set
			{
				_guarantee = value;
				//Console.WriteLine("Guarantee changed");
				ReevaluateAvailableTraits();
				OnPropertyChanged(nameof(Guarantee));
			}
		}

		private IEnumerable<WorldTrait> _guarantee = new HashSet<WorldTrait>();

		public IEnumerable<WorldTrait> Prohibit
		{
			get => _prohibit;
			set
			{
				_prohibit = value;
				//Console.WriteLine("Prohibited changed");
				ReevaluateAvailableTraits();
				OnPropertyChanged(nameof(Prohibit));
			}
		}
		private IEnumerable<WorldTrait> _prohibit = new HashSet<WorldTrait>();

		public AsteroidQuery(SearchQuery _parent, Asteroid target, int index)
		{
			parent = _parent;
			targetAsteroid = target;
			Guarantee = new HashSet<WorldTrait>();
			Prohibit = new HashSet<WorldTrait>();
			worldIndex = index;
		}

		public bool CanToggleGuaranteedTrait(WorldTrait trait)
		{
			if (HasFixedTrait(trait))
				return false;

			if (HasGuaranteedTrait(trait))
				return true;
			else
			{
				return !HasProhibitedTrait(trait)
				&& Guarantee.Count() < GetMaxCount()
				&& AvailableTraits.Contains(trait);
			}

		}

		public List<WorldTrait> GetCurrentGuaranteesIncFixed()
		{
			var currentGuarantees = new List<WorldTrait>();
			if (HasFixedTraits())
			{
				foreach (var trait in targetAsteroid.GetConsolidatedTraitRule().specificTraits)
				{
					if (WorldTrait.KeyValues.TryGetValue(trait, out var value))
						currentGuarantees.Add(value);
				}
			}
			foreach( var guaranteed in Guarantee)
				if(!currentGuarantees.Contains(guaranteed))
					currentGuarantees.Add(guaranteed);

			return currentGuarantees;
		}

		public int GetMaxCountIncGuaranteed() => GetMaxCount() + (targetAsteroid.GetConsolidatedTraitRule().specificTraits?.Count ?? 0);
		public int GetMaxCount() => targetAsteroid.GetConsolidatedTraitRule().max;

		public bool CanToggleProhibitedTrait(WorldTrait trait) => trait != null && !HasGuaranteedTrait(trait);
		public bool HasGuaranteedTrait(WorldTrait trait) => Guarantee.Contains(trait);
		public bool HasProhibitedTrait(WorldTrait trait) => Prohibit.Contains(trait);
		public bool CannotHaveTraits() => targetAsteroid.DisableWorldTraits;
		public bool HasFixedTraits() => targetAsteroid.GetConsolidatedTraitRule().specificTraits != null && targetAsteroid.GetConsolidatedTraitRule().specificTraits.Count > 0;
		public bool HasFixedTrait(WorldTrait trait) => HasFixedTraits() && targetAsteroid.GetConsolidatedTraitRule().specificTraits.Contains(trait.Id);


		public void ResetAll()
		{
			if (targetAsteroid?.GetConsolidatedTraitRule()?.specificTraits?.Count > 0)
				return;

			Guarantee = new HashSet<WorldTrait>();
			Prohibit = new HashSet<WorldTrait>();
		}

		public List<WorldTrait> GetAllWorldCompatibleTraits() => DataImport.GetActive().GetCompatibleTraits(targetAsteroid);
		private HashSet<WorldTrait> _availableTraits = new();
		public HashSet<WorldTrait> AvailableTraits => _availableTraits;


		public void ReevaluateAvailableTraits()
		{
			_availableTraits = GetAllCurrentlyAvailableTraits();
			parent.ClearQueryResults();
		}

		public bool HasFilters() => Guarantee.Any() || Prohibit.Any();

		public HashSet<WorldTrait> GetAllCurrentlyAvailableTraits()
		{
			var allTraits = new List<WorldTrait>(GetAllWorldCompatibleTraits());

			var allSelected = Guarantee;

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
