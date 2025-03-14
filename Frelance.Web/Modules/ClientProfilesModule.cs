using Frelance.Application.Mediatr.Commands.UserProfile;
using Frelance.Application.Mediatr.Queries.UserProfile;
using Frelance.Contracts.Enums;
using Frelance.Contracts.Requests.ClientProfile;
using Frelance.Contracts.Requests.Common;
using Frelance.Web.Extensions;
using Frelance.Web.Modules.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Frelance.Web.Modules;

public static class ClientProfilesModule
{
    public static void AddClientProfilesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/clientProfiles",
                async (IMediator mediator, CreateClientProfileRequest createClientProfileRequest,
                    CancellationToken ct) =>
                {
                    await mediator.Send(new CreateUserProfileCommand(Role.Client, createClientProfileRequest), ct);
                    return Results.Created();
                })
            .WithTags("ClientProfiles")
            .RequireAuthorization("ClientRole");
        app.MapGet("/api/clientProfiles/{id:int}",
                async (IMediator mediator, int id, HttpContext httpContext, CancellationToken ct) =>
                {
                    var commandRole = ModulesUtils.GetRole(httpContext);
                    var result = await mediator.Send(new GetUserProfileByIdQuery(commandRole, id), ct);
                    return Results.Ok(result);
                }).WithTags("ClientProfiles")
            .RequireAuthorization();
        app.MapGet("/api/clientProfiles",
                async (IMediator mediator, HttpContext httpContext, [FromQuery] int pageSize,
                    [FromQuery] int pageNumber,
                    CancellationToken ct) =>
                {
                    var commandRole = ModulesUtils.GetRole(httpContext);
                    var paginatedResult =
                        await mediator.Send(
                            new GetUserProfilesQuery(commandRole,
                                new PaginationParams { PageSize = pageSize, PageNumber = pageNumber }), ct);
                    return Results.Extensions.OkPaginationResult(paginatedResult.PageSize,
                        paginatedResult.CurrentPage,
                        paginatedResult.TotalCount, paginatedResult.TotalPages,
                        paginatedResult.Items);
                }).WithTags("ClientProfiles")
            .RequireAuthorization();
        app.MapGet("/api/current/clientProfiles",
                async (IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
                {
                    var result = await mediator.Send(new GetCurrentUserProfileQuery(ModulesUtils.GetRole(httpContext)),
                        ct);
                    return Results.Ok(result);
                }).WithTags("ClientProfiles")
            .RequireAuthorization();
        app.MapPut("/api/clientProfiles/{id:int}", async (IMediator mediator, int id,
                UpdateClientProfileRequest updateClientProfileRequest, CancellationToken ct) =>
            {
                await mediator.Send(new UpdateUserProfileCommand(id, Role.Client, updateClientProfileRequest), ct);
                return Results.NoContent();
            }).WithTags("ClientProfiles")
            .RequireAuthorization("ClientRole");

        app.MapPatch("/api/clientProfiles/verify/{id:int}", async (IMediator mediator, int id, CancellationToken ct) =>
            {
                await mediator.Send(new VerifyUserProfileCommand(id, Role.Client), ct);
                return Results.NoContent();
            }).WithTags("ClientProfiles")
            .RequireAuthorization("ClientRole");

        app.MapDelete("/api/clientProfiles/{id:int}", async (IMediator mediator, int id, CancellationToken ct) =>
            {
                await mediator.Send(new DeleteUserProfileCommand(Role.Client, id), ct);
                return Results.NoContent();
            }).WithTags("ClientProfiles")
            .RequireAuthorization("ClientRole");
    }
}