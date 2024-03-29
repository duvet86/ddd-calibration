﻿using Ardalis.ApiEndpoints;
using CleanArchi.Core.ProjectAggregate;
using CleanArchi.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchi.Web.Endpoints.ProjectEndpoints;

public class Delete : BaseAsyncEndpoint
    .WithRequest<DeleteProjectRequest>
    .WithoutResponse
{
    private readonly IRepository<Project> _repository;

    public Delete(IRepository<Project> repository)
    {
        _repository = repository;
    }

    [HttpDelete(DeleteProjectRequest.Route)]
    [SwaggerOperation(
        Summary = "Deletes a Project",
        Description = "Deletes a Project",
        OperationId = "Projects.Delete",
        Tags = new[] { "ProjectEndpoints" })
    ]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteProjectRequest request,
        CancellationToken cancellationToken)
    {
        var aggregateToDelete = await _repository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (aggregateToDelete == null)
        {
            return NotFound();
        }

        await _repository.DeleteAsync(aggregateToDelete, cancellationToken);

        return NoContent();
    }
}
