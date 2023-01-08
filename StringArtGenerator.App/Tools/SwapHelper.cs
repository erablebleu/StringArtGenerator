namespace StringArtGenerator.App.Tools;

public static class SwapHelper
{
    public static void Swap<T>(ref T v1, ref T v2)
    {
        (v2, v1) = (v1, v2);
    }
}