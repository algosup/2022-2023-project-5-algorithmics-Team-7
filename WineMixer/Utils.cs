namespace WineMixer
{
    class Utils
    {
        public static bool IsClose(double a, double b, double epsilon = 1e-8)
            => Math.Abs(a - b) < epsilon;
    }
}