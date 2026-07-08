using MediatR;
using UserService.Application.Users.Commands.RegisterUser;

namespace UserService.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/users");

        group.MapPost("/register", async (RegisterUserCommand cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.IsSuccess
                ? Results.Created($"/v1/users/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
