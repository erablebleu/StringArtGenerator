using StringArtGenerator.App.Model;
using StringArtGenerator.App.Tools;
using System.Collections.Generic;
using System.Linq;

namespace StringArtGenerator.App.Adapters.Shape;

public class RectangleSettingsAdapter : ShapeSettingsAdapter
{
    private double _height = 350;
    private double _width = 700;
    private int _xNailCount = 100;
    private int _yNailCount = 50;
    private bool _excludeSameSideLine = true;
    private bool _excludeVertex = false;

    public double Height { get => _height; set => Set(ref _height, value); }
    public double Width { get => _width; set => Set(ref _width, value); }
    public int XNailCount { get => _xNailCount; set => Set(ref _xNailCount, value); }
    public int YNailCount { get => _yNailCount; set => Set(ref _yNailCount, value); }
    public bool ExcludeSameSideLine { get => _excludeSameSideLine; set => Set(ref _excludeSameSideLine, value); }
    public bool ExcludeVertex { get => _excludeVertex; set => Set(ref _excludeVertex, value); }
    public override string Name => "Rectangle";

    public override NailMap GetNailMap()
    {
        NailMap result = new()
        {
            Nails = ShapeGenerator.GetRectangle(XNailCount, YNailCount, Width, Height, ExcludeVertex).Select(p => new Nail { Diameter = NailDiameter, Position = p }).ToList(),
        };
        HashSet<long> lines = new();

        int GetEdge(int idx)
        {
            if (idx < XNailCount) return 0;
            if (idx < XNailCount + YNailCount) return 1;
            if (idx < 2 * XNailCount + YNailCount) return 2;
            return 3;
        }

        bool IsEdge(int idx)
        {
            return idx == 0 || idx == XNailCount || idx == XNailCount + YNailCount || idx == 2 * XNailCount + YNailCount;
        }
        
        for (int i = 0; i < result.Nails.Count; i++)
        {
            int iEdge = GetEdge(i);
            for (int j = i + 1; j < i + result.Nails.Count; j++)
            {
                int jdx = j % result.Nails.Count;
                int jEdge = GetEdge(jdx);
                bool iIsVertex = IsEdge(i);
                bool jIsVertex = IsEdge(jdx);
                long l = GetHash(i, jdx);

                if (ExcludeSameSideLine && (iEdge == jEdge || !ExcludeVertex
                        && (jIsVertex && jEdge == (iEdge + 1) % 4
                           || iIsVertex && iEdge == (jEdge + 1) % 4))
                    || lines.Contains(l))
                    continue;

                lines.Add(l);
            }
        }
        result.Lines = lines.Select(l => GetLine(l)).ToList();

        return result;
    }
}