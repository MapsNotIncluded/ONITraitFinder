
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

        public bool HasFilters() => AsteroidParams!=null&& AsteroidParams.Any(param => param.Value.HasFilters());

        public int CurrentQuerySeed = 1;

        public int QueryTarget = 5;

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
            AsteroidParams = new(0);
            ClearQueryResults();
        }
        public bool IsModeSelected(ClusterCategory mode) => ActiveMode == mode;
        public bool AnyModeSelected() => ActiveMode != null;

        public void SelectCluster(ClusterLayout cluster)
        {
            SelectedCluster = cluster;
            InitializeAsteroidQueryParams();
        }
        public bool HasClusterSelected()
        {
            return SelectedCluster != null;
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

                AsteroidParams.Add(asteroid, new AsteroidQuery(this,asteroid, i));

            }
            if (hasFixedCoordinate)
            {
                PrefillFixedTraits(fixedCoordinate);
            }
        }

        public async Task StartSearching()
        {
            await DataImport.FetchSeeds(this, CurrentQuerySeed, QueryTarget, 10000);
        }
        public void ClearQueryResults()
        {
            QueryResults = new List<QueryResult>(QueryTarget);
            ResetQuerySeed();
        }

        public void ResetFilters()
        {
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
            if(AsteroidParams.TryGetValue(asteroid,out var query))
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
