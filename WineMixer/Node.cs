namespace WineMixer
{
    class Node : IEquatable<Node>
    {
        public Tank[] Tanks { get; }
        public (int, int)[] Connections { get; }
        public Node? Parent { get; }

        public Node(Tank[] tanks, (int, int)[] connections, Node? parent)
        {
            Tanks = tanks;
            Connections = connections;
            Parent = parent;
        }

        public IEnumerable<Tank> GetFilledTanks()
            => Tanks.Where(tank => tank.IsFull());

        public double GetDifference(double[] recipe)
            => Tanks.Select(tank => tank.GetDifference(recipe)).Min();

        public double[] GetPercentagesBestTank(double[] recipe)
        {
            Tank? tank = GetFilledTanks().MinBy(tank => tank.GetDifference(recipe));
            return tank is null ? new double[0] : tank.Percentages;
        }

        public double GetWaste()
            => Tanks.Where(tank => tank.IsWaste).Aggregate(0d, (acc, tank) => acc + tank.Size * tank.Percentages.Sum());

        public double GetProduced(double[] recipe)
        {
            double[] formula = GetPercentagesBestTank(recipe);
            // return Tanks.Where(tank => tank.Percentages.Zip(recipe).Select(p => Utils.IsClose(p.First, p.Second)).All(x => x)).Select(tank => tank.Size).Sum();
            double sum = 0;
            foreach (Tank tank in Tanks)
            {
                bool[] closeness = new bool[recipe.Count()];
                for (int i = 0; i < recipe.Count(); i++)
                {
                    closeness[i] = Utils.IsClose(tank.Percentages[i], recipe[i]);
                }
                if (closeness.All(x => x))
                {
                    sum += tank.Size;
                }
            }
            return sum;
        }

        public double GetUnused(double[] recipe)
            => Tanks.Take(recipe.Length).Select(tank => tank.Size).Sum() - GetWaste() - GetProduced(recipe);

        public int GetStep()
        {
            int step = 0;
            for (Node node = this; node.Parent != null; node = node.Parent) step++;
            return step;
        }
        
        public Node? Transfer((int, int)[] connections)
        {
            Tank[] tanks = Tanks.Select(tank => tank.Clone()).ToArray();
            // Calculate the tranfer speed from/into each tank
            (int, int)[] remainingConnections = (connections.Clone() as (int, int)[])!;
            IEnumerable<int> inTanks = connections.Select(t => t.Item1);
            IEnumerable<int> outTanks = connections.Select(t => t.Item2);
            Dictionary<int, int> extractSpeed = inTanks.Distinct().ToDictionary(i => i, i => inTanks.Count(x => x == i));
            Dictionary<int, int> insertSpeed = outTanks.Distinct().ToDictionary(j => j, j => outTanks.Count(x => x == j));

            while (insertSpeed.Any())
            {
                // Calculate how much we will transfert at this step
                var volumesExtract = extractSpeed.Select(kv => tanks[kv.Key].GetVolumes().Sum() / kv.Value).ToArray();
                var volumesInsert = insertSpeed.Select(kv => (tanks[kv.Key].Size - tanks[kv.Key].GetVolumes().Sum()) / kv.Value).ToArray();
                double transfertVolume = volumesExtract.Concat(volumesInsert).Min();

                // Change the content in each tank accordingly
                foreach ((int a, int b) in remainingConnections)
                {
                    (Tank tankA, Tank tankB) = (tanks[a], tanks[b]);
                    double factor = transfertVolume / tankA.GetVolumes().Sum();
                    for (int i = 0; i < tankA.Percentages.Length; i++)
                    {
                        double t = tankA.Percentages[i] * tankA.Size * factor;
                        tankA.Percentages[i] -= t / tankA.Size;
                        tankB.Percentages[i] += t / tankB.Size;
                    }
                }

                // Remove unnecessary connections and change speed
                foreach ((int a, int b) in (remainingConnections.Clone() as (int, int)[])!)
                {
                    if (!tanks[a].IsEmpty() && Math.Round(tanks[b].GetVolumes().Sum(), 6) < tanks[b].Size) continue;

                    remainingConnections = remainingConnections.Where(connection => connection.Item1 != a || connection.Item2 != b).ToArray();
                    if (--extractSpeed[a] <= 0)
                    {
                        inTanks = inTanks.Where(i => i != a);
                        extractSpeed.Remove(a);
                    }
                    if (--insertSpeed[b] <= 0)
                    {
                        outTanks = outTanks.Where(j => j != b);
                        insertSpeed.Remove(b);
                    }
                }
            }

            for (int i = 0; i < tanks.Length; i++)
            {
                Tank tank = tanks[i];
                bool isWaste = tank.IsWaste;
                if (!Utils.IsClose(tank.Percentages.Sum(), 1)) isWaste = true;
                if (Utils.IsClose(tank.Percentages.Sum(), 0)) isWaste = false;
                tanks[i] = new Tank(tank.Size, tank.Percentages, isWaste);
            }

            return new Node(tanks, connections, this);
        }

        public Node[] GenerateChildren(int connectionsLimit = int.MaxValue)
        {
            // Can tranfert from possibleIn to possibleOut
            IEnumerable<int> possibleIn = Tanks.Select((tank, index) => (tank, index)).Where(t => t.tank.IsFull() && !t.tank.IsWaste).Select(t => t.index);
            IEnumerable<int> possibleOut = Tanks.Select((tank, index) => (tank, index)).Where(t => !t.tank.IsFull()).Select(t => t.index);

            List<(int, int)[]> combinations = new List<(int, int)[]>{};
            foreach ((int, int)[] connections in Utils.PairsCombinationsGenerator(possibleIn, possibleOut))
            {
                if (!connections.Any()) continue;
                var xs = connections.Select(t => t.Item1);
                var ys = connections.Select(t => t.Item2);

                // Cannot have a tank connected to too many others
                if (xs.GroupBy(x => x).Any(t => t.Count() > connectionsLimit) || ys.GroupBy(y => y).Any(t => t.Count() > connectionsLimit)) continue;

                // Output must be smaller than the input
                double inputVolume = xs.Distinct().Select(i => Tanks[i].GetVolumes().Sum()).Sum();
                double outputMaxVolume = ys.Distinct().Select(j => Tanks[j].Size - Tanks[j].GetVolumes().Sum()).Sum();

                combinations = combinations.Append(connections).ToList();
            }

            // Generate all the children from the possible connections
            return combinations.Select(connections => Transfer(connections)).Where(node => node != null).ToArray()!;
        }

        public override bool Equals(object? obj) => this.Equals(obj as Node);

        public bool Equals(Node? node)
        {
            if (node is null || GetType() != node.GetType()) return false;
            
            Tank[] otherTanks = ((Node) node).Tanks;
            return Tanks.Zip(otherTanks).All(t => t.First.Equals(t.Second));
        }

        public override int GetHashCode()
        {
            // TODO: Better hash function
            return Tanks.Aggregate(0, (hash, tank) => hash ^ tank.GetHashCode());
        }

        public static bool operator == (Node? a, Node? b)
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        public static bool operator != (Node? a, Node? b) => !(a == b);

        public override string ToString() {
            return string.Format(
                "Step {0}: [\n\t{1}\n]",
                GetStep(),
                string.Join("\n\t", Tanks.AsEnumerable())
            );
        }
    }
}