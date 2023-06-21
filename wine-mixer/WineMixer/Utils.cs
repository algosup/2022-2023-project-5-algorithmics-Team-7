using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineMixer
{
    public static class Utils
    {
        public static void TimeIt(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            Debug.WriteLine($"Time elapsed = {sw.Elapsed.TotalSeconds:0.####}");
        }
    }
}
