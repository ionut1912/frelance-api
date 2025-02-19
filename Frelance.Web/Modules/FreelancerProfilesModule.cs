using Frelance.Application.Mediatr.Commands.ClientProfiles;
using Frelance.Application.Mediatr.Commands.FreelancerProfiles;
using Frelance.Application.Mediatr.Queries.FreelancerProfiles;
using Frelance.Contracts.Dtos;
using Frelance.Contracts.Requests.Common;
using Frelance.Contracts.Requests.FreelancerProfiles;
using Frelance.Contracts.Requests.Skills;
using Frelance.Web.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Frelance.Web.Modules;

public static class FreelancerProfilesModule
{
    public static void AddFreelancerProfilesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/freelancerProfiles",
                async (IMediator mediator, CreateFreelancerProfileRequest createFreelancerProfileRequest,
                    CancellationToken ct) =>
                {

                    var result =
                        await mediator.Send(createFreelancerProfileRequest.Adapt<CreateFreelancerProfileCommand>(), ct);
                    return Results.Ok(result);
                })
            .WithTags("FreelancerProfiles")
            .RequireAuthorization("FreelancerRole");
        app.MapGet("/api/freelancerProfiles/{id}", async (IMediator mediator, int id, CancellationToken ct) =>
            {
                var freelancerProfile = await mediator.Send(new GetFreelancerProfileByIdQuery(id), ct);
                return Results.Ok(freelancerProfile);
            }).WithTags("FreelancerProfiles").
            RequireAuthorization();
        app.MapGet("/api/freelancerProfiles", async (IMediator mediator, [FromQuery] int pageSize, [FromQuery] int pageNumber, CancellationToken ct) =>
            {
                var paginatedFreelancerProfiles = await mediator.Send(new GetFreelancerProfilesQuery
                    (new PaginationParams { PageSize = pageSize, PageNumber = pageNumber }), ct);
                return Results.Extensions.OkPaginationResult(paginatedFreelancerProfiles.PageSize, paginatedFreelancerProfiles.CurrentPage,
                    paginatedFreelancerProfiles.TotalCount, paginatedFreelancerProfiles.TotalPages, paginatedFreelancerProfiles.Items);
            }).WithTags("FreelancerProfiles").
            RequireAuthorization();

        app.MapGet("/api/current/freelancerProfiles",
                async (IMediator mediator, CancellationToken ct) =>
                {
                    var freelancerProfile = await mediator.Send(new GetLoggedInFreelancerProfileQuery(), ct);
                    return Results.Ok(freelancerProfile);
                }).WithTags("FreelancerProfiles")
            .RequireAuthorization("FreelancerRole");

        app.MapPut("/api/freelancerProfiles/{id}", async (IMediator mediator, int id,
                         UpdateFreelancerProfileRequest updateFreelancerProfileRequest, CancellationToken ct) =>
                    {
                        var command = updateFreelancerProfileRequest.Adapt<UpdateFreelancerProfileCommand>() with { Id = id };
                        var result = await mediator.Send(command, ct);
                        return Results.Ok(result);
                    }).WithTags("FreelancerProfiles").
                    RequireAuthorization("FreelancerRole");
        app.MapDelete("/api/freelancerProfiles/{id}", async (IMediator mediator, int id, CancellationToken ct) =>
            {
                var command = new DeleteFreelancerProfileCommand(id);
                var result = await mediator.Send(command, ct);
                return Results.Ok(result);
            }).WithTags("FreelancerProfiles").
            RequireAuthorization("FreelancerRole");
    }
}