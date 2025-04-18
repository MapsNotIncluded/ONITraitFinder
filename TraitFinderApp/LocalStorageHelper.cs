using System.Xml.Serialization;

namespace TraitFinderApp
{
    public static class LocalStorageHelper
    {
        public const string HideTraitlessKey = "MNI_Traitfinder_HideTraitless";
        public const string PersistantTraitOrderingKey = "MNI_Traitfinder_OrderTraitsPersistant";
        public const string HideLocationlessDistancesKey = "MNI_Traitfinder_HideLocationlessStarmapBands";
        public static bool DarkThemeActive = true;
        public static bool UsePersistentTraitOrdering = false;
        public static bool HideTraitlessAsteroids = false;
        public static bool HideLocationlessDistances = false;

        public static string EmbbeddedIn = null;
        public static string MNI_Token = null;

        public static bool TryGetMNIToken(out string token)
		{
			token = MNI_Token;
			return !string.IsNullOrEmpty(token);
		}
        public static bool IsEmbeddedIn(out string pageEmbedd)
        {
			pageEmbedd = EmbbeddedIn;
			return !string.IsNullOrEmpty(pageEmbedd);
		}

	}
}
