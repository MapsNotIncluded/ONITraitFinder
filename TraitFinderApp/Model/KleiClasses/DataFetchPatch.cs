using Microsoft.VisualBasic;

namespace TraitFinderApp.Client.Model.KleiClasses
{
    /// <summary>
    ///  not an actual class, use the commented out part in any oni mod to generate an updated json data export for the DataImport class
    /// </summary>
    public class DataFetchPatch
    {
    //    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnPrefabInit))]
    //    public class MainMenu_OnPrefabInit
    //    {
    //        public class Asteroid
    //        {
    //            public string Id;
    //            public string Name;
    //            public string Image;
    //            public bool DisableWorldTraits = false;
    //            public List<TraitRule> TraitRules;
    //        }
    //        public class ClusterLayout
    //        {
    //            public string Id;
    //            public string Name;
    //            public string Prefix;
    //            public int menuOrder;
    //            public string[] RequiredDlcsIDs;
    //            public string[] ForbiddenDlcIDs;
    //            public List<string> WorldPlacementIDs;
    //            public int clusterCategory;
    //            public int fixedCoordinate;
    //        }
    //        public class DataExport
    //        {
    //            public List<ClusterLayout> clusters = new();
    //            public List<Asteroid> asteroids = new();
    //            public List<WorldTrait> worldTraits = new();

    //        }
    //        public class WorldTrait
    //        {
    //            public string Id;
    //            public string Name, ColorHex;
    //            public List<string> forbiddenDLCIds, exclusiveWith, exclusiveWithTags, traitTags;

    //            public WorldTrait()
    //            {
    //                exclusiveWith = new List<string>();
    //                exclusiveWithTags = new List<string>();
    //                forbiddenDLCIds = new List<string>();
    //                traitTags = new List<string>();
    //                Name = string.Empty;
    //                Id = string.Empty;
    //            }
    //        }


    //        public static void Postfix()
    //        {
    //            var export = new DataExport();
    //            foreach (var cluster in SettingsCache.clusterLayouts.clusterCache.Values)
    //            {
    //                var data = new ClusterLayout();
    //                data.Id = cluster.filePath;
    //                data.Name = Strings.Get(cluster.name);
    //                data.Prefix = cluster.coordinatePrefix;
    //                data.menuOrder = cluster.menuOrder;
    //                data.RequiredDlcsIDs = cluster.requiredDlcIds;
    //                data.ForbiddenDlcIDs = cluster.forbiddenDlcIds;
    //                //data.WorldPlacements = cluster.worldPlacements;
    //                data.WorldPlacementIDs = cluster.worldPlacements.Select(pl => pl.world).ToList();
    //                data.clusterCategory = (int)cluster.clusterCategory;
    //                data.fixedCoordinate = cluster.fixedCoordinate;
    //                export.clusters.Add(data);
    //            }
    //            foreach (var world in SettingsCache.worlds.worldCache.Values)
    //            {
    //                var data = new Asteroid();
    //                data.Id = world.filePath;
    //                data.Name = Strings.Get(world.name);
    //                data.DisableWorldTraits = world.disableWorldTraits;
    //                data.TraitRules = world.worldTraitRules;
    //                export.asteroids.Add(data);
    //            }
    //            foreach (var trait in SettingsCache.worldTraits.Values)
    //            {
    //                var data = new WorldTrait();
    //                data.Id = trait.filePath;
    //                data.Name = Strings.Get(trait.name);
    //                data.ColorHex = trait.colorHex;
    //                data.forbiddenDLCIds = trait.forbiddenDLCIds;
    //                data.exclusiveWith = trait.exclusiveWith;
    //                data.exclusiveWithTags = trait.exclusiveWithTags;
    //                data.traitTags = trait.traitTags;
    //                export.worldTraits.Add(data);
    //            }
    //            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    //            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(export));
    //        }
    //    }
    }
}
