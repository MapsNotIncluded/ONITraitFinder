using System.Xml.Linq;
using TraitFinderApp.Client.Model;

namespace OniStarmapGenerator.Model.Search
{
    public class SpaceDestination
    {
        public int distance;
		public VanillaStarmapLocation Type;


        public SpaceDestination() 
        {
        
        }

		public SpaceDestination(int id, VanillaStarmapLocation type, int dist)
		{
			//id is unused, its from the game metho
			distance = dist;
			Type = type;
        }
        public SpaceDestination(int id, string typeId, int dist)
        {
            //id is unused, its from the game method
            distance = dist;
            Type = DataImport.StarmapImport.Locations[typeId];
        }
    }
}
