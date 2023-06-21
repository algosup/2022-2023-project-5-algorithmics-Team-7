using System.Diagnostics;

namespace WineMixer;

/// <summary>
/// A tank list is a sequence of tanks, always ordered and with no repetitions.
/// This is used to represent possible inputs and outputs from a transfer operation. 
/// </summary>
public class TankList
{
    public int Volume { get; }
    public int Count => Tanks.Count;
    public int this[int i] => Tanks[i];
    public IReadOnlyList<int> Tanks { get; }
    public TankList(int volume, params int[] tanks) => (Volume, Tanks) = (volume, tanks);
    public TankList(int volume, List<int> tanks) => (Volume, Tanks) = (volume, tanks);
    public bool HasTank(int n) => Tanks.Contains(n);
    public int Last => Count == 0 ? -1 : Tanks[Count - 1];

    public TankList AddTank(int tankVolume, int tankIndex)
    {
        Debug.Assert(!Tanks.Contains(tankIndex));
        var tmp = new List<int>(Tanks) { tankIndex };
        return new TankList(Volume + tankVolume, tmp);
    }

    public override string ToString()
    {
        return $"V={Volume}({string.Join(", ", Tanks)})";
    }
}