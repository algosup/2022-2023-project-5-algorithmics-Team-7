using System.Diagnostics;
using WineMixer;

namespace WineMixerTests
{
    public class TestTankFinder
    {
        //public static int[] TankSizes = new[] { 1, 1, 2, 3, 5, 6, 8, 13 };
        
        // public static int[] BigTankSizes = new[] { 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13 };
        
        public static int[] OldTankSizes = 
        {
            20, 20, 20, 20,
            20, 20, 20, 20,
            1, 1, 2, 2, 3, 3, 4,
            4, 5, 5, 6, 10, 10,
            12, 12, 13, 15, 16,
            20, 24, 25, 25, 30,
            30, 32, 40, 40, 60, 80
        };

        public static int[] TankSizes =
        {
            20, 20, 20, 20,
            20, 20, 20, 20,
            1, 1, 2, 2, 3, 3, 4,
            4, 5, 5, 6, 7, 8, 9, 10, 10, 11, 11, 
            12, 12, 13, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 25, 26, 27, 28, 29, 30,
            30, 32, 33, 34, 35, 36, 40, 42, 45, 45, 48, 50, 
            52, 55, 58, 60, 65, 66, 68, 70, 72, 75, 78, 79, 80,

            20, 20, 20, 20,
            20, 20, 20, 20,
            1, 1, 2, 2, 3, 3, 4,
            4, 5, 5, 6, 7, 8, 9, 10, 10, 11, 11,
            12, 12, 13, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 25, 26, 27, 28, 29, 30,
            30, 32, 33, 34, 35, 36, 40, 42, 45, 45, 48, 50,
            52, 55, 58, 60, 65, 66, 68, 70, 72, 75, 78, 79, 80
        };

        public static int Volume = 80;//

        /*
        [Test]
        public void ProfileTankFinder()
        {
            TankSizes = TankSizes.OrderByDescending(x => x).ToArray();

            {
                var t = new TankFinder(TankSizes);
                Console.WriteLine("First run");
                var sw = Stopwatch.StartNew();
                const int maxCount = 5;
                t.GetPermutationsOfVolume(Volume, maxCount);
                var init = t.Lookup[Volume].ToList();
                for (var i = 0; i < 10; ++i)
                {
                    Console.WriteLine(string.Join(", ", init[i]));
                }
                Console.WriteLine($"Found {init.Count} permutations from {TankSizes.Length} of up to {maxCount} adding up to {Volume} in {sw.Elapsed.TotalSeconds:#.00} seconds");
                //Console.WriteLine($"Number of tank indices in lookup {t.Lookup.Count(xs => xs != null)}");
                //Console.WriteLine($"Total entries in lookup {t.Lookup.Sum(x => x?.Count() ?? 0)}");
            }
        }*/
    }
}
