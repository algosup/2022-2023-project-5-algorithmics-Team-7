using System.Diagnostics;
using WineMixer;

namespace WineMixerTests
{
    public class BenchmarkScaffolding
    {
        public static List<int> EmptyList = new List<int>();
        private const int MaxSize = 32;

        public IEnumerable<(TParameter, TimeSpan?)> ProfileAndTest<TParameter, TInput, TOutput>(
            IEnumerable<TParameter> parameters,
            Func<TParameter, TInput> inputGenerator,
            Func<TInput, TOutput> f, 
            Func<TInput, TOutput, bool> validator)
        {
            var i = 0;
            foreach (var p in parameters)
            {
                Console.WriteLine($"Running test {i++} with parameter {p}");
                var input = inputGenerator(p);
                var sw = Stopwatch.StartNew();
                var output = f(input);
                var elapsed = sw.Elapsed;
                var passed = validator(input, output);
                yield return (p, passed ? elapsed : null);
            }
        }

        public void RunTest(Func<int, int> f)
        {
            var results = ProfileAndTest(
                Enumerable.Range(20, 7),
                x => x,
                f,
                (i, o) =>
                {
                    Console.WriteLine($"Input = {i}, Output = {o}");
                    return true;
                }
            ).ToList();

            var i = 0;
            foreach (var r in results)
            {
                var passedStr = r.Item2 == null ? "failed" : "passed";
                Console.WriteLine($"Test {i++} with parameter {r.Item1} {passedStr} in {r.Item2?.TotalSeconds:0.###} seconds");
            }
        }
    }
}
