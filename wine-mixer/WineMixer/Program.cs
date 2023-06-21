using System.Diagnostics;
using System.Text.Json;

namespace WineMixer
{

    public class Program
    {
        public static string ExeFolder =
            Environment.ProcessPath != null ? Path.GetDirectoryName(Environment.ProcessPath) : "";

        public static string UsageFile = Path.Combine(ExeFolder, "usage.txt");

        public static void ShowUsage(bool exit = true)
        {
            // Fall-back text for usage 
            var usageText = "Usage: wine-mixer.exe <tanksizes> <winemix> [<optionsfilename>]";
            
            try
            {
                // Try loading a more elaborate usage file 
                usageText = File.ReadAllText(UsageFile);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(usageText);
            if (exit)
            {
                Environment.Exit(1);
            }
        }

        public static void WriteMix(Mix mix, string fileName)
        {
            File.WriteAllLines(fileName, mix.Values.Select(v => $"{v:#.00000}"));
        }

        public static void WriteTransfers(IEnumerable<Transfer> transfers, string fileName)
        {
            File.WriteAllLines(fileName, transfers.Select(t => t.ToString()));
        }

        public static void WriteJson(Session session, string fileName)
        {
            var sessionText = JsonSerializer.Serialize(session, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(fileName, sessionText);
        }

        public static void OutputConfigurationAnalysis(Configuration config)
        {
            Console.WriteLine($"Configuration analysis");
            Console.WriteLine($"The original target blend was {config.OriginalTarget} and added up to {config.OriginalTarget}");
            Console.WriteLine($"The target blend is {config.Target} has {config.NumWines} components and adds up to {config.Target.Sum}");
            Console.WriteLine($"The number of starting tanks is {config.NumTanks}");
            Console.WriteLine($"The smallest tank is {config.Sizes.Min()} and the largest is {config.Sizes.Max()}");
            Console.WriteLine($"The starting amount of wine is {config.InitialWineAmount}");
            Console.WriteLine($"JSON options provided are: {config.Options}");
        }

        public static void OutputState(State state)
        {
            Console.WriteLine(state);
            var bestMix = state.BestMix();
            var dist = state.TargetDistance(bestMix);
            Console.WriteLine($"Target mix is {state.Configuration.Target}");
            Console.WriteLine($"Best mix is {bestMix.SumOfOne}");
            Console.WriteLine($"Distance is {dist:#.0000}");

            var sw = Stopwatch.StartNew();
            Console.WriteLine($"Computing transfers ...");
            var cnt = state.Transfers.Count;
            Console.WriteLine($"Found {cnt} transfers in {sw.Elapsed.TotalSeconds:#.0000}");

            //Console.WriteLine($"Original Target mix is {state.Configuration.OriginalTarget}");
            //Console.WriteLine($"Scaled best mix     is {state.Configuration.ScaleMixToOriginalTarget(bestMix)}");
        }

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Initializing .... ");
                Configuration config = null;
                try
                {
                    var tankSizesFileName = args[0];
                    var wineMixFileName = args[1];
                    var optionsFileName = args.Length > 2 ? args[2] : Configuration.DefaultOptionsFileName;
                    config = Configuration.LoadFromFiles(tankSizesFileName, wineMixFileName, optionsFileName);
                }
                catch (Exception _)
                {
                    ShowUsage(false);
                    return;
                }

                OutputConfigurationAnalysis(config);

                // Test that we can write to the output folder, before running the long process. 
                var outputFolder = Path.Combine(ExeFolder, config.Options.OutputFolder);
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                var outputBlendFilePath = Path.Combine(outputFolder, config.Options.OutputBlendFileName);
                var outputStepsFilePath = Path.Combine(outputFolder, config.Options.OutputStepsFileName);

                // Test writing the files with empty text. 
                File.WriteAllText(outputBlendFilePath, "");
                File.WriteAllText(outputStepsFilePath, "");

                // Create the starting state 
                Console.WriteLine("Starting compute process");
                var state = State.Create(config);
                var options = config.Options;
                var evaluator = new Evaluator(options);

                var transfers = new List<Transfer>();
                var states = new List<State> { state };

                for (var i = 0; i < options.MaxNumSteps; ++i)
                {
                    state.CheckTotalWineIsValid();
                    state.CheckThatTankAmountsAreValid();

                    var sw = Stopwatch.StartNew();
                    Console.WriteLine($"Step {i}");
                    OutputState(state);
                    var transfer = evaluator.GetBestTransfer(state);
                    if (transfer == null)
                    {
                        Console.WriteLine($"Stopping early, no more transfers found");
                        break;
                    }

                    Console.WriteLine($"Found transfer {transfer} in {sw.Elapsed.TotalSeconds:#.00} second");
                    transfers.Add(transfer);
                    state = state.Apply(transfer);
                    states.Add(state);
                }

                state.CheckTotalWineIsValid();
                state.CheckThatTankAmountsAreValid();

                Console.WriteLine($"Final state");
                Console.WriteLine(state.BuildString());

                WriteTransfers(transfers, outputStepsFilePath);

                var bestMix = state.BestMix();
                var finalMix = config.ScaleMixToOriginalTarget(bestMix);
                WriteMix(finalMix, outputBlendFilePath);

                var session = new Session()
                {
                    States = states,
                    Transfers = transfers,
                    TankSizes = config.Sizes.ToList(),
                    TargetMix = config.OriginalTarget.Values.ToList(),
                    Options = config.Options,
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred: {e}");
                Environment.Exit(1);
            }
        }
    }
}