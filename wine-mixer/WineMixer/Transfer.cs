using System.Diagnostics;

namespace WineMixer;

public class Transfer
{
    public TankList Inputs { get; }
    public TankList Outputs { get; }
    
    public Transfer(TankList inputs, TankList outputs)
    {
        Debug.Assert(inputs.Volume == outputs.Volume);
        Inputs = inputs;
        Outputs = outputs;
    }

    public override string ToString()
        => $"({string.Join(",", Inputs.Tanks)}) -> ({string.Join(",", Outputs.Tanks)})";
}   
