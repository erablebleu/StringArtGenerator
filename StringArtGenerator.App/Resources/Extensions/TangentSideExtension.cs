using StringArtGenerator.App.Resources.Enums;
using System;

namespace StringArtGenerator.App.Resources.Extensions;

public static class TangentSideExtension
{
    public static TangentSide Reverse(this TangentSide side) => side switch
    {
        TangentSide.Left => TangentSide.Right,
        TangentSide.Right => TangentSide.Left,
        _ => throw new NotSupportedException()
    };
}