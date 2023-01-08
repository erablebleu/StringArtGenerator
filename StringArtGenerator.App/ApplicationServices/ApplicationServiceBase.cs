using AutoMapper;
using StringArtGenerator.Common;

namespace StringArtGenerator.App.ApplicationServices;

public class ApplicationServiceBase : Adapters.AdapterBase
{
    [Injectable] public IMapper Mapper { get; set; }
}