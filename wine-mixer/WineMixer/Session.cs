namespace WineMixer
{
    public class Session
    {
        public List<State> States { get; set; }
        public List<Transfer> Transfers { get; set; }
        public List<double> TargetMix { get; set; }
        public List<int> TankSizes { get; set; }
        public Options Options { get; set; }
    }
}