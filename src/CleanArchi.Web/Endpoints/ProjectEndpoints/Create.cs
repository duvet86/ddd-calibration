﻿using Ardalis.ApiEndpoints;
using CleanArchi.Core.ProjectAggregate;
using CleanArchi.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchi.Web.Endpoints.ProjectEndpoints;

public class Create : BaseAsyncEndpoint
    .WithRequest<CreateProjectRequest>
    .WithResponse<CreateProjectResponse>
{
    private readonly IRepository<Project> _repository;

    public Create(IRepository<Project> repository)
    {
        _repository = repository;
    }

    [HttpPost("/Projects")]
    [SwaggerOperation(
        Summary = "Creates a new Project",
        Description = "Creates a new Project",
        OperationId = "Project.Create",
        Tags = new[] { "ProjectEndpoints" })
    ]
    public override async Task<ActionResult<CreateProjectResponse>> HandleAsync(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        if (request.Name == null)
        {
            return BadRequest();
        }

        var newProject = new Project(request.Name);

        var createdItem = await _repository.AddAsync(newProject, cancellationToken);

        var response = new CreateProjectResponse
        (
            id: createdItem.Id,
            name: createdItem.Name
        );

        return Ok(response);
    }
}
