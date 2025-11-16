namespace SecretSync.Core.Projects;

public interface IProjectStore
{
  Task SaveAsync(Project project, CancellationToken ct = default); // Save or update a project
  Task<Project?> LoadAsync(Guid id, CancellationToken ct = default); // Load a project by ID
}

public sealed class Project
{
  public Guid Id { get; }
  public string Name { get; private set; }
  public int Version { get; private set; }

  // Constructor
  public Project(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Project name cannot be empty.", nameof(name)); 
    }

    Id = Guid.NewGuid();
    Name = name.Trim();
    Version = 0;
  }
}
