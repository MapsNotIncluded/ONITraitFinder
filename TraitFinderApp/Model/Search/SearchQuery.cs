
using OniStarmapGenerator.Model;
using OniStarmapGenerator.Model.Search;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TraitFinderApp.Model.Mixing;
using TraitFinderApp.Model.Search;

namespace TraitFinderApp.Client.Model.Search
{
	public class SearchQuery
	{
		public List<Dlc> ActiveDlcs = new List<Dlc>();
		public ClusterCategory? ActiveMode;
		public ClusterLayout? SelectedCluster;

		public Dictionary<Asteroid, AsteroidQuery> AsteroidParams;

		public IEnumerable<QueryResult> QueryResults = new HashSet<QueryResult>(32);
		public bool HasResults() => QueryResults != null && QueryResults.Any();

		public bool HasFilters() => AsteroidParams != null && AsteroidParams.Any(param => param.Value.HasFilters()) || RequiredStarmapLocations.Any();
		public bool HasStarmapFilters() => RequiredStarmapLocations != null && RequiredStarmapLocations.Any();


		public IEnumerable<VanillaStarmapLocation> RequiredStarmapLocations
		{
			get => _requiredStarmapLocations; set
			{
				_requiredStarmapLocations = value;
			}
		}
		private IEnumerable<VanillaStarmapLocation> _requiredStarmapLocations = new HashSet<VanillaStarmapLocation>(16);
		
		public int CurrentQuerySeed = 1;

		public int QueryTarget = 5;

		public SearchQuery()
		{
			ActiveDlcs = Dlc.Values.Where(dlc => !dlc.IsMainVersion).ToList();
			MixingHandler.DlcMixingSettings.ForEach(mix => mix.ForceEnabledState(true));
			ResetFilters();
			ResetQuerySeed();
		}

		#region dlc

		public void ToggleDlc(Dlc dlc)
		{
			SetDlcEnabled(dlc, !IsDlcSelected(dlc));
		}

		public void SetDlcEnabled(Dlc dlc, bool enable)
		{
			if (dlc.IsMainVersion)
				ActiveMode = null;
			if (dlc.IsMainVersion || (SelectedCluster?.RequiredDlcs.Contains(dlc) ?? false && !enable) || (SelectedCluster?.ForbiddenDlcs.Contains(dlc) ?? false && enable))
				SelectedCluster = null;

			if (enable && !IsDlcSelected(dlc))
			{
				if (dlc.IsMainVersion)
				{
					DataImport.SetActiveVersion(dlc);

					ActiveMode = null;
					if (!ActiveDlcs.Contains(dlc))
					{
						ActiveDlcs.RemoveAll(d => d.IsMainVersion);
					}
				}
				ActiveDlcs.Add(dlc);
			}
			else if (IsDlcSelected(dlc))
			{
				ActiveDlcs.Remove(dlc);
			}
		}
		public bool IsDlcSelected(Dlc dLC) => ActiveDlcs.Contains(dLC);
		public bool MainVersionSelected() => ActiveDlcs.Any(dlc => dlc.IsMainVersion);
		#endregion

		public void SelectMode(ClusterCategory mode)
		{
			ActiveMode = mode;
			SelectedCluster = null;
			AsteroidParams = new(16);
			ClearQueryResults();
		}
		public bool IsModeSelected(ClusterCategory mode) => ActiveMode == mode;
		public bool AnyModeSelected() => ActiveMode != null;

		public void SelectCluster(ClusterLayout cluster)
		{
			SelectedCluster = cluster;
			InitializeMixingsForCluster();
			InitializeAsteroidQueryParams();
		}
		public bool HasClusterSelected()
		{
			return SelectedCluster != null;
		}
		public bool SpacedOutSelected() => IsDlcSelected(Dlc.SPACEDOUT);

		private void InitializeAsteroidQueryParams()
		{
			ResetFilters();
			if (SelectedCluster == null)
				return;

			AsteroidParams = new(SelectedCluster.WorldPlacements.Count);
			bool hasFixedCoordinate = SelectedCluster.HasFixedCoordinate();
			int fixedCoordinate = SelectedCluster.fixedCoordinate;

			for (int i = 0; i < SelectedCluster.WorldPlacements.Count; i++)
			{
				var asteroid = SelectedCluster.WorldPlacements[i].Asteroid;

				AsteroidParams.Add(asteroid, new AsteroidQuery(this, asteroid, i));

			}
			if (hasFixedCoordinate)
			{
				PrefillFixedTraits(fixedCoordinate);
			}
		}
		public async Task SearchFixedSeed(int seed)
		{
			await DataImport.FetchSeeds(this, seed, 1, 1);
		}
		public async Task StartSearching()
		{
			await DataImport.FetchSeeds(this, CurrentQuerySeed, QueryTarget, 5000);
		}
		public void ClearQueryResults()
		{
			QueryResults = new List<QueryResult>(QueryTarget);
			ResetQuerySeed();
		}

		public void ResetFilters()
		{
			RequiredStarmapLocations = new HashSet<VanillaStarmapLocation>(16);
			ClearQueryResults();
			if (SelectedCluster == null)
				return;
			if (SelectedCluster != null && SelectedCluster.HasFixedCoordinate())
				return;
			if (AsteroidParams != null)
			{
				foreach (var item in AsteroidParams)
				{
					item.Value.ResetAll();
				}
			}
		}
		public void ResetQuerySeed()
		{
			if (SelectedCluster != null && SelectedCluster.HasFixedCoordinate())
				return;
			CurrentQuerySeed = 1;
		}

		public void PrefillFixedTraits(int seed)
		{
			if (SelectedCluster == null)
				return;

			for (int i = 0; i < SelectedCluster.WorldPlacements.Count; i++)
			{
				var asteroid = SelectedCluster.WorldPlacements[i].Asteroid;

				var traits = DataImport.GetRandomTraits(seed + i, asteroid);
				AsteroidParams[asteroid].Guarantee = traits;
			}
		}

		public bool IsClusterSelected(ClusterLayout cluster) => SelectedCluster == cluster;
		public bool AnyClusterSelected() => SelectedCluster != null;


		public bool AsteroidHasTraitGuaranteed(Asteroid asteroid, WorldTrait trait)
		{
			if (AsteroidParams.TryGetValue(asteroid, out var query))
			{
				return query.Guarantee.Contains(trait);
			}
			return false;
		}
		public void InitializeMixingsForCluster()
		{
			if (SelectedCluster == null)
				return;

			foreach (var dlc in SelectedCluster.RequiredDlcs)
			{
				if (MixingHandler.DlcMixingSettingsDict.TryGetValue(dlc, out var mixing) && !IsMixingEnabled(mixing))
				{
					SetMixingEnabled(mixing, true);
				}
			}
			MixingHandler.SetMixingStateWhere(mixing => !IsMixingAllowedByCluster(mixing), false);
		}
		public void ReevaluateMixingsOnChanged(MixingSettingConfig changedMixing, SettingLevel level)
		{
			changedMixing.CurrentLevel = level;

			bool isEnabled = IsMixingEnabled(changedMixing);
			if (changedMixing.IsDlcMixing())
			{
				if (changedMixing.IsActive())
				{
					SetDlcEnabled(changedMixing.DlcFrom, true);
				}
				else
				{
					SetDlcEnabled(changedMixing.DlcFrom, false); 
					MixingHandler.SetMixingStateWhere(mixing => (mixing != changedMixing && mixing.DlcFrom == changedMixing.DlcFrom), false);

				}
			}
		}


		public void SetMixingEnabled(MixingSettingConfig mixingToToggle, bool enabled)
		{
			if(!IsMixingAllowedByCluster(mixingToToggle))
			{
				Console.WriteLine($"Mixing {mixingToToggle.Name} is not allowed in the current cluster selection.");
				return;
			}
			if (enabled)
			{
				mixingToToggle.ForceEnabledState(true);
				if (mixingToToggle.IsDlcMixing() && !IsDlcSelected(mixingToToggle.DlcFrom))
				{
					if (!ActiveDlcs.Contains(mixingToToggle.DlcFrom))
					{
						ActiveDlcs.Add(mixingToToggle.DlcFrom);
					}
				}				
			}
			else
			{
				mixingToToggle.ForceEnabledState(false);
				if (mixingToToggle.IsDlcMixing())
				{
					SetDlcEnabled(mixingToToggle.DlcFrom, false);

					MixingHandler.SetMixingStateWhere(mixing => (mixing.DlcFrom == mixingToToggle.DlcFrom), false);					
				}
			}
		}
		public bool CanToggleMixing(MixingSettingConfig mixing)
		{
			if (mixing.SettingType == MixingType.DLC)
				return !IsDlcRequiredByCluster(mixing.DlcFrom);

			return ActiveDlcs.Contains(mixing.DlcFrom) && IsMixingAllowedByCluster(mixing);
		}
		public bool IsMixingEnabled(MixingSettingConfig mixing) => mixing.IsActive();
		
		public bool IsDlcRequiredByCluster(Dlc dlc)
		{
			if (SelectedCluster != null)
				return SelectedCluster.RequiredDlcs.Contains(dlc);
			return false;
		}


		public bool IsMixingAllowedByCluster(MixingSettingConfig mixing)
		{
			if (mixing.SettingType == MixingType.World && !SpacedOutSelected())
				return false;
			if (mixing.SettingType == MixingType.World && (SelectedCluster?.RequiredDlcs.Contains(mixing.DlcFrom)??false))
				return false;

			var forbiddenTags = mixing.ForbiddenClusterTags?.ToHashSet() ?? new HashSet<string>();

			if (mixing.SettingType == MixingType.Subworld && SelectedCluster != null && SelectedCluster.ClusterTags != null)
			{
				foreach (var clusterTags in SelectedCluster.ClusterTags)
				{
					if (forbiddenTags.Contains(clusterTags))
						return false;
				}
			}
			return true;
		}


		public void AddQueryResults(IEnumerable<QueryResult> results, int finalSeed)
		{
			CurrentQuerySeed = finalSeed;

			List<QueryResult> currentlist = QueryResults.ToList();
			currentlist.AddRange(results);
			currentlist = currentlist.OrderBy(entry => entry.seed).ToList();
			while (currentlist.Count > QueryTarget)
			{
				currentlist.RemoveAt(0);
			}
			QueryResults = (currentlist); //or QueryResults.Concat
		}

		public int GetTotalQueryParams(Asteroid asteroid)
		{
			int total = 0;
			if (AsteroidParams.TryGetValue(asteroid, out var query))
			{
				total += query.Guarantee.Count();
				total += query.Prohibit.Count();
			}
			else
			{
				Console.WriteLine(asteroid.Name + " was not found in query???");
			}

			return total;
		}
	}
}
