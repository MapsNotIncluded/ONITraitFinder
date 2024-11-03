
namespace SeedFinder.Client.Model.Search
{
    public class SearchQuery
    {
        public List<Dlc> ActiveDlcs = new List<Dlc>();
        public ClusterCategory? ActiveMode;
        public ClusterLayout? SelectedCluster;

        public Dictionary<Asteroid,AsteroidQuery> AsteroidParams;



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
            AsteroidParams = new(SelectedCluster.WorldPlacements.Count);
            for (int i = 0; i < SelectedCluster.WorldPlacements.Count; i++)
            {
                var asteroid = SelectedCluster.WorldPlacements[i].Asteroid;
                AsteroidParams.Add(asteroid,new(asteroid, i));
            }
        }

        public bool IsClusterSelected(ClusterLayout cluster) => SelectedCluster == cluster;
        public bool AnyClusterSelected() => SelectedCluster != null;


        public bool CanAsteroidHaveTrait(Asteroid asteroid, WorldTrait trait)
        {
            return false;
        }
        public bool AsteroidHasTraitQueried(Asteroid asteroid, WorldTrait trait)
        {
            return true;
        }

    }
}
