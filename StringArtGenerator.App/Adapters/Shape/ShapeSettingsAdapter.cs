using AutoMapper;
using StringArtGenerator.App.Model;
using System;

namespace StringArtGenerator.App.Adapters.Shape;

public abstract class ShapeSettingsAdapter : AdapterBase
{
    private double _nailDiameter = 1.8;


    protected static long GetHash(int n1, int n2) => Math.Min(n1, n2) | ((long)Math.Max(n1, n2) << 32);
    protected static LineInstruction GetLine(long hash) => new() { Nail1Index = (int)hash, Nail2Index = (int)(hash >> 32) };

    public double NailDiameter { get => _nailDiameter; set => Set(ref _nailDiameter, value); }
    public abstract string Name { get; }

    public abstract NailMap GetNailMap();
    public NailMapAdapter GetNailMapAdapter(IMapper mapper) => mapper.Map<NailMapAdapter>(GetNailMap());
}