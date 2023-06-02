namespace WineMixer
{
    class Manager
    {
        public double[] Sizes { get; set; }
        public double[] Recipe { get; set; }

        public Manager(double[] sizes, double[] recipe)
        {
            Sizes = sizes;
            Recipe = recipe;
        }
    }
}