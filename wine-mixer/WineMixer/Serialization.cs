namespace WineMixer.Serialization
{
    public class Wine
    {
        public double WineAmount { get; set; }
        public string? WineName { get; set; }
        public int InitialTankIndex { get; set; }
    }

    public class Input
    {
        public List<Wine> TargetRecipe { get; set; } = new();
        public List<int> TankSizes { get; set; } = new();
    }

    public class BlendComponent
    {
        public int WineIndex { get; set; } = new();
        public string? WineName { get; set; }
        public double Amount { get; set; }
    }

    public class TankContents
    {
        public int TankIndex { get; set; }
        public double TankSize { get; set; }
        public List<BlendComponent> Components { get; set; } = new();
    }

    public class State
    {
        public TankContents Best { get; set; } = new();
        public double DistanceToTarget { get; set; } = new();
        public double TotalWine { get; set; } = new();
        public List<TankContents> Tanks { get; set; } = new();
    }

    public class Operation
    {
        public List<TankContents> InputTanks { get; set; } = new();
        public List<TankContents> OutputTanks { get; set; } = new();
        public State State { get; set; } = new();
    }

    public class Output
    {
        public Input Input { get; set; } = new();
        public State Start { get; set; } = new();
        public State End { get; set; } = new();
        public List<Operation> Operations { get; set; } = new();
    }
}
