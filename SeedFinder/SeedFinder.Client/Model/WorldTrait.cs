using static MudBlazor.CategoryTypes;
using System.Xml.Linq;

namespace SeedFinder.Client.Model
{
    public class WorldTrait
    {
        public string Id;
        public string Name;
        public string ColorHex;
        public List<string> forbiddenDLCIds, exclusiveWith, exclusiveWithTags, traitTags;

        public WorldTrait()
        {
            exclusiveWith = new List<string>();
            exclusiveWithTags = new List<string>();
            forbiddenDLCIds = new List<string>();
            traitTags = new List<string>();
            Name = string.Empty;
            Id = string.Empty;
        }
        public static Dictionary<string, WorldTrait> Values
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
    }
}
