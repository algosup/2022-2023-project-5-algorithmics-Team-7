namespace WineMixer
{
    class Node
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
            return tank == null ? new double[0] : tank.Percentages;
        }

        public double GetWaste()
            => Tanks.Where(tank => tank.IsWaste).Aggregate(0d, (acc, tank) => acc + tank.Size * tank.Percentages.Sum());

        public double GetProduced(double[] recipe)
        {
            double[] formula = GetPercentagesBestTank(recipe);
            // return Tanks.Where(tank => tank.Percentages.Zip(recipe).Select(p => Utils.IsClose(p.First, p.Second)).All(x => x)).Select(tank => tank.Size).Sum();
            Console.WriteLine("Formula " + string.Join(", ", formula));
            double sum = 0;
            foreach (Tank tank in Tanks)
            {
                bool[] closeness = new bool[recipe.Count()];
                for (int i = 0; i < recipe.Count(); i++)
                {
                    Console.WriteLine($"Closeness {tank.Percentages[i]}, {recipe[i]}");
                    closeness[i] = Utils.IsClose(tank.Percentages[i], recipe[i]);
                }
                Console.WriteLine(string.Join(",", closeness));
                if (closeness.All(x => x))
                {
                    sum += tank.Size;
                }
            }
            return sum;
        }

        public double GetUnused(double[] recipe)
            => Tanks.Take(recipe.Length).Select(tank => tank.Size).Sum() - GetWaste() - GetProduced(recipe);
        
        public Node Transfer((int, int)[] Connections)
        {
            // TODO
            throw new NotImplementedException();
        }

        public Node[] GenerateChildren()
        {
            // TODO
            throw new NotImplementedException();
        }

        // TODO: Override Equals
    }
}