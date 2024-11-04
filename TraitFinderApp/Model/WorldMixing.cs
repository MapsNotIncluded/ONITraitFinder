namespace TraitFinderApp.Client.Model
{
    public class WorldMixing
    {
        public List<string> requiredTags { get; set; }

        public List<string> forbiddenTags { get; set; }


        public WorldMixing()
        {
            requiredTags = new List<string>();
            forbiddenTags = new List<string>();
        }
    }
}
