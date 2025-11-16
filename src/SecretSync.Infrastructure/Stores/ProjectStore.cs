using System.Collections.Concurrent;
using SecretSync.Core.Projects;

namespace SecretSync.Infrastructure.Stores;

public sealed class ProjectStore : IProjectStore
{
  // In-memory store for starting testing; replace with database access later
  private readonly ConcurrentDictionary<Guid, Project> _projects = new();

  public Task SaveAsync(Project project, CancellationToken ct = default)
  {
    _projects[project.Id] = project;
    return Task.CompletedTask;
  }

  public Task<Project?> LoadAsync(Guid id, CancellationToken ct = default)
  {
    _projects.TryGetValue(id, out var project);
    return Task.FromResult(project);
  }
}
