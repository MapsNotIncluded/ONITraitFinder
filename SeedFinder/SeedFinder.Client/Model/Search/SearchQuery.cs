namespace SeedFinder.Client.Model.Search
{
    public class SearchQuery
    {
        public List<Dlc> ActiveDlcs = new List<Dlc>();
        public ClusterCategory? ActiveMode;

        #region dlc

        public void ToggleDlc(Dlc dlc)
        {
            SetDlcEnabled(dlc, !IsDlcSelected(dlc));
            ActiveMode = null;
        }

        public void SetDlcEnabled(Dlc dlc, bool enable)
        {
            if (enable && !IsDlcSelected(dlc))
            {
                if (dlc.IsMainVersion)
                {
                    if (!ActiveDlcs.Contains(dlc))
                    {
                        ActiveDlcs.RemoveAll(d => d.IsMainVersion);
                    }
                }
                ActiveDlcs.Add(dlc);
            }
            else if(IsDlcSelected(dlc))
            {
                ActiveDlcs.Remove(dlc);
            }
        }
        public bool IsDlcSelected(Dlc dLC) => ActiveDlcs.Contains(dLC);
        public bool MainVersionSelected() => ActiveDlcs.Any(dlc => dlc.IsMainVersion);
#endregion

        public void SelectMode(ClusterCategory category) => ActiveMode = category;
        public bool IsModeSelected(ClusterCategory mode) => ActiveMode == mode;
        public bool AnyModeSelected() => ActiveMode != null;

    }
}
