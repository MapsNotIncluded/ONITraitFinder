
using static TraitFinderApp.Client.Model.DataImport;

namespace TraitFinderApp.Client.Model
{
	public class Dlc
	{
		public static string BASEGAME_ID = "", SPACEDOUT_ID = "EXPANSION1_ID", FROSTYPLANET_ID = "DLC2_ID", PREHISTORICPLANET_ID = "DLC4_ID";


		public string ID;
		//localize
		public string Name;
		public bool IsMainVersion = false;
		public string Image;
		public List<spaceDestinations> extraSpaceDestinationsBasegame = [];

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
			Name = "Frosty Planet Pack",
			Image = "./images/logos/logo_frosty_planet_banner.webp",
			extraSpaceDestinationsBasegame = [new("DLC2CeresSpaceDestination", 3, 10)]
		};
		public static readonly Dlc PREHISTORICPLANET = new Dlc()
		{
			ID = PREHISTORICPLANET_ID,
			IsMainVersion = false,
			Name = "Prehistoric Planet Pack",
			Image = "./images/logos/Prehistoric_Planet_Banner.png",
			extraSpaceDestinationsBasegame = [new("DLC4PrehistoricSpaceDestination", 3, 10)]
		};
		public static IEnumerable<Dlc> Values
		{
			get
			{
				yield return BASEGAME;
				yield return SPACEDOUT;
				yield return FROSTYPLANET;
				yield return PREHISTORICPLANET;
			}
		}
		public static Dictionary<string, Dlc> KeyValues = new()
		{
			{BASEGAME_ID,BASEGAME },
			{SPACEDOUT_ID,SPACEDOUT},
			{FROSTYPLANET_ID,FROSTYPLANET},
			{PREHISTORICPLANET_ID,PREHISTORICPLANET},
		};

		internal static void AddMixingDestinations(List<DataImport.spaceDestinations> mixingDestinations, List<Dlc> mixingDlcs)
		{
			foreach (var dlc in mixingDlcs)
			{
				mixingDestinations.AddRange(dlc.extraSpaceDestinationsBasegame);
			}
		}
	}
}
