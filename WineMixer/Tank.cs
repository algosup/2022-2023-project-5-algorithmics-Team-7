namespace WineMixer
{
    class Tank
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

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Tank other = (Tank) obj;

            if (Size != other.Size || IsWaste != other.IsWaste) return false;
            return Percentages.Zip(other.Percentages).All(t => t.First == t.Second);
        }
        
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            // throw new System.NotImplementedException();
            return base.GetHashCode();
        }

        public override string ToString()
            => string.Format(
                "Tank [{0}]{1}",
                string.Join(", ", GetVolumes().Select(x => Math.Round(x, 3))),
                IsWaste ? " X" : ""
            );
    }
}