using Microsoft.AspNetCore.Mvc;
using SecretSync.Application.Services;

namespace SecretSync.Server.Controllers;

[ApiController]
[Route("api/[controller]")] // gets from controller name so currently /api/SecretSync
public class SecretSyncController : ControllerBase
{
  private readonly IProjectService _projectservice;
  
  // Constructor
  public SecretSyncController(IProjectService projectService)
  {
    _projectservice = projectService;
  }

  // POST to /api/SecretSync
  [HttpPost]
  [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<ProjectResponse>> CreateProject(CreateProjectRequest request, CancellationToken ct)
  {
    // For simplicity just using the request string as the project name
    if (string.IsNullOrWhiteSpace(request.Name))
    {
      return BadRequest("Project name cannot be empty.");
    }

    var project = await _projectservice.CreateProjectAsync(request.Name.Trim(), ct);

    var response = new ProjectResponse
    {
      ProjectId = project.Id,
      Name = project.Name,
      Version = 0
    };

    return CreatedAtAction(nameof(GetProject), new { id = response.ProjectId }, response);
  }

  [HttpGet("{id:guid}")]
  public async Task<ActionResult<ProjectResponse>> GetProject(Guid id, CancellationToken ct)
  {
    var project = await _projectservice.GetProjectAsync(id, ct);
    
    if (project == null)
    {
      return NotFound();
    }
    
    var response = new ProjectResponse
    {
      ProjectId = project.Id,
      Name = project.Name,
      Version = project.Version
    };

    return Ok(response);
  }
}

public sealed class CreateProjectRequest
{
  public string Name { get; set; } = string.Empty;
}

public sealed class ProjectResponse
{
  public Guid ProjectId { get; set; }
  public string Name { get; set; } = string.Empty;
  public int Version { get; set; }
}

