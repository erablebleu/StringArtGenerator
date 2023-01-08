namespace StringArtGenerator.Common;

public interface ILoader
{
    T Resolve<T>();
}