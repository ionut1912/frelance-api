﻿
using Frelance.API.Extensions;
using Frelance.Application.Commands.Projects.CreateProject;
using Frelance.Application.Commands.Projects.DeleteProject;
using Frelance.Application.Commands.Projects.UpdateProject;
using Frelance.Application.Queries.Projects.GetProjectById;
using Frelance.Application.Queries.Projects.GetProjects;
using Frelance.Contracts.Requests.Common;
using Frelance.Contracts.Requests.Projects;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Frelance.API.Modules;

public static class ProjectModule
{
      public static void AddProjectsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/projects", async (IMediator mediator, [FromQuery] int pageSize, [FromQuery] int pageNumber, CancellationToken ct) =>
            {
                var paginatedProjectDtos = await mediator.Send(new GetProjectsQuery
                    (new PaginationParams {PageSize = pageSize, PageNumber = pageNumber}), ct);
                return Results.Extensions.OkPaginationResult(paginatedProjectDtos.PageSize, paginatedProjectDtos.CurrentPage,
                    paginatedProjectDtos.TotalCount, paginatedProjectDtos.TotalPages, paginatedProjectDtos.Items);
            }).WithTags("Projects");
    
            app.MapGet("/api/projects/{id}", async (IMediator mediator, int id, CancellationToken ct) =>
            {
                var project = await mediator.Send(new GetProjectByIdQuery(id), ct);
                return Results.Ok(project);
            }).WithTags("Projects");
    
            app.MapPost("/api/projects", async (IMediator mediator, CreateProjectRequest createProjectRequest,
                CancellationToken ct) =>
            {
                var command = createProjectRequest.Adapt<CreateProjectCommand>();
                var result = await mediator.Send(command, ct);
                return Results.Ok(result);
            }).WithTags("Projects");
    
            app.MapPut("/api/projects/{id}", async (IMediator mediator, int id,
                UpdateProjectRequest updateProjectRequest, CancellationToken ct) =>
            {
                var command = updateProjectRequest.Adapt<UpdateProjectCommand>() with { Id = id };
                var result = await mediator.Send(command, ct);
                return Results.Ok(result);
            }).WithTags("Projects");
    
            app.MapDelete("/api/projects/{id}", async (IMediator mediator, int id, CancellationToken ct) =>
            {
                var command = new DeleteProjectCommand(id);
                var result = await mediator.Send(command, ct);
                return Results.Ok(result);
            }).WithTags("Projects");
        }
}