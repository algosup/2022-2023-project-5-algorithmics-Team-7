using System.Diagnostics;
using WineMixer;

namespace WineMixerTests;

public static class Tests
{
    /*
    [OneTimeSetUp]
    public static void SetUp()
    {
        //Console.SetOut(new DebugWriter());
    }

    public static Mix[] Targets = 
    {
        new Mix(0.1, 0.15, 0.25, 0.5),
        //new Mix(0.12, 0.18, 0.2, 0.3, 0.1, 0.04, 0.06 ),
        //new Mix(0.01, 0.09, 0.3, 0.6),    
        //new Mix(0.063, 0.234, 0.167, 0.075, 0.023, 0.033, 0.084, 0.063, 0.097, 0.083, 0.007, 0.071),
        //new Mix(0.018, 0.072, 0.04, 0.002, 0.008, 0.062, 0.07, 0.022, 0.049, 0.04, 0.006, 0.04, 0.008, 0.011, 0.106, 0.017, 0.012, 0.162, 0.028, 0.042, 0.055, 0.015, 0.002, 0.079, 0.033),
    };

    public static int[][] PossibleTankSizes()
    {
        return new[]
        {
            //new [] { 8, 20, 20, 30, 1, 1, 2, 2, 3, 3, 4, 4, 5, 6, 10, 12, 12, 13, 15, 16, 20, 24, 25, 32 },
            new [] 
            { 20, 20, 20, 20, 
                20, 20, 20, 20,
                1, 1, 2, 2, 3, 3, 4, 
                4, 5, 5, 6, 10, 10, 
                12, 12, 13, 15, 16, 
                20, 24, 25, 25, 30,
                30, 32, 40, 40, 60, 80 },
            //new [] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7 },
            //new [] { 1, 1, 2, 3, 3, 4, 5, 5, 6, 6, 6, 6, 7, 7, 8, 8, 12, 12, 13, 15, 16, 20, 24, 24, 28 },
            //new [] { 1, 1, 2, 3, 5, 5, 6, 8, 10, 11, 12, 13, 15, 18, 20, 25, 23, 25, 25, 28, 30, 35, 40, 50 },
            //Enumerable.Range(0,100).ToArray(),
        };
    }

    public static int TestTankFinder()
    {
        var tankFinder = new TankFinder(

    }

    public static Options Options = new Options();

    public static IReadOnlyList<Configuration> GetInputConfigurations()
        => Targets.SelectMany(t =>
                PossibleTankSizes().Select(tc => ToConfiguration(tc, t, Options)))
            .ToList();

    public static Configuration ToConfiguration(int[] sizes, Mix target, Options options)
    {
        return new Configuration(sizes, target, options);
    }

    public static double ScoreSimpleLookahead(State state, int depth)
    {
        var r = state.BestMixDistance;
        if (depth == 0)
        {
            return r;
        }

        foreach (var nextState in state.GetNextStates())
        {
            if (nextState.BestMixDistance > state.BestMixDistance)
            //if (nextState.BestMixDistance > r)
                continue;

            var tmp = ScoreSimpleLookahead(nextState, depth - 1);
            if (tmp < r)
                r = tmp;
        }

        return r;
    }

    public static void OutputStateAnalysis(State state, bool contents)
    {
        Console.WriteLine(state.BuildString(null, true, contents));

        var transfers = state.Transfers;
        Console.WriteLine($"Found {transfers.Count} transfers");
        var nextStates = transfers.Select(state.Apply).ToList();
        var best = double.MaxValue;
        var score = ScoreUsingBest(state);
        var cntBetter = 0;
        var cntWorse = 0;
        var cntBest = 1;
        foreach (var nextState in nextStates)
        {
            var tmp = ScoreUsingBest(nextState);
            if (tmp.AlmostEquals(best)) cntBest++;
            if (tmp < best)
            {
                best = tmp;
                cntBest = 1;
            }
            if (tmp < score) cntBetter++;
            if (tmp > score) cntWorse++;
        }
        Console.WriteLine($"Current score = {score}");
        Console.WriteLine($"Found {cntBetter} improvements, and {cntWorse} harmful");
        Console.WriteLine($"Best next score is {best} found {cntBest} instances");
    }

    // NOTE: added heuristic so that the best score is even better if in a smaller container. 
    public static double ScoreUsingBest(State state)
        => state.BestMixDistance + (state.BestMix?.Sum ?? 0) / 1000;

    // NOTE: looking at deltas now. 
    public static double ScoreUsingBestAndDelta(State state)
    {
        var dist = state.BestMixDistance;
        var deltaDist = 0; // ClosestDistanceToOffset(state);
        return dist + deltaDist / 100;
    }

    // Choose the score, based on the best score of the next state
    // Looking ahead in a greedy manner (only considering overal state improvements)
    public static double ScoreWithLookahead(State state, double current, int depth, ref int considered)
    {
        if (depth == 0)
            return current;

        var r = current;
        foreach (var transfer in state.Transfers)
        {
            var nextState = state.Apply(transfer);
            var nextCandidate = ScoreUsingBest(nextState);
            if (nextCandidate < r)
                r = nextCandidate;
            considered++;

            // Only consider improvements on the current score. 
            if (nextCandidate < current)
            {
                var tmp = ScoreWithLookahead(nextState, nextCandidate, depth - 1, ref considered);
                if (tmp < r) 
                    r = tmp;
            }
        }

        return r;
    }

    // Choose the score, based on the best score of the next state
    // Looking ahead in a greedy and narrow manner 
    public static double ScoreWithNarrowLookahead(State state, double score, int depth, ref int considered)
    {
        considered++;
        var r = score;
        if (depth == 0)
            return r;

        var nextStates = state.GetNextStates().Select(st => (st, ScoreUsingBest(st))).ToList();
        nextStates = nextStates.OrderBy(tuple => tuple.Item2).ToList();
        foreach (var nextState in nextStates.Take(20))
        {
            var tmp = ScoreWithNarrowLookahead(nextState.Item1, nextState.Item2, depth - 1, ref considered);
            if (tmp < r)
                r = tmp;
        }

        return r;
    }


    public static double ClosestDistanceToOffset(State state)
    {
        return state.Mixes.Min(mix => mix.DistanceOfNormals(ClosestOffset(state, mix)));
    }

    [Test]
    [TestCaseSource(nameof(GetInputConfigurations))]
    public static void TestInputState(Configuration config)
    {
        var state = State.Create(config);
        OutputStateAnalysis(state, false);
        var score = ScoreUsingBest(state);
        Console.WriteLine($"Assigned a score of {score}");

        var depth = 2;
        var considered = 0;
        var lookahead = ScoreWithNarrowLookahead(state, score, depth, ref considered);
        Console.WriteLine($"Considered {considered} positions to depth {depth} got lookahead score {lookahead}");
    }

    [Test]
    [TestCaseSource(nameof(GetInputConfigurations))]
    public static void Test(Configuration config)
    {
        var state = State.Create(config);

        for (var i = 0; i < 10; ++i)
        {
            Console.WriteLine($"step {i}");

            var transfers = state.Transfers;
            Console.WriteLine($"Found {transfers.Count} transfer operations");
            OutputStateAnalysis(state, false);

            //var bestTransfer = transfers.MinBy(t => ScoreUsingBestAndDelta(state.Apply(t)));
            var bestTransfer = transfers.MinBy(t => ScoreSimpleLookahead(state, 1));

            if (bestTransfer == null)
            {
                Console.WriteLine("No more transfers possible");
                break;
            }

            Console.WriteLine($"Applying transfer {bestTransfer}");
            Console.WriteLine();

            state = state.Apply(bestTransfer);
        }
    }

    [Test]
    [TestCaseSource(nameof(GetInputConfigurations))]
    public static void OutputTankLists(Configuration config)
    {
        Console.WriteLine($"Found {config.TankLists.Count} individual tank lists");
    }
    */
}