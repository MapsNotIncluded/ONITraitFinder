namespace OniStarmapGenerator.Model
{
    public class VanillaStarmapLocation
    {
        public string Id;
        public string Name;
        public string Description;
        //public string Image;
        public string Image => $"./images/Starmap/starmap_destinations_basegame/{Path.GetFileName(Id)}.png";
        public Dictionary<string, float> Ressources_Elements;
        public Dictionary<string, int> Ressources_Entities;


        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object? obj)
        {
            return obj is VanillaStarmapLocation t && t.GetHashCode() == this.GetHashCode();
        }
        public override string ToString()
        {
            return Name;
        }
    }

}
