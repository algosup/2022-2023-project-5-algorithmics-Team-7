namespace WineMixer;

/// <summary>
/// This class is used to compute the list of source tanks
/// and the list of output tanks. Only tanks that pass the predicate
/// are considered 
/// </summary>
public class TankFinder
{
    public TankFinder(IReadOnlyList<int> tankSizes, Func<int, bool> predicate, int maxCount)
    {
        TankSizes = tankSizes;
        Predicate = predicate;
        MaxCount = maxCount;
    }

    public IReadOnlyList<int> TankSizes { get; }
    public Func<int, bool> Predicate { get; }
    public int MaxCount { get; }

    public int NumTanks
        => TankSizes.Count;

    public int GetTankSize(int n)
        => TankSizes[n];

    public int GetTankSizeSum(IEnumerable<int> tanks)
        => tanks.Sum(GetTankSize);

    public IEnumerable<TankList> GetPermutationsOfVolume(int target)
        => GetPermutationsOfVolume(target, new TankList(0), 0);

    public IEnumerable<TankList> GetPermutationsOfVolume(int target, TankList prevList, int depth)
    {
        // If we have reached the target volume, stop
        if (prevList.Volume == target)
            yield return prevList;

        // If we have reach the maximum volume, we are complete
        if (prevList.Volume >= target)
            yield break;

        // If we have considered all of the tanks we are done 
        if (depth >= NumTanks)
            yield break;
        
        // Get the volume of the current tank
        var tankVolume = TankSizes[depth];

        // Terminate early if we have the maximum number of tanks 
        if (prevList.Count == MaxCount)
            yield break;

        // Recursively look for more tanks 
        foreach (var tmp in GetPermutationsOfVolume(target, prevList, depth + 1))
            yield return tmp;

        // We don't add this tank to the list if the predicate is false. 
        if (!Predicate(depth))
            yield break;
        
        // We are going to try adding this tank 
        var nextList = prevList.AddTank(tankVolume, depth);
        
        // Too much volume so we can stop 
        if (nextList.Volume > target)
            yield break;

        // The target volume, so we can return it and stop 
        if (nextList.Volume == target)
            yield return nextList;
        else
            // Recursively look for possibilities with this tank added  
            foreach (var tmp in GetPermutationsOfVolume(target, nextList, depth + 1))
                yield return tmp;
    }

    public IEnumerable<TankList> GetAllPermutations()
        => GetAllPermutations(new TankList(0), 0);

    public IEnumerable<TankList> GetAllPermutations(TankList prevList, int depth)
    {
        // If we have considered all of the tanks we are done 
        if (depth >= NumTanks)
            yield break;

        // Get the volume of the current tank
        var tankVolume = TankSizes[depth];

        // Terminate early if we have the maximum number of tanks 
        if (prevList.Count == MaxCount)
            yield break;

        // Recursively look for more tanks 
        foreach (var tmp in GetAllPermutations(prevList, depth + 1))
            yield return tmp;

        // We don't add this tank to the list if the predicate is false. 
        if (!Predicate(depth))
            yield break;

        // We are going to try adding this tank 
        var nextList = prevList.AddTank(tankVolume, depth);

        // Return the generated list
        yield return nextList;

        // Recursively look for possibilities 
        foreach (var tmp in GetAllPermutations(nextList, depth + 1))
            yield return tmp;
    }
}
