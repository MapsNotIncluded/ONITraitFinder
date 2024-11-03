using static MudBlazor.CategoryTypes;
using System.Xml.Linq;
using SeedFinder.Client.Model.KleiClasses;
using System;

namespace SeedFinder.Client.Model
{
    public class WorldTrait
    {
        public override string ToString()
        {
            return Name;
        }

        public string Id;
        public string Name;
        public string ColorHex;
        public string Image=>"images/traits/"+Path.GetFileName(Id)+".png";
        public List<string> forbiddenDLCIds, exclusiveWith, exclusiveWithTags, traitTags;
        public Dictionary<string, int> globalFeatureMods { get; set; }
        public TagSet traitTagsSet
        {
            get
            {
                if (m_traitTagSet == null)
                {
                    m_traitTagSet = new TagSet(traitTags);
                }

                return m_traitTagSet;
            }
        }
        public TagSet m_traitTagSet;
        public WorldTrait()
        {
            exclusiveWith = new List<string>();
            exclusiveWithTags = new List<string>();
            forbiddenDLCIds = new List<string>();
            traitTags = new List<string>();
            Name = string.Empty;
            Id = string.Empty;
        }

        public static List<WorldTrait> Values = KeyValues.Values.ToList();

        public static Dictionary<string, WorldTrait> KeyValues
        {
            get
            {
                if (_values == null)
                {
                    DataImport.ImportGameData(true);
                }

                return _values;
            }
            set
            {
                _values = value;
            }
        }
        private static Dictionary<string, WorldTrait> _values = null;

        public bool IsValid(Asteroid world, bool logErrors)
        {
            int num = 0;
            int num2 = 0;
            foreach (KeyValuePair<string, int> globalFeatureMod in globalFeatureMods)
            {
                num += globalFeatureMod.Value;
                num2 += (int)(world.worldTraitScale * (float)globalFeatureMod.Value);
            }

            if (globalFeatureMods.Count > 0 && num2 == 0)
            {
                return false;
            }

            return true;
        }
    }
}
