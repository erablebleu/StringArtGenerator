using AutoMapper;

namespace StringArtGenerator.App.Resources.Extensions;

public static class IProfileExpressionExtension
{
    public static void CreateMapWithReverse<TSource, TDestination>(this IProfileExpression profileExpression)
    {
        profileExpression.CreateMap<TSource, TDestination>();
        profileExpression.CreateMap<TDestination, TSource>();
    }
    public static void CreateExtendedMap<TSource, TDestination>(this IProfileExpression profileExpression)
    {
        profileExpression.CreateMap<TSource, TSource>();
        profileExpression.CreateMap<TDestination, TDestination>();
        profileExpression.CreateMap<TSource, TDestination>();
        profileExpression.CreateMap<TDestination, TSource>();
    }
}