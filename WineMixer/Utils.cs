namespace WineMixer
{
    class Utils
    {
        public static bool IsClose(double a, double b, double epsilon = 1e-8)
            => Math.Abs(a - b) < epsilon;

        public static double[] Normalize(IEnumerable<double> values, double factor, int rounding)
        {
            if (values.Any(x => x < 0)) throw new NotImplementedException("Cannot constrain negative values yet");
            double sum = values.Sum();
            if (sum == 0) return values.ToArray();
            IEnumerable<double> factored = values.Select(x => x * factor / sum);
            return factored.Select(x => Math.Round(x, (int) rounding)).ToArray();
        }

        internal static IEnumerable<(int, int)[]> PairsCombinationsGenerator(IEnumerable<int> L1, IEnumerable<int> L2, List<(int, int)>? stack = null) {
            // Supposes all three arguments to be sorted
            if (stack is null) stack = new List<(int, int)>{};

            // TODO: Change order of yielding [[(1, 1), (1, 2), (1, 3), ...], ...] -> [[(1, 1)], [(1, 2)], [(1, 3)], ...]
            yield return stack.ToArray();
            if (stack.Count() == L1.Count() * L2.Count()) yield break;

            (int minA, int minB) = stack.Any() ? stack.Last() : (int.MinValue, 0);
            foreach (int a in L1)
            {
                if (a < minA) continue;
                foreach (int b in L2)
                {
                    if (a == minA && b <= minB) continue;
                    // TODO: Flatten from recursive to iterative
                    List<(int, int)> stack2 = stack.Append((a, b)).ToList();
                    foreach ((int, int)[] combinasion in PairsCombinationsGenerator(L1, L2, stack2))
                        yield return combinasion;
                }
            }

        }
    }
}