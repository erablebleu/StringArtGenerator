using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StringArtGenerator.App.Model;

public class NailMap
{
    public List<LineInstruction> Lines { get; set; } = new();
    public List<Nail> Nails { get; set; } = new();
    public Point Position { get; set; } = new();
    public double Scale { get; set; } = 1;

    public IEnumerable<Nail> GetTargets(Nail nail)
    {
        int idx = Nails.IndexOf(nail);
        return Lines.Where(l => l.Nail1Index == idx).Select(l => l.Nail2Index)
            .Concat(Lines.Where(l => l.Nail2Index == idx).Select(l => l.Nail1Index)).Select(i => Nails[i]);
    }
    public IEnumerable<int> GetTargets(int idx)
    {
        return Lines.Where(l => l.Nail1Index == idx).Select(l => l.Nail2Index)
            .Concat(Lines.Where(l => l.Nail2Index == idx).Select(l => l.Nail1Index));
    }
}