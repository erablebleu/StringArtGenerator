using StringArtGenerator.App.Adapters;
using StringArtGenerator.App.Tools.Serialization;

namespace StringArtGenerator.App.ApplicationServices;

public class ProjectApplicationService : ApplicationServiceBase
{
    private ProjectAdapter _currentProject = new();
    public ProjectAdapter CurrentProject { get => _currentProject; set => Set(ref _currentProject, value); }

    public void Open(string filePath)
    {
        Model.Project project = JsonSerializer.Deserialize<Model.Project>(System.IO.File.ReadAllText(filePath));
        CurrentProject = Mapper.Map<ProjectAdapter>(project);
        CurrentProject.FilePath = filePath;
    }
    public void Save(string filePath)
    {
        System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(Mapper.Map<Model.Project>(CurrentProject)));
        CurrentProject.FilePath = filePath;
    }
}