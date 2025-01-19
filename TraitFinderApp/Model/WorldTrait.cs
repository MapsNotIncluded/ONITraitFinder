using static MudBlazor.CategoryTypes;
using System.Xml.Linq;
using TraitFinderApp.Client.Model.KleiClasses;
using System;
using MudBlazor.Utilities;

namespace TraitFinderApp.Client.Model
{
    public class WorldTrait
    {
        public override bool Equals(object? obj)
        {
            return obj is WorldTrait t && t.GetHashCode() == this.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
        public override int GetHashCode() => Id.GetHashCode();

        public string ImageString()=> $"<image width=\"20\" height=\"20\" xlink:href=\"{Image}\" alt=\"{Name}\">";


        public string Id;
        public string Name;
        public string Description;
		public string ColorHex;
        public string Image=> "./images/traits/" + Path.GetFileName(Id)+".png";
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
        }

        public static List<WorldTrait> Values => DataImport.GetActive().worldTraits;

        public static Dictionary<string, WorldTrait> KeyValues => DataImport.GetActive().worldTraitsDict;
        

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
