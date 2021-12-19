using System.Security.Claims;
using Ardalis.ApiEndpoints;
using CleanArchi.Core.Interfaces;
using CleanArchi.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanArchi.Web.Endpoints.AuthEndpoints;

public class Authenticate : BaseAsyncEndpoint
    .WithoutRequest
    .WithResponse<AuthenticateResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenClaimsService _tokenClaimsService;

    public Authenticate(UserManager<ApplicationUser> userManager, ITokenClaimsService tokenClaimsService)
    {
        _userManager = userManager;
        _tokenClaimsService = tokenClaimsService;
    }

    [Authorize(AuthenticationSchemes = Infrastructure.Identity.IdentityConstants.COOCKIE_SCHEME)]
    [HttpGet("/Authenticate")]
    [SwaggerOperation(
        Summary = "Authenticates a user",
        Description = "Authenticates a user",
        OperationId = "auth.authenticate",
        Tags = new[] { "AuthEndpoints" })
    ]
    public override async Task<ActionResult<AuthenticateResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var userName = User.Claims.Single(p => p.Type == ClaimTypes.Name).Value;
        var user = await _userManager.FindByEmailAsync(userName);

        return new AuthenticateResponse
        {
            Token = await _tokenClaimsService.GetTokenAsync(user.UserName),
            Username = user.UserName
        };
    }
}
