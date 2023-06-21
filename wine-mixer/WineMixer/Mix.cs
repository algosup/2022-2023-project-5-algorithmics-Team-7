using System.Globalization;
using System.Text.Json.Serialization;

namespace WineMixer;

/// <summary>
/// A mix is a vector representing how much of each wine is stored in a tank,
/// or that represents the target.
/// The program aims to minimize the Euclidean distance between the target mix and
/// the contents of a tank.
/// </summary>
public class Mix
{
    public Mix(params double[] values)
        : this((IReadOnlyList<double>)values)
    { }

    public Mix(IReadOnlyList<double> values)
        => Values = values;

    public IReadOnlyList<double> Values { get; }
    public int Count => Values.Count;
    public double Sum => Values.Sum();
    public double SumSqrs => Values.Select(v => v * v).Sum();
    public double Length => Math.Sqrt(SumSqrs);

    public Mix Normal
    {
        get
        {
            var len = Length;
            if (len.AlmostEquals(0))
                return this;
            if (len.AlmostEquals(1))
                return this;
            return this / Length;
        }
    }

    public Mix SumOfOne
    {
        get
        {
            var len = Length;
            if (len.AlmostEquals(0))
                return this;
            if (len.AlmostEquals(1))
                return this;
            return this / Sum;
        }
    }

    public double Distance(Mix other) 
        => other == null 
            ? double.MaxValue 
            : (other - this).Length;

    public static Mix Add(Mix mixA, Mix mixB)
    {
        var tmp = new double[mixA.Count];
        for (var i = 0; i < mixA.Count; ++i)
        {
            tmp[i] = mixA.Values[i] + mixB.Values[i];
        }

        return new(tmp);
    }

    public static Mix Subtract(Mix mixA, Mix mixB)
    {
        var tmp = new double[mixA.Count];
        for (var i = 0; i < mixA.Count; ++i)
        {
            tmp[i] = mixA.Values[i] - mixB.Values[i];
        }

        return new(tmp);
    }

    public static Mix operator +(Mix a, Mix b)
        => Add(a, b);

    public static Mix operator -(Mix a, Mix b)
        => Subtract(a, b);

    public static Mix operator *(Mix mix, double x)
        => Multiply(mix, x);

    public static Mix operator /(Mix mix, double x)
        => Multiply(mix, 1.0 / x);

    public static Mix Multiply(Mix mix, double x)
    {
        var tmp = new double[mix.Count];
        for (var i = 0; i < mix.Count; ++i)
        {
            tmp[i] = mix.Values[i] * x;
        }

        return new(tmp);
    }

    public override string ToString()
    {
        return $"({string.Join(", ", Values.Select(x => $"{x:0.###}"))})";
    }

    public int CountUsedWines()
    {
        return Values.Count(x => x > 0);
    }

    public static Mix LoadFromFile(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var r = new List<double>();
        foreach (var line in lines)
        {
            var amt = double.Parse(line.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
            r.Add(amt);
        }
        return new Mix(r);
    }

    public static Mix CreateFromIndex(int index, int numWines)
    {
        var tmp = new double[numWines];
        tmp[index] = 1.0;
        return new Mix(tmp);
    }

    public Mix Lerp(Mix other, double amount = 0.5)
    {
        var tmp = new double[Count];
        for (var i = 0; i < Count; ++i)
        {
            tmp[i] = (Values[i] * 1.0 - amount) + (other.Values[i] * amount);
        }

        return new Mix(tmp);
    }

    public double DistanceOfNormals(Mix other)
    {
        return Normal.Distance(other?.Normal);
    }
}