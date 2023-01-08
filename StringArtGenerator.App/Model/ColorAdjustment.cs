namespace StringArtGenerator.App.Model;

public class ColorAdjustment : Activable
{
    public double[,] ColorMatrix { get; set; } = new double[3, 3]
    {
        {0.30d, 0.30d, 0.30d },
        {0.59d, 0.59d, 0.59d },
        {0.11d, 0.11d, 0.11d },
    };
}