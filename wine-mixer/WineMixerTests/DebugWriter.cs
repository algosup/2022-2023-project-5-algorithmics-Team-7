using System.Diagnostics;
using System.Text;

namespace WineMixerTests;

class DebugWriter : TextWriter
{
    public TextWriter stdOut = Console.Out;

    public override void WriteLine(string? value)
    {
        Debug.WriteLine(value);
        stdOut.WriteLine(value);
        base.WriteLine(value);
    }

    public override void Write(string? value)
    {
        Debug.Write(value);
        stdOut.Write(value);
        base.Write(value);
    }

    public override Encoding Encoding
        => Encoding.Unicode; 
}