using TraitFinderApp.Client.Model.Search;
using static MudBlazor.Icons.Custom;

namespace TraitFinderApp.Client.Model
{
    public class ClusterCategory
    {
        public bool AllowedWithCurrentQuery(SearchQuery query)
        {
            //todo: checks if any cluster is available with current filters
            foreach (var dlc in RequiredDlc)
            {
                if (!query.ActiveDlcs.Contains(dlc))
                    return false;
            }


            return true;
        }

        public List<Dlc> RequiredDlc;
        public string Name;
        public string Image;
        public int ID; //underlying game enum value

        public static ClusterCategory BASEGAME_STANDARD = new ClusterCategory()
        {
            RequiredDlc = new() { Dlc.BASEGAME },
            Name = "Standard",
            Image = "./images/gamemodes/gamemode_basegame_standard.png",
            ID = 0
        };
        public static ClusterCategory BASEGAME_THELAB = new ClusterCategory()
        {
            RequiredDlc = new() { Dlc.BASEGAME },
            Name = "The Lab",
            Image = "./images/gamemodes/gamemode_basegame_thelab.png",
            ID = 3
        };
        public static ClusterCategory SPACEDOUT_CLASSIC = new ClusterCategory()
        {
            RequiredDlc = new() { Dlc.SPACEDOUT },
            Name = "Classic",
            Image = "./images/gamemodes/gamemode_spacedout_classic.png",
            ID = 1
        };
        public static ClusterCategory SPACEDOUT_SPACEDOUT = new ClusterCategory()
        {
            RequiredDlc = new() { Dlc.SPACEDOUT },
            Name = "Spaced Out!",
            Image = "./images/gamemodes/gamemode_spacedout_spacedout.png",
            ID = 1
        };
        public static ClusterCategory SPACEDOUT_THELAB = new ClusterCategory()
        {
            RequiredDlc = new() { Dlc.SPACEDOUT },
            Name = "The Lab",
            Image = "./images/gamemodes/gamemode_spacedout_thelab.png",
            ID = 1
        };
        public static IEnumerable<ClusterCategory> Values
        {
            get
            {
                yield return BASEGAME_STANDARD;
                yield return BASEGAME_THELAB;
                yield return SPACEDOUT_CLASSIC;
                yield return SPACEDOUT_SPACEDOUT;
                yield return SPACEDOUT_THELAB;
            }
        }
    }
}
