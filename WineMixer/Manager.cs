namespace WineMixer
{
    using DefaultComparerT = Tuple<double, double, double, int, int>;

    class Config
    {
        public int MaxSteps { get; set; } = 10;
        // public Func<Node> Algorithm { get; set; } = ; // TODO
        public double AcceptancePercentage { get; set; } = 0.01; // Requested formula ±1%/wine
        public Func<Node, Node, double[], int> Comparer = Manager.Comparer;
    }

    class Manager
    {
        public double[] Sizes { get; set; }
        public double[] Recipe { get; set; }
        public Config Config { get; }

        public Manager(double[] sizes, double[] recipe)
        {
            Sizes = sizes;
            Recipe = recipe;
            Config = new Config();
        }

        public Node Bruteforce()
        {
            // Setup
            double[] recipe = Utils.Normalize(Recipe, 1, 0);
            Tank[] startingTanks = Sizes.Select((size, i)
                => new Tank(size, recipe.Select((_, j) => i == j ? 1d : 0d).ToArray(), false)
            ).ToArray();
            Node start = new Node(startingTanks, new (int, int)[0], null);

            // Edge case: If there is no recipe or if we already meet the criteria
            if (!recipe.Any() || start.GetDifference(recipe) < Config.AcceptancePercentage) return start;

            Node bestNode = start;
            List<Node> children = new List<Node>() {start};
            List<Node> parents;
            HashSet<Node> done = new HashSet<Node>();

            for (int step = 1; step <= Config.MaxSteps; step++)
            {
                if (!children.Any()) break; // We checked every possibility

                (parents, children) = (children, new List<Node>());
                done.UnionWith(parents);

                foreach (Node parent in parents) foreach (Node child in parent.GenerateChildren())
                {
                    if (done.Contains(child) || double.IsInfinity(child.GetDifference(recipe))) continue;
                    children.Add(child);
                    // Console.WriteLine(child);

                    // Memorize the best one
                    if (Config.Comparer(child, bestNode, recipe) < 0) bestNode = child;
                }
            }

            return bestNode;
        }

        public static int Comparer(Node node1, Node node2, double[] recipe)
        {
            DefaultComparerT r1 = new DefaultComparerT(
                Math.Round(node1.GetDifference(recipe), 6),
                -Math.Round(node1.GetProduced(recipe), 6),
                Math.Round(node1.GetWaste(), 6),
                node1.GetStep(),
                node1.Connections.Count()
            );
            DefaultComparerT r2 = new DefaultComparerT(
                Math.Round(node2.GetDifference(recipe), 6),
                -Math.Round(node2.GetProduced(recipe), 6),
                Math.Round(node2.GetWaste(), 6),
                node2.GetStep(),
                node2.Connections.Count()
            );
            return Comparer<DefaultComparerT>.Default.Compare(r1, r2);
        }
    }
}