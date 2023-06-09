namespace WineMixer
{
    class Tank : IEquatable<Tank>
    {
        public double Size { get; }
        public double[] Percentages { get; }
        public bool IsWaste { get; }

        public Tank(double size, double[] percentages, bool isWaste = false)
        {
            Size = size;
            Percentages = percentages;
            IsWaste = isWaste;
        }

        public IEnumerable<double> GetVolumes()
            => Percentages.Select(p => p * Size);

        public double GetDifference(double[] recipe)
        {
            if (IsWaste || !IsFull()) return double.PositiveInfinity;
            return Percentages.Zip(recipe).Select(v => Math.Abs(v.First - v.Second)).Max();
        }

        public bool IsFull()
            => Utils.IsClose(Percentages.Sum(), 1);

        public bool IsEmpty()
            => Utils.IsClose(Percentages.Sum(), 0);

        public Tank Clone()
            => new Tank(Size, (Percentages.Clone() as double[])!, IsWaste);

        public override bool Equals(object? obj) => this.Equals(obj as Tank);

        public bool Equals(Tank? tank)
        {
            if (tank is null || GetType() != tank.GetType()) return false;

            if (Size != tank.Size || IsWaste != tank.IsWaste) return false;
            return Percentages.Zip(tank.Percentages).All(t => t.First == t.Second);
        }

        public override int GetHashCode()
        {
            // TODO: Better hash function
            int baseHash = Size.GetHashCode() ^ (IsWaste ? int.MinValue : 0);
            return Percentages.Aggregate(baseHash, (hash, p) => hash ^ p.GetHashCode());
        }

        public static bool operator == (Tank? a, Tank? b)
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        public static bool operator != (Tank? a, Tank? b) => !(a == b);

        public override string ToString()
            => string.Format(
                "Tank [{0}]{1}",
                string.Join(", ", GetVolumes().Select(x => Math.Round(x, 3))),
                IsWaste ? " X" : ""
            );
    }
}