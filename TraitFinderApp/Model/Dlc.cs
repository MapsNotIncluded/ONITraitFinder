namespace TraitFinderApp.Client.Model
{
    public class Dlc
    {
        public static string BASEGAME_ID = "", SPACEDOUT_ID = "EXPANSION1_ID", FROSTYPLANET_ID = "DLC2_ID";


        public string ID;
        //localize
        public string Name;
        public bool IsMainVersion = false;
        public string Image;

        public static readonly Dlc BASEGAME = new Dlc()
        {
            ID = BASEGAME_ID,
            IsMainVersion = true,
            Name = "Base Game",
            Image = "./images/logos/logo_oni.png",
        };
        public static readonly Dlc SPACEDOUT = new Dlc()
        {
            ID = SPACEDOUT_ID,
            IsMainVersion = true,
            Name = "Spaced Out!",
            Image = "./images/logos/logo_spaced_out.png",
        };
        public static readonly Dlc FROSTYPLANET = new Dlc()
        {
            ID = FROSTYPLANET_ID,
            IsMainVersion = false,
            Name = "Frosty Planet",
            Image = "./images/logos/logo_frosty_planet_banner.webp",
        };
        public static IEnumerable<Dlc> Values
        {
            get
            {
                yield return BASEGAME;
                yield return SPACEDOUT;
                yield return FROSTYPLANET;
            }
        }
        public static Dictionary<string, Dlc> KeyValues = new()
        {
            {BASEGAME_ID,BASEGAME },
            {SPACEDOUT_ID,SPACEDOUT},
            {FROSTYPLANET_ID,FROSTYPLANET},
        };
    }
}
