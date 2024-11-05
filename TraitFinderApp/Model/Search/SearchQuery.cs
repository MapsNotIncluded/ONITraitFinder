
using System.Collections.Generic;
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

        public int CurrentQuerySeed = 1;

        int QueryTarget = 5;

        #region dlc

        public void ToggleDlc(Dlc dlc)
        {
            SetDlcEnabled(dlc, !IsDlcSelected(dlc));
        }

        public void SetDlcEnabled(Dlc dlc, bool enable)
        {

            if (dlc.IsMainVersion)
                ActiveMode = null;
            if (dlc.IsMainVersion || (SelectedCluster?.RequiredDlcs.Contains(dlc) ?? false) || (SelectedCluster?.ForbiddenDlcs.Contains(dlc) ?? false))
                SelectedCluster = null;

            if (enable && !IsDlcSelected(dlc))
            {
                if (dlc.IsMainVersion)
                {
                    DataImport.ImportGameData(dlc == Dlc.SPACEDOUT);

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
        }
        public bool IsModeSelected(ClusterCategory mode) => ActiveMode == mode;
        public bool AnyModeSelected() => ActiveMode != null;

        public void SelectCluster(ClusterLayout cluster)
        {
            SelectedCluster = cluster;
            InitializeAsteroidQueryParams();
        }

        private void InitializeAsteroidQueryParams()
        {
            ResetFilters();
            AsteroidParams = new(SelectedCluster.WorldPlacements.Count);
            bool hasFixedCoordinate = SelectedCluster.HasFixedCoordinate();
            int fixedCoordinate = SelectedCluster.fixedCoordinate;

            for (int i = 0; i < SelectedCluster.WorldPlacements.Count; i++)
            {
                var asteroid = SelectedCluster.WorldPlacements[i].Asteroid;

                AsteroidParams.Add(asteroid, new(asteroid, i));

            }
            if (hasFixedCoordinate)
            {
                PrefillFixedTraits(fixedCoordinate);
            }
        }

        public void StartSearching()
        {
            DataImport.FetchSeeds(this, CurrentQuerySeed, QueryTarget, 5000);
        }
        public void ClearQueryResults()=> QueryResults = new List<QueryResult>(QueryTarget);

        public void ResetFilters()
        {
            ClearQueryResults();
            if (SelectedCluster != null && SelectedCluster.HasFixedCoordinate())
                return;
            if (AsteroidParams != null)
            {
                foreach (var item in AsteroidParams)
                {
                    item.Value.ResetAll();
                }
            }
            ResetQuerySeed();
        }
        public void ResetQuerySeed() => CurrentQuerySeed = 1;

        public void PrefillFixedTraits(int seed)
        {
            for (int i = 0; i < SelectedCluster.WorldPlacements.Count; i++)
            {
                var asteroid = SelectedCluster.WorldPlacements[i].Asteroid;

                var traits = DataImport.GetAsteroidTraitsForSeed(asteroid, seed + i);
                AsteroidParams[asteroid].Guarantee = traits;
            }
        }

        public bool IsClusterSelected(ClusterLayout cluster) => SelectedCluster == cluster;
        public bool AnyClusterSelected() => SelectedCluster != null;


        public bool AsteroidHasTraitGuaranteed(Asteroid asteroid, WorldTrait trait)
        {
            if(AsteroidParams.TryGetValue(asteroid, out var query))
            {
                return query.Guarantee.Contains(trait);
            }
            return false;
        }

        
        public void AddQueryResults(IEnumerable<QueryResult> results, int finalSeed)
        {
            CurrentQuerySeed = finalSeed;

            List<QueryResult> currentlist = QueryResults.ToList();
            currentlist.AddRange(results);
            currentlist.OrderBy(entry => entry.seed);
            List<QueryResult> newList = new();
            while (newList.Count < QueryTarget && currentlist.Any())
            {
                newList.Insert(0,currentlist.Last());
                currentlist.Remove(currentlist.Last());

            }
            QueryResults = (newList); //or QueryResults.Concat
        }
    }
}
