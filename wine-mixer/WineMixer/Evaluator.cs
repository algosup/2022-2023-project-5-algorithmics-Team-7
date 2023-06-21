namespace WineMixer;

public class Evaluator
{
    public Options Options { get; }

    public Evaluator(Options options)
        => Options = options;

    public double EvaluateByTransfers(State state) 
        => state.Transfers.Min(t => state.TargetDistance(state.GetMix(t)));

    public double EvaluateByMix(State state)
        => state.Mixes.Min(state.TargetDistance);

    public Transfer GetBestTransfer(State state)
    {
        var candidates = state.Transfers
            .GroupBy(t => EvaluateByTransfers(state.Apply(t)))
            .MinBy(g => g.Key)
            .ToList();
        var results = candidates
            //.GroupBy(t => EvaluateByMix(state.Apply(t)))
            //.MinBy(g => g.Key)
            .ToList();
        Console.WriteLine($"Considered {state.Transfers.Count}, found {candidates.Count} candidates, and {results.Count} results");
        return results.First();
    }

    public Transfer ChooseBestTransfer(List<(Transfer, double)> choices)
        => choices.Count > 0 ? choices.MinBy(tuple => tuple.Item2).Item1 : null;
}