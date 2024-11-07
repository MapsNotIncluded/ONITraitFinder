namespace TraitFinderApp.Client.Model
{
    public class Asteroid
    {
        public string Id;
        public string Name;
        public string Image;
        public bool DisableWorldTraits = false;
        public List<WorldTraitRule> TraitRules;
        //all asteroids have only one trait rule (atm)

        public float worldTraitScale = 1.0f;

        public WorldTraitRule TraitRule() => TraitRules.FirstOrDefault();
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
            if(TraitRules == null)
                TraitRules = new List<WorldTraitRule>();

            TraitRules.Add(rule); return this;
        }

        public static List<Asteroid> Values => DataImport.GetActive().asteroids.ToList();
        public static Dictionary<string, Asteroid> KeyValues => DataImport.GetActive().asteroidsDict;
                
        //public static Asteroid FORESTMOONLET = new Asteroid("ForestMoonlet", "Folia Asteroid").AddRule(new WorldTraitRule(2, 4).ForbiddenTag("Oil").ForbiddenTag("NonStartWorld").ForbiddenTrait("GeoDormant"));
        //public static Asteroid IDEALLANDINGSITE = new Asteroid("IdealLandingSite", "Irradiated Forest Asteroid").AddRule(new WorldTraitRule(2, 4).ForbiddenTag("Oil").ForbiddenTag("NonStartWorld").ForbiddenTrait("GeoDormant"));
    }
}
