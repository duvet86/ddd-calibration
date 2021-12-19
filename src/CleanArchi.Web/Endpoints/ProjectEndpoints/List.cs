using Ardalis.ApiEndpoints;
using CleanArchi.Core.ProjectAggregate;
using CleanArchi.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchi.Web.Endpoints.ProjectEndpoints;

public class List : BaseAsyncEndpoint
    .WithoutRequest
    .WithResponse<ProjectListResponse>
{
    private readonly IReadRepository<Project> _repository;

    public List(IReadRepository<Project> repository)
    {
        _repository = repository;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("/Projects")]
    [SwaggerOperation(
      Summary = "Gets a list of all Projects",
      Description = "Gets a list of all Projects",
      OperationId = "Project.List",
      Tags = new[] { "ProjectEndpoints" })
  ]
    public override async Task<ActionResult<ProjectListResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var projects = await _repository.ListAsync(cancellationToken);

        var response = new ProjectListResponse
        {
            Projects = projects
                .Select(project => new ProjectRecord(project.Id, project.Name))
                .ToList()
        };

        return Ok(response);
    }
}
