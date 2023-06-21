namespace WineMixer;

public static class Extensions
{
    const double Epsilon = 0.000001;

    private static readonly Random Rng = new();

    public static T GetRandomElement<T>(this IReadOnlyList<T> self) 
        => self.Count == 0 ? default : self[Rng.Next(self.Count)];

    public static bool AlmostEquals(this double self, double x) 
        => Math.Abs(self - x) < Epsilon;

    public static Mix Average(this IEnumerable<Mix> mixes)
    {
        Mix result = null;
        var i = 0;
        foreach (var m in mixes)
        {
            if (m == null)
                continue;
            if (result == null) 
                result = m;
            else
                result += m;
            i++;
        }
        return result == null 
            ? result : result / i;
    }
}