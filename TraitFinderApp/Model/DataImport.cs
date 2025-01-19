using System.Diagnostics;
using System;
using TraitFinderApp.Client.Model.KleiClasses;
using TraitFinderApp.Client.Model.Search;
using TraitFinderApp.Model.Search;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using static MudBlazor.CategoryTypes;
using Newtonsoft.Json;
using OniStarmapGenerator.Model.Search;
using TraitFinderApp.Model.KleiClasses;
using OniStarmapGenerator.Model;
using static MudBlazor.Icons.Custom;

namespace TraitFinderApp.Client.Model
{
    public class Data
    {
        public Data() { }

        public List<ClusterLayout> clusters { get; set; }
        public List<Asteroid> asteroids { get; set; }
        public List<WorldTrait> worldTraits { get; set; }

        public Dictionary<string, ClusterLayout> clustersDict = new();
        public Dictionary<string, Asteroid> asteroidsDict = new();
        public Dictionary<string, WorldTrait> worldTraitsDict = new();

        private Dictionary<Asteroid, List<WorldTrait>>? _compatibleTraits = null;

        public bool MapGameData()
        {
            asteroidsDict = new();
            Console.WriteLine(asteroids.Count + " asteroids");
            foreach (var asteroid in asteroids)
            {
                asteroidsDict[asteroid.Id] = asteroid;
                asteroid.InitBindings(this);
            }
            Console.WriteLine(asteroids.Count + " asteroids initialized");

            clustersDict = new();
            Console.WriteLine(clusters.Count + " clusters");
            foreach (var cluster in clusters)
            {
                clustersDict[cluster.Id] = cluster;
                cluster.InitBindings(this);
            }
            Console.WriteLine(clusters.Count + " clusters initialized");
            worldTraitsDict = new();
            Console.WriteLine(worldTraitsDict.Count + " worldTraits");
            foreach (var trait in worldTraits)
            {
                worldTraitsDict[trait.Id] = trait;
            }
            Console.WriteLine(worldTraitsDict.Count + " worldTraits initialized");
            CalculateTraitCompatibilities();
            return true;
        }

        public void CalculateTraitCompatibilities()
        {
            _compatibleTraits = new();
            foreach (var asteroid in asteroids)
            {
                var allTraits = new List<WorldTrait>(worldTraits);
                if (asteroid.DisableWorldTraits)
                {
                    _compatibleTraits[asteroid] = new();
                    continue;
                }

                List<string> ExclusiveWithTags = new List<string>();

                if (asteroid.TraitRules != null)
                {
                    foreach (var rule in asteroid.TraitRules)
                    {
                        TagSet? requiredTags = (rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null;
                        TagSet? forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

                        allTraits.RemoveAll((WorldTrait trait) =>
                              (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
                            || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
                            || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.Id)));
                    }
                }
                allTraits.RemoveAll((WorldTrait trait) => !trait.IsValid(asteroid, logErrors: true));
                _compatibleTraits[asteroid] = allTraits;
            }
        }

        public List<WorldTrait> GetCompatibleTraits(Asteroid asteroid)
        {
            if (_compatibleTraits == null)
                return new();
            return _compatibleTraits[asteroid];
        }
    }
    public class StarmapData
    {
        public Dictionary<string, VanillaStarmapLocation> Locations;
        public void MapGameData()
        {
            Locations.Remove("SaltDesertPlanet"); //not found in starmap gen, disable it
		}

    }

    public class DataImport
    {
        public static StarmapData StarmapImport;
        public static List<VanillaStarmapLocation> GetVanillaStarmapLocations()
        {
            return StarmapImport.Locations.Values.Where(e=>!e.Disabled).ToList();
        }


        public static Data BaseGame;
        public static Data SpacedOut;

        internal static async Task FetchSeeds(SearchQuery searchQuery, int startSeed, int targetCount = 4, int seedRange = 2000)
        {
            var cluster = searchQuery.SelectedCluster;
            bool checkForStarmap = searchQuery.HasStarmapFilters();
            bool isBaseGame = searchQuery.ActiveDlcs.Contains(Dlc.BASEGAME);


            var asteroids = new Tuple<Asteroid, int>[cluster.WorldPlacements.Count];
            var dlcs = searchQuery.ActiveDlcs;

            List<QueryResult> results = new List<QueryResult>(targetCount);

            for (int i = 0; i < cluster.WorldPlacements.Count; i++)
            {
                //0 seed always generates all asteroids with no traits
                int offsetIndex = (startSeed > 0) ? i : 0;

                asteroids[i] = (new(cluster.WorldPlacements[i].Asteroid, offsetIndex));
            }
            int queryableRange = startSeed + seedRange;

            int asteroidCount = asteroids.Length;

            //check asteroids first that have forbidden traits
            var asteroidsSortedByFilterCount = asteroids.OrderByDescending(searchEntry => searchQuery.GetTotalQueryParams(searchEntry.Item1)).ToArray();

            Dictionary<Asteroid, AsteroidQuery> queryParams = new(searchQuery.AsteroidParams);

            Dictionary<Asteroid, List<WorldTrait>> TraitStorage = new(asteroidCount);

            List<SpaceDestination> destinations = new();

            while (startSeed < queryableRange && results.Count < targetCount)
            {
                int localSeed = 0;
                bool seedFailedQuery = false;
                //Console.Write("Checking seed: "+startSeed);
                if (isBaseGame)
                {
                    destinations = GenerateRandomDestinations(startSeed, dlcs);
                    if (checkForStarmap)
					{
						HashSet<VanillaStarmapLocation> requiredStarmapLocations = new(searchQuery.RequiredStarmapLocations);
						for (int i = 0; i < destinations.Count; i++)
                        {
                            var destination = destinations[i];
                            requiredStarmapLocations.Remove(destination.Type);
                            if (!requiredStarmapLocations.Any())
                                break;
                        }
                        if (requiredStarmapLocations.Any())
                            seedFailedQuery = true;
                    }

                }
                if (!seedFailedQuery)
                {
                    for (int i = 0; i < asteroidCount; ++i)
                    {
                        var asteroidWithOffset = asteroidsSortedByFilterCount[i];
                        var asteroid = asteroidWithOffset.Item1;
                        localSeed = asteroidWithOffset.Item2;
                        var traits = GetRandomTraits(startSeed + localSeed, asteroid);
                        TraitStorage[asteroid] = traits;

                        if (queryParams.TryGetValue(asteroid, out var asteroidParams))
                        {
                            bool hasProhibited = asteroidParams.Prohibit.Any();
                            bool hasGuaranteed = asteroidParams.Guarantee.Any();
                            if (
                                 //asteroid had prohibited traits
                                 hasProhibited && asteroidParams.Prohibit.Intersect(traits).Any()
                                //not all guaranteed traits are in asteroid
                                || hasGuaranteed && asteroidParams.Guarantee.Except(traits).Any()
                                )
                            {

                                seedFailedQuery = true;
                                break;
                            }
                        }
                    }
                }
                if (!seedFailedQuery) //some asteroids were canceled, checking next
                {
                    var asteroidQueryResults = new List<QueryAsteroidResult>(asteroidCount);

                    foreach (var asteroidWithIndex in asteroids)
                    {
                        var asteroid = asteroidWithIndex.Item1;
                        if (TraitStorage.TryGetValue(asteroid, out var traitResults))
                            asteroidQueryResults.Add(new(searchQuery, asteroid, new(traitResults)));
                    }
                    var queryResult = new QueryResult()
                    {
                        seed = startSeed,
                        cluster = cluster,
                        asteroidsWithTraits = asteroidQueryResults
                    };
                    if (isBaseGame)
                    {
                        int maxDistance = destinations.Select(x => x.distance).Max();


                        var bands = new DistanceBand[maxDistance + 1];
                        for (int i = 0; i < bands.Length; i++)
                        {
                            bands[i] = new(i);
                        }


                        foreach (var entry in destinations)
                        {
                            bands[entry.distance].Destinations.Add(entry.Type);
                        }
                        queryResult.distanceBands = bands.ToList();
                    }
                    results.Add(queryResult);
                }
                ++startSeed;
            }
            searchQuery.AddQueryResults(results, startSeed);
        }

        private static List<SpaceDestination> GenerateFixedDestinations()
        {
            return new List<SpaceDestination>()
            {
                new SpaceDestination(0, "CarbonaceousAsteroid", 0),
                new SpaceDestination(1, "CarbonaceousAsteroid", 0),
                new SpaceDestination(2, "MetallicAsteroid", 1),
                new SpaceDestination(3, "RockyAsteroid", 2),
                new SpaceDestination(4, "IcyDwarf", 3),
                new SpaceDestination(5, "OrganicDwarf", 4)
            };
        }

        public class spaceDesinations
        {
            public string type;
            public int minTier, maxTier;
        }

        public static spaceDesinations ceresBaseGameExtraDestionation = new()
        {
            type = "DLC2CeresSpaceDestination",
            minTier = 3,
            maxTier = 10
        };



        private static List<SpaceDestination> GenerateRandomDestinations(int seed, List<Dlc> mixingDlcs)
        {
            var destinations = GenerateFixedDestinations();


            KRandom rng = new KRandom(seed);
            List<List<string>> stringListList = new List<List<string>>()
            {
              new List<string>(),
              new List<string>() { "OilyAsteriod" },
              new List<string>() { "Satellite" },
              new List<string>()
              {
                "Satellite",
                "RockyAsteroid",
                "CarbonaceousAsteroid",
                "ForestPlanet"
              },
              new List<string>()
              {
                "MetallicAsteroid",
                "RockyAsteroid",
                "CarbonaceousAsteroid",
                "SaltDwarf"
              },
              new List<string>()
              {
                "MetallicAsteroid",
                "RockyAsteroid",
                "CarbonaceousAsteroid",
                "IcyDwarf",
                "OrganicDwarf"
              },
              new List<string>()
              {
                "IcyDwarf",
                "OrganicDwarf",
                "DustyMoon",
                "ChlorinePlanet",
                "RedDwarf"
              },
              new List<string>()
              {
                "DustyMoon",
                "TerraPlanet",
                "VolcanoPlanet"
              },
              new List<string>()
              {
                "TerraPlanet",
                "GasGiant",
                "IceGiant",
                "RustPlanet"
              },
              new List<string>()
              {
                "GasGiant",
                "IceGiant",
                "HeliumGiant"
              },
              new List<string>()
              {
                "RustPlanet",
                "VolcanoPlanet",
                "RockyAsteroid",
                "TerraPlanet",
                "MetallicAsteroid"
              },
              new List<string>()
              {
                "ShinyPlanet",
                "MetallicAsteroid",
                "RockyAsteroid"
              },
              new List<string>()
              {
                "GoldAsteroid",
                "OrganicDwarf",
                "ForestPlanet",
                "ChlorinePlanet"
              },
              new List<string>()
              {
                "IcyDwarf",
                "MetallicAsteroid",
                "DustyMoon",
                "VolcanoPlanet",
                "IceGiant"
              },
              new List<string>()
              {
                "ShinyPlanet",
                "RedDwarf",
                "RockyAsteroid",
                "GasGiant"
              },
              new List<string>()
              {
                "HeliumGiant",
                "ForestPlanet",
                "OilyAsteriod"
              },
              new List<string>()
              {
                "GoldAsteroid",
                "SaltDwarf",
                "TerraPlanet",
                "VolcanoPlanet"
              }
            };
            List<int> list = new List<int>();
            int num1 = 3;
            int minValue = 15;
            int maxValue = 25;
            for (int index1 = 0; index1 < stringListList.Count; ++index1)
            {
                if (stringListList[index1].Count != 0)
                {
                    for (int index2 = 0; index2 < num1; ++index2)
                        list.Add(index1);
                }
            }
            int nextId = destinations.Count;
            int num2 = rng.Next(minValue, maxValue);
            List<SpaceDestination> collection1 = new List<SpaceDestination>();
            for (int index3 = 0; index3 < num2; ++index3)
            {
                int index4 = rng.Next(0, list.Count - 1);
                int num3 = list[index4];
                list.RemoveAt(index4);
                List<string> stringList = stringListList[num3];
                string type = stringList[rng.Next(0, stringList.Count)];
                SpaceDestination spaceDestination = new SpaceDestination(GetNextID(), type, num3);
                collection1.Add(spaceDestination);

            }
            list.ShuffleSeeded(rng);
            List<SpaceDestination> collection2 = new List<SpaceDestination>();
            var mixingDestinations = new List<spaceDesinations>();
            if (mixingDlcs.Contains(Dlc.BASEGAME) && mixingDlcs.Contains(Dlc.FROSTYPLANET))
            {
                mixingDestinations.Add(ceresBaseGameExtraDestionation);
            }
            foreach (var spaceDesination in mixingDestinations)
            {
                bool flag = false;
                if (list.Count > 0)
                {
                    for (int index = 0; index < list.Count; ++index)
                    {
                        int distance = list[index];
                        if (distance >= spaceDesination.minTier && distance <= spaceDesination.maxTier)
                        {
                            SpaceDestination spaceDestination = new SpaceDestination(GetNextID(), spaceDesination.type, distance);
                            collection2.Add(spaceDestination);
                            list.RemoveAt(index);
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    for (int index = 0; index < collection1.Count; ++index)
                    {
                        SpaceDestination spaceDestination = collection1[index];
                        if (spaceDestination.distance >= spaceDesination.minTier && spaceDestination.distance <= spaceDesination.maxTier)
                        {
                            collection1[index] = new SpaceDestination(GetNextID(), spaceDesination.type, spaceDestination.distance);


                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    Console.WriteLine("error while placing the mixing destination");
                }
            }
            destinations.AddRange(collection1);
            destinations.Add(new SpaceDestination(GetNextID(), "Earth", 4));
            destinations.Add(new SpaceDestination(GetNextID(), "Wormhole", stringListList.Count));
            destinations.AddRange(collection2);



            return destinations;

            int GetNextID() => nextId++;


        }

        public static List<WorldTrait> GetRandomTraits(int seed, Asteroid world)
        {
            if (world.DisableWorldTraits || world.TraitRules == null || seed == 0)
            {
                return new List<WorldTrait>();
            }
            var worldTraits = GetActive().worldTraitsDict;


            KRandom kRandom = new KRandom(seed);
            List<WorldTrait> allTraits = new List<WorldTrait>(worldTraits.Values);
            List<WorldTrait> result = new List<WorldTrait>();
            TagSet tagSet = new TagSet();
            var rule = world.TraitRule();

            if (rule.specificTraits != null)
            {
                foreach (string specificTrait in rule.specificTraits)
                {
                    result.Add(worldTraits[specificTrait]);
                }
            }

            List<WorldTrait> allTraitsLocal = new List<WorldTrait>(allTraits);
            TagSet requiredTags = ((rule.requiredTags != null) ? new TagSet(rule.requiredTags) : null);
            TagSet forbiddenTags = ((rule.forbiddenTags != null) ? new TagSet(rule.forbiddenTags) : null);

            allTraitsLocal.RemoveAll((WorldTrait trait) =>
            (requiredTags != null && !trait.traitTagsSet.ContainsAll(requiredTags))
            || (forbiddenTags != null && trait.traitTagsSet.ContainsOne(forbiddenTags))
            || (rule.forbiddenTraits != null && rule.forbiddenTraits.Contains(trait.Id))
            || !trait.IsValid(world, logErrors: true));

            int randomNumber = kRandom.Next(rule.min, Math.Max(rule.min, rule.max + 1));
            int count = result.Count;
            while (result.Count < count + randomNumber && allTraitsLocal.Count > 0)
            {
                int index = kRandom.Next(allTraitsLocal.Count);
                WorldTrait worldTrait = allTraitsLocal[index];
                bool flag = false;
                foreach (string exclusiveId in worldTrait.exclusiveWith)
                {
                    if (result.Find((WorldTrait t) => t.Id == exclusiveId) != null)
                    {
                        flag = true;
                        break;
                    }
                }
                foreach (string exclusiveWithTag in worldTrait.exclusiveWithTags)
                {
                    if (tagSet.Contains(exclusiveWithTag))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    result.Add(worldTrait);
                    allTraits.Remove(worldTrait);
                    foreach (string exclusiveWithTag2 in worldTrait.exclusiveWithTags)
                    {
                        tagSet.Add(exclusiveWithTag2);
                    }
                }

                allTraitsLocal.RemoveAt(index);
            }

            if (result.Count != count + randomNumber)
            {
                Debug.Fail($"TraitRule on {world.Name} tried to generate {randomNumber} but only generated {result.Count - count}");
            }
            return result;
        }

        internal static void SetActiveVersion(Dlc version)
        {
            if (version == Dlc.SPACEDOUT)
            {
                active = SpacedOut;
            }
            else
            {
                active = BaseGame;
            }
        }
        static Data active = BaseGame;
        internal static Data GetActive()
        {
            return active;
        }
    }
}
