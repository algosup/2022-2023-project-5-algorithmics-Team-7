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

        // TODO: Override Equals
    }
}