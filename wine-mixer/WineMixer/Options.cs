using System.Text.Json;

namespace WineMixer;

public class Options
{
    // public bool MinimizeWaste { get; set; } 
    // public long TimeOutInSeconds { get; set; }
    public int MaxInputOrOutputTanks { get; set; } = 4;
    public string? OutputBlendFileName { get; set; } = "result.txt";
    public string? OutputStepsFileName { get; set; } = "steps.txt";
    public string? OutputFolder { get; set; } = "output";
    public int MaxNumSteps { get; set; } = 10;
    public override string ToString()
        => JsonSerializer.Serialize(this, 
            new JsonSerializerOptions() { WriteIndented = true });
}
