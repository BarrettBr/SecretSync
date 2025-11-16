using SecretSync.Application.Dtos;
using SecretSync.Core.Projects;

namespace SecretSync.Application.Services;

public interface IProjectService
{
  Task<ProjectDto> CreateProjectAsync(string name, CancellationToken ct = default);
  Task<ProjectDto?> GetProjectAsync(Guid id, CancellationToken ct = default);
}

public sealed class ProjectService : IProjectService
{
  private readonly IProjectStore _projectStore; // interface from Core

  // Constructor
  public ProjectService(IProjectStore projectStore)
  {
    _projectStore = projectStore;
  }

  public async Task<ProjectDto> CreateProjectAsync(string name, CancellationToken ct = default)
  {
    // Create a domain object, store it, return DTO
    var project = new Project(name); // domain model from Core
    await _projectStore.SaveAsync(project, ct);

    return new ProjectDto(project.Id, project.Name, 0);
  }
  public async Task<ProjectDto?> GetProjectAsync(Guid id, CancellationToken ct = default)
  {
    var project = await _projectStore.LoadAsync(id, ct);
    
    if (project == null)
    {
      return null;
    }

    // Convert to DTO so we don't leak domain model as project could have methods we don't want exposed
    return new ProjectDto(project.Id, project.Name, project.Version);
  }
}
