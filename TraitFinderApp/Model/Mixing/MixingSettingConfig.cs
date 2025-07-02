
using TraitFinderApp.Client.Model;

namespace TraitFinderApp.Model.Mixing
{
	public class MixingSettingConfig
	{
		public string Id;
		public string Name;
		public string Description;
		public long coordinate_range;
		public string DlcIdFrom;
		public Dlc DlcFrom;
		public string Icon;
		public List<SettingLevel> Levels;
		public string[] MixingTags;
		public string[] ForbiddenClusterTags;
		public string WorldMixing;
		public string SubworldMixing;
		public MixingType SettingType = MixingType.None;

		public SettingLevel CurrentLevel;
		public SettingLevel OnLevel, OffLevel, ThirdLevel; //mixings have either 2 or 3 settings levels
		public bool TwoLevels => OffLevel != null && ThirdLevel == null;

		public SettingLevel GetLevel()
		{
			return CurrentLevel;
		}
		public bool IsDlcMixing() => SettingType == MixingType.DLC;	

		internal void InitBindings()
		{
			DlcFrom = Dlc.KeyValues[DlcIdFrom];
			if (Levels.Count == 3)
				ThirdLevel = Levels[1];
			OnLevel = Levels.Last();
			OffLevel = Levels.First();
			CurrentLevel = OffLevel;
		}
		public bool IsActive() => CurrentLevel != OffLevel;
		public bool IsCurrentLevel(SettingLevel level) => CurrentLevel == level;

		public void ForceEnabledState(bool enabled) => CurrentLevel = (enabled ? OnLevel : OffLevel);

		public string GetIcon()
		{
			if (SettingType == MixingType.DLC)
				return DlcFrom.Image;

			if (SettingType == MixingType.World)
			{
				if (DataImport.SpacedOut?.asteroidsDict?.TryGetValue(WorldMixing, out var asteroid) ?? false)
				{
					return asteroid.Image;
				}
			}
			if (SettingType == MixingType.Subworld)
			{
				return "./images/biomes/" + Icon + ".png";
			}

			return string.Empty;
		}
	}
}
