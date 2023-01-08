using System;

namespace StringArtGenerator.App.Resources.Extensions;

public static class ArrayExtension
{
    public static T2[,] Select<T1, T2>(this T1[,] data, Func<T1, T2> predicate)
    {
        T2[,] result = new T2[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
            for (int j = 0; j < data.GetLength(1); j++)
                result[i, j] = predicate(data[i, j]);
        return result;
    }
    public static T2[,] Select<T1, T2>(this T1[,] data, Func<int, int, T2> predicate)
    {
        T2[,] result = new T2[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
            for (int j = 0; j < data.GetLength(1); j++)
                result[i, j] = predicate(i, j);
        return result;
    }
}