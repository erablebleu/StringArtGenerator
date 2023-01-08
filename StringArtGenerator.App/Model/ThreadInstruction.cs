using StringArtGenerator.App.Resources.Enums;
using StringArtGenerator.App.Resources.Extensions;
using System.Windows;

namespace StringArtGenerator.App.Model;

public class ThreadInstruction
{
    public int Nail1Index { get; set; }
    public TangentSide Nail1Out { get; set; }
    public Point Nail1RealPos { get; set; }
    public TangentSide Nail2In { get; set; }
    public int Nail2Index { get; set; }
    public Point Nail2RealPos { get; set; }
    public int ThreadIndex { get; set; }

    public ThreadInstruction Reverse()
    {
        (Nail1Index, Nail2Index) = (Nail2Index, Nail1Index);
        (Nail1RealPos, Nail2RealPos) = (Nail2RealPos, Nail1RealPos);
        Nail1Out = Nail1Out.Reverse();
        Nail2In = Nail2In.Reverse();
        return this;
    }
}