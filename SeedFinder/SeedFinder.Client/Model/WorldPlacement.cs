namespace SeedFinder.Client.Model
{
    public class WorldPlacement
    {
        public Asteroid Asteroid;
        //json mapping
        public string world;
        //public WorldMixing WorldMixingRules;

        public WorldPlacement() { }
        public WorldPlacement(Asteroid asteroid)
        {
            Asteroid = asteroid;
            world = asteroid.Id;
        }
    }
}
