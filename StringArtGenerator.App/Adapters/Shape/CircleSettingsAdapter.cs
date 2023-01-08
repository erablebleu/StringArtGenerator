using StringArtGenerator.App.Model;
using StringArtGenerator.App.Tools;
using System.Collections.Generic;
using System.Linq;

namespace StringArtGenerator.App.Adapters.Shape;

public class CircleSettingsAdapter : ShapeSettingsAdapter
{
    private int _deadZone;
    private double _diameter = 700;
    private int _nailCount = 300;

    public int DeadZone { get => _deadZone; set => Set(ref _deadZone, value); }
    public double Diameter { get => _diameter; set => Set(ref _diameter, value); }
    public int NailCount { get => _nailCount; set => Set(ref _nailCount, value); }
    public override string Name => "Circle";

    public override NailMap GetNailMap()
    {
        NailMap result = new()
        {
            Nails = ShapeGenerator.GetCircle(NailCount, Diameter).Select(p => new Nail { Diameter = NailDiameter, Position = p }).ToList(),
        };
        HashSet<long> lines = new();

        for (int i = 0; i < result.Nails.Count; i++)
        {
            for (int j = i + 1 + DeadZone; j < i + result.Nails.Count - DeadZone; j++)
            {
                int jdx = j % NailCount;
                long l = GetHash(i, jdx);

                if (lines.Contains(l))
                    continue;

                lines.Add(l);
            }
        }
        result.Lines = lines.Select(l => GetLine(l)).ToList();

        return result;
    }
}