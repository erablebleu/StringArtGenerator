using System.Reflection;

namespace StringArtGenerator.Injection;

internal class ServiceInfo
{
    public Type[] ConstructorArgs { get; set; }
    public Type ImplementationType { get; set; }
    public PropertyInfo[] InjectableProperties { get; set; }
    public bool IsDisposable { get; set; }
    public RegistrationMode RegistrationMode { get; set; }
    public object? Singleton { get; set; }
}