using System.Diagnostics;
using System;
using TraitFinderApp.Client.Model.KleiClasses;
using TraitFinderApp.Client.Model.Search;
using TraitFinderApp.Model.Search;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using static MudBlazor.CategoryTypes;
using Newtonsoft.Json;

namespace TraitFinderApp.Client.Model
{
    public class Data
    {
        public Data(){}

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

    public class DataImport
    {
        public static Data BaseGame;
        public static Data SpacedOut;

        internal static void FetchSeeds(SearchQuery searchQuery, int startSeed, int targetCount = 4, int seedRange = 2000)
        {
            var cluster = searchQuery.SelectedCluster;

            var asteroids = new Tuple<Asteroid, int>[cluster.WorldPlacements.Count];

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


            while (startSeed < queryableRange && results.Count < targetCount)
            {
                int localSeed = 0;
                bool seedFailedQuery = false;
                //Console.Write("Checking seed: "+startSeed);
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
                if (!seedFailedQuery) //some asteroids were canceled, checking next
                {

                    var asteroidQueryResults = new List<QueryAsteroidResult>(asteroidCount);

                    foreach (var asteroidWithIndex in asteroids)
                    {
                        var asteroid = asteroidWithIndex.Item1;
                        if (TraitStorage.TryGetValue(asteroid, out var traitResults))
                            asteroidQueryResults.Add(new(searchQuery, asteroid, new(traitResults)));
                    }

                    results.Add(new()
                    {
                        seed = startSeed,
                        cluster = cluster,
                        asteroidsWithTraits = asteroidQueryResults
                    });
                }
                ++startSeed;
            }
            searchQuery.AddQueryResults(results, startSeed);
        }


        public static List<WorldTrait> GetRandomTraits(int seed, Asteroid world)
        {
            if (world.DisableWorldTraits || world.TraitRules == null || seed == 0)
            {
                return new List<WorldTrait>();
            }
            var worldTraits = WorldTrait.KeyValues;


            KRandom kRandom = new KRandom(seed);
            List<WorldTrait> allTraits = new List<WorldTrait>(WorldTrait.Values);
            List<WorldTrait> result = new List<WorldTrait>();
            TagSet tagSet = new TagSet();
            var rule = world.TraitRule();

            if (rule.specificTraits != null)
            {
                foreach (string specificTrait in rule.specificTraits)
                {
                    if (worldTraits.TryGetValue(specificTrait, out var _))
                    {
                        result.Add(worldTraits[specificTrait]);
                    }
                    else
                    {
                        Debug.Fail("World traits " + specificTrait + " doesn't exist, skipping.");
                    }
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
