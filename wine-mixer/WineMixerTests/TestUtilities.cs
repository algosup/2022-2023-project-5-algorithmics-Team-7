using WineMixer;

namespace WineMixerTests;

public static class TestUtilities
{
    public static void SaveDefaultOptions()
    {

    }

    public static Random rng = new Random();

    public static Mix CreateRandomMix(int cnt)
    {
        var val = 1.0 / cnt;
        var tmp = Enumerable.Repeat(val, cnt).ToArray();

        for (var i = 0; i < cnt; ++i)
        {
            var a = rng.Next(cnt);
            var b = rng.Next(cnt);
            var d = rng.NextDouble() * tmp[a];
            tmp[a] -= d;
            tmp[b] += d;
        }

        return new Mix(tmp);
    }
    
    [Test]
    public static void OutputRandomMixes()
    {
        for (var i = 5; i < 50; ++i)
        {
            var mix = CreateRandomMix(i);
            Console.WriteLine(mix);
        }
    }
}