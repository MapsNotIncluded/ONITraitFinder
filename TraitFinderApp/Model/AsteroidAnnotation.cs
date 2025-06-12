using TraitFinderApp.Client.Model;

namespace TraitFinderApp.Model
{
	public class AsteroidAnnotation
	{
		public static List<AsteroidAnnotation> GetAnnotationsFor(Asteroid asteroid, ClusterLayout cluster)
		{
			var annotations = new List<AsteroidAnnotation>();
			bool isStarter = cluster.StarterAsteroid() == asteroid;

			if (isStarter && cluster.RequiredDlcs.Contains(Dlc.SPACEDOUT))
				annotations.Add(StarterAsteroid);

			if (!isStarter && asteroid.SpecialPOIs.Contains("Teleporter"))
				annotations.Add(WarpAsteroid);

			if (asteroid.SpecialPOIs.Contains("GeothermalController"))
				annotations.Add(HeatpumpAsteroid);

			if (asteroid.SpecialPOIs.Contains("LargeImpactor"))
				annotations.Add(DemoliorAsteroid);

			return annotations;
		}

		public static AsteroidAnnotation StarterAsteroid = new("Starter Asteroid", "icons/printingpod.ico");
		public static AsteroidAnnotation WarpAsteroid = new("Teleporter Asteroid", "icons/building_teleporter_transmitter.webp");
		public static AsteroidAnnotation HeatpumpAsteroid = new("Geothermal Heat Pump", "icons/building_geothermal_heat_pump.webp");
		public static AsteroidAnnotation DemoliorAsteroid = new("Demolior Impactor", "icons/potato.webp");


		public AsteroidAnnotation(string tooltip, string icon)
		{
			_tooltip = tooltip;
			_icon = icon;
		}

		string _tooltip = string.Empty;
		string _icon = string.Empty;
		public string GetTooltip() => _tooltip;
		public string GetIcon() => _icon;
	}
}
