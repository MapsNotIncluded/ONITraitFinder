using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TraitFinderApp
{
    public static class LocalStorageHelper
    {
		public static async ValueTask<T?> GetItemAsyncWithConsent<T>(this Blazored.LocalStorage.ILocalStorageService service, string key)
		{
			if (!LocalStorageConsentGiven)
				return await new ValueTask<T?>(default(T));

			return await service.GetItemAsync<T>(key);
		}
		public static async Task SetItemAsyncWithConsent<T>(this Blazored.LocalStorage.ILocalStorageService service, string key, T value)
		{
			if (!LocalStorageConsentGiven)
				return;

			await service.SetItemAsync<T>(key,value);
		}

		public static async Task FetchConsent(this Blazored.LocalStorage.ILocalStorageService service)
		{
			LocalStorageConsentGiven = await service.GetItemAsync<bool>(LocalStorageConsentGivenKey);
		}

		public static async Task SetConsentGiven(this Blazored.LocalStorage.ILocalStorageService service) => await service.SetItemAsync<bool>(LocalStorageConsentGivenKey, true);

		public const string LocalStorageConsentGivenKey = "MNI_LocalStorageConsent";
		public static bool LocalStorageConsentGiven = false;

		public const string HideTraitlessKey = "MNI_Traitfinder_HideTraitless";
        public const string PersistantTraitOrderingKey = "MNI_Traitfinder_OrderTraitsPersistant";
        public const string HideLocationlessDistancesKey = "MNI_Traitfinder_HideLocationlessStarmapBands";
        public const string MaxSeedNumberPerSearchKey = "MNI_Traitfinder_MaxSeedNumberPerSearch";

		public const string SavedMixingsKey = "MNI_Traitfinder_SavedMixings";
		public const string SavedSettingsKey = "MNI_Traitfinder_SavedSettings";
		public const string SavedStoryTraitsKey = "MNI_Traitfinder_SavedStoryTraits";




		public static bool DarkThemeActive = true;
        public static bool UsePersistentTraitOrdering = false;
        public static bool HideTraitlessAsteroids = false;
        public static bool HideLocationlessDistances = false;
        public static int MaxSeedNumberPerSearch = 5000;

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
