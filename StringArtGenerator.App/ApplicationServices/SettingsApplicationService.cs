using AutoMapper;
using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Model;
using StringArtGenerator.App.Tools.Serialization;
using System.IO;

namespace StringArtGenerator.App.ApplicationServices;

public class SettingsApplicationService : ApplicationServiceBase
{
    private const string SettingsFilePath = "settings.json";
    private SettingsAdapter _settings = new();

    public SettingsAdapter Settings { get => _settings; private set => Set(ref _settings, value); }

    public SettingsApplicationService(IMapper mapper)
    {
        if(File.Exists(SettingsFilePath))
            Settings = mapper.Map<SettingsAdapter>(JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFilePath)));
    }

    public void Save(SettingsAdapter adapter)
    {
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(Mapper.Map<Settings>(adapter)));
        Settings = adapter;
    }
}
