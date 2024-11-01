using static MudBlazor.CategoryTypes;
using System.Xml.Linq;

namespace SeedFinder.Client.Model
{
    public class WorldTrait
    {
        public string ID;
        public string Name;
        public List<string> forbiddenDLCIds, exclusiveWith, exclusiveWithTags, traitTags;

        public WorldTrait()
        {
            exclusiveWith = new List<string>();
            exclusiveWithTags = new List<string>();
            forbiddenDLCIds = new List<string>();
            traitTags = new List<string>();
            Name = string.Empty;
            ID = string.Empty;
        }
    }
}
