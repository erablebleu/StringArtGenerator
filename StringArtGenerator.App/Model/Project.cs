using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Model;

public class Project
{
    public CalculationOptions CalculationOptions { get; set; } = new();
    public CalculationResult CalculationResult { get; set; } = new();
    public NailMap NailMap { get; set; } = new();
    public ImageOptions ImageOptions { get; set; } = new();
    public BitmapImage? SourceImage { get; set; }
    public double[] Spectrum { get; set; } = Array.Empty<double>();
    public Stepper Stepper { get; set; } = new();
    public List<Thread> Threads { get; set; } = new();
}