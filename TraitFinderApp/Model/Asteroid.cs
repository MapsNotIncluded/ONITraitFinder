namespace TraitFinderApp.Client.Model
{
	public class Asteroid
	{
		public string Id;
		public string Name;
		public string Image;
		public bool DisableWorldTraits = false;
		public List<WorldTraitRule> TraitRules;
		public float worldTraitScale = 1.0f;

		///nothing built in, just a measure for me to keep info on stuff like the teleporter, ceres heatpump
		public List<string> SpecialPOIs = new();

		public WorldTraitRule GetConsolidatedTraitRule()
		{
			if (TraitRules == null)
				return new(0, 0);
			if (TraitRules.Count == 1)
				return TraitRules.First();

			var rule = new WorldTraitRule(0, 0);
			foreach (var traitRule in TraitRules)
			{
				if (traitRule.min > 0)
					rule.min = traitRule.min;
				if (traitRule.max > 0)
					rule.max = traitRule.max;

				if (traitRule.requiredTags != null && traitRule.requiredTags.Any())
					rule.requiredTags.AddRange(traitRule.requiredTags);

				if (traitRule.specificTraits != null && traitRule.specificTraits.Any())
					rule.specificTraits.AddRange(traitRule.specificTraits);

				if (traitRule.forbiddenTags != null && traitRule.forbiddenTags.Any())
					rule.forbiddenTags.AddRange(traitRule.forbiddenTags);

				if (traitRule.forbiddenTraits != null && traitRule.forbiddenTraits.Any())
					rule.forbiddenTraits.AddRange(traitRule.forbiddenTraits);
			}
			return rule;

		}
		public Asteroid(string _id, string _name, string _image, List<WorldTraitRule> _rules)
		{
			Id = _id;
			Name = _name;
			Image = _image;
			TraitRules = _rules;
		}
		public Asteroid() { }

		public void InitBindings(Data data)
		{
			Image = $"./images/asteroids/{Path.GetFileName(Id)}.png";
		}

		public Asteroid(string _id, string _name)
		{
			Id = _id;
			Name = _name;
			Image = $"./images/{Id}.png";
			TraitRules = null;
		}
		public Asteroid NoTraits()
		{
			DisableWorldTraits = true;
			return this;
		}
		public Asteroid AddRule(WorldTraitRule rule)
		{
			if (TraitRules == null)
				TraitRules = new List<WorldTraitRule>();

			TraitRules.Add(rule); return this;
		}

		public static List<Asteroid> Values => DataImport.GetActive().asteroids.ToList();
		public static Dictionary<string, Asteroid> KeyValues => DataImport.GetActive().asteroidsDict;

	}
}
