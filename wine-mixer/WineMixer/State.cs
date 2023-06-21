using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using WineMixer.Serialization;

namespace WineMixer;

/// <summary>
/// Represents the state of the tanks: which ones have wine, and what the blend of wines
/// currently is in each tank. If we think of wine blending as a graph problem
/// the state is a node in the graph.  
/// </summary>
public class State
{
    public static State Create(Configuration configuration)
    {
        var contents = new Mix[configuration.NumTanks];

        // TODO: for now the first N tanks contain the first N wines. 
        var numWines = configuration.NumWines;
        for (var i = 0; i < numWines; ++i)
        {
            var mix = Mix.CreateFromIndex(i, numWines);
            contents[i] = mix * configuration.Sizes[i];
        }

        return new State(configuration, contents);
    }

    public State(Configuration configuration, IReadOnlyList<Mix> contents, int depth = 0)
    {
        Debug.Assert(configuration.Sizes.Count == contents.Count);
        Configuration = configuration;
        Contents = contents;
        Depth = depth;
        StateId = _nextStateId++;
    }

    private static int _nextStateId;

    public int NumTanks => Configuration.NumTanks;
    public Configuration Configuration { get; }
    public IReadOnlyList<Mix> Contents { get; }
    public IEnumerable<Mix> Mixes => Contents.Where(m => m != null)!;
    public int Depth { get; }
    public int StateId { get; }
    public double Volume { get; }
    public int UsedTanks { get; }
    public double TotalWine => Contents.Sum(m => m?.Sum ?? 0);
    public IReadOnlyList<Transfer> Transfers => ComputeTransfers();
    private List<Transfer> _transfers;

    private double _score = double.MaxValue;

    public double Score
    {
        get
        {
            if (_score >= double.MaxValue)
            {
                _score = Mixes.Min(TargetDistance);
            }
            return _score;
        }
    }

    public bool IsOccupied(int i) 
        => Contents[i] != null;

    public bool IsUnoccupied(int i)
        => Contents[i] == null;

    public Mix this[int i]
        => Contents[i];

    public int GetTankSize(int i)
        => Configuration[i];

    public double TargetDistance(Mix mix) 
        => Configuration.TargetDistance(mix.SumOfOne);
     
    private IReadOnlyList<Transfer> ComputeTransfers()
    {
        var sizes = Configuration.Sizes;
        var maxCount = Configuration.Options.MaxInputOrOutputTanks;

        if (_transfers == null) 
        {
            _transfers = new List<Transfer>();

            // Make sure we don't consider two empty tanks of the same size! 
            // We do this by creating a boolean array equivalent to the number of tanks
            var validOutputTanks = new bool[NumTanks];
            for (var i = 0; i < NumTanks; ++i)
            {
                if (IsOccupied(i))
                    continue;
                
                // Check that there is no other tank that is being considered
                // that has the same size.  
                var validOutputTank = true;
                for (var j = 0; j < i; ++j)
                {
                    if (validOutputTanks[j] && GetTankSize(j) == GetTankSize(i))
                        validOutputTank = false;
                }
                validOutputTanks[i] = validOutputTank;
            }

            var srcTankFinder = new TankFinder(sizes, IsOccupied, maxCount);
            var destTankFinder = new TankFinder(sizes, i => validOutputTanks[i], maxCount);

            var destLookup = new Dictionary<int, List<TankList>>();

            var srcTankLists = srcTankFinder.GetAllPermutations().ToList();

            //Console.WriteLine($"Found {srcTankLists.Count} possible input configurations");
            //Console.WriteLine($"Found {validOutputTanks.Count(x => x)} valid output tanks");

            foreach (var occ in srcTankLists)
            {
                if (occ.Volume == 0)
                    continue;

                var volume = occ.Volume;
                
                if (!destLookup.ContainsKey(volume))
                    destLookup.Add(volume, destTankFinder.GetPermutationsOfVolume(volume).ToList());

                var destTankLists = destLookup[volume];

                foreach (var unocc in destTankLists)
                {
                    if (occ.Count > 1 || unocc.Count > 1)
                    {
                        var transfer = new Transfer(occ, unocc);
                        Debug.Assert(IsTransferValid(transfer));
                        _transfers.Add(transfer);
                    }
                }
            }
        }
        return _transfers;
    }

    public Mix GetMix(TankList tanks)
        => tanks.Tanks.Aggregate(Configuration.EmptyMix, (mix, tank) => mix + Contents[tank]);

    public Mix GetMix(Transfer transfer)
        => GetMix(transfer.Inputs);

    public bool IsOccupied(TankList tanks)
        => tanks.Tanks.All(IsOccupied);

    public bool IsUnoccupied(TankList tanks)
        => !tanks.Tanks.Any(IsOccupied);

    public bool IsTransferValid(Transfer transfer)
        => transfer.Inputs.Volume == transfer.Outputs.Volume
        && IsOccupied(transfer.Inputs)
        && IsUnoccupied(transfer.Outputs);

    public State Apply(Transfer transfer)
    {
        if (transfer.Inputs.Count == 0) 
            throw new Exception("Transfer cannot have 0 inputs");

        Debug.Assert(IsTransferValid(transfer));

        var mix = GetMix(transfer);
        var newContents = Contents.ToArray();

        foreach (var inputTank in transfer.Inputs.Tanks)
            newContents[inputTank] = null;

        var totalVolume = (double)transfer.Outputs.Volume;
        Debug.Assert(mix.Sum.AlmostEquals(totalVolume));

        var testSum = 0.0;
        foreach (var outputTank in transfer.Outputs.Tanks)
        {
            if (newContents[outputTank] != null)
                throw new Exception("Attempting to transfer to a non-empty container");

            var relativeVolume = GetTankSize(outputTank) / totalVolume;
            var relativeMix = mix * relativeVolume;
            testSum += relativeMix.Sum;
            newContents[outputTank] = relativeMix;
        }

        // Just make sure things are adding up. 
        Debug.Assert(testSum.AlmostEquals(totalVolume));

        // Get the new state
        var r = new State(Configuration, newContents, Depth+1);

        // Assure that the amount of wine in the system is the same
        Debug.Assert(r.TotalWine.AlmostEquals(TotalWine));

        return r;
    }

    public StringBuilder BuildString(StringBuilder sb = null, bool contents = true)
    {
        sb ??= new StringBuilder();
        sb.AppendLine($"State {StateId} depth={Depth} volume={Volume} tanks={UsedTanks}/{NumTanks}");

        if (contents)
        {
            for (var i = 0; i < NumTanks; ++i)
            {
                var t = Contents[i];
                if (t != null)
                    sb.AppendLine($"Tank {i} has size {Configuration[i]} and contains {t.SumOfOne} of distance {TargetDistance(t):0.###}");
                //else 
                //  sb.AppendLine($"Tank {i} has size {Configuration[i]} and is empty");
            }
        }
        return sb;
    }

    public override string ToString() 
        => BuildString().ToString();

    public IEnumerable<State> GetNextStates()
        => Transfers.Select(Apply);

    public void CheckTotalWineIsValid()
    {
        if (!TotalWine.AlmostEquals(Configuration.InitialWineAmount))
            throw new Exception($"The amount of wine in the system is not correct, expected {Configuration.InitialWineAmount} but was {TotalWine}");
    }

    public void CheckThatTankAmountsAreValid()
    {
        for (var i = 0; i < Contents.Count; ++i)
        {
            if (Contents[i] == null) continue;
            var amount = Contents[i]!.Sum;
            if (!amount.AlmostEquals(GetTankSize(i)))
                throw new Exception($"Invalid amount of wine: {amount} expected {GetTankSize(i)}");
        }
    }

    public Mix BestMix()
        => Mixes.MinBy(TargetDistance);
}