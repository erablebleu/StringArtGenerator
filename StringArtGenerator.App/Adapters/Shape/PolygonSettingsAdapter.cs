using StringArtGenerator.App.Model;
using StringArtGenerator.App.Tools;
using System.Collections.Generic;
using System.Linq;

namespace StringArtGenerator.App.Adapters.Shape;

public class PolygonSettingsAdapter : ShapeSettingsAdapter
{
    private double _diameter = 700;
    private int _edgeCount = 6;
    private int _edgeNailCount = 50;
    private bool _excludeSameSideLine = true;
    private bool _excludeVertex = false;
    private bool _topEdge;

    public double Diameter { get => _diameter; set => Set(ref _diameter, value); }
    public int EdgeCount { get => _edgeCount; set => Set(ref _edgeCount, value); }
    public int EdgeNailCount { get => _edgeNailCount; set => Set(ref _edgeNailCount, value); }
    public bool ExcludeSameSideLine { get => _excludeSameSideLine; set => Set(ref _excludeSameSideLine, value); }
    public bool ExcludeVertex { get => _excludeVertex; set => Set(ref _excludeVertex, value); }
    public bool TopEdge { get => _topEdge; set => Set(ref _topEdge, value); }
    public override string Name => "Polygon";

    public override NailMap GetNailMap()
    {
        NailMap result = new()
        {
            Nails = ShapeGenerator.GetPolygon(EdgeCount, EdgeNailCount, ExcludeVertex, Diameter, TopEdge).Select(p => new Nail { Diameter = NailDiameter, Position = p }).ToList(),
        };
        HashSet<long> lines = new();

        for (int i = 0; i < result.Nails.Count; i++)
        {
            int iEdge = i / EdgeNailCount;
            for (int j = i + 1; j < i + result.Nails.Count; j++)
            {
                int jdx = j % (EdgeCount * EdgeNailCount);
                int jEdge = jdx / EdgeNailCount;
                bool iIsVertex = i % EdgeNailCount == 0;
                bool jIsVertex = j % EdgeNailCount == 0;
                long l = GetHash(i, jdx);

                if (ExcludeSameSideLine && (iEdge == jEdge || !ExcludeVertex
                        && (jIsVertex && jEdge == (iEdge + 1) % EdgeCount
                           || iIsVertex && iEdge == (jEdge + 1) % EdgeCount))
                    || lines.Contains(l))
                    continue;

                lines.Add(l);
            }
        }
        result.Lines = lines.Select(l => GetLine(l)).ToList();

        return result;
    }
}