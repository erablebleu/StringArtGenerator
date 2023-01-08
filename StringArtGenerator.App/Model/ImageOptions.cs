using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StringArtGenerator.App.Model;

public abstract class ActivableOption
{
    public bool IsEnabled { get; set; }
    public BitmapImage? ResultImage { get; set; }
}
public class OpacityOption : ActivableOption
{
    public double EvolutionPow { get; set; } = 1; // degree use in a ^x
    public double Length { get; set; } = 100; // in mm to nails
}
public class BlurOption : ActivableOption
{
    public double EvolutionPow { get; set; } = 1; // degree use in a ^x
    public double Length { get; set; } = 100; // in mm to nails
    public double Diameter { get; set; } = 20; // in px
}
public class SizeOption : ActivableOption
{
    public double MaxPPMM { get; set; } = 1;
}
public class ImageOptions
{
    public BlurOption BlurOption { get; set; } = new();
    public OpacityOption OpacityOption { get; set; } = new();
    public SizeOption SizeOption { get; set; } = new();
}
