using Microsoft.AspNetCore.Http;
using ProductCatalog.Application.Abstractions.Authentication;
using System.Security.Claims;

namespace ProductCatalog.Infrastructure.Authentication;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated
        => Principal?.Identity?.IsAuthenticated ?? false;

    public string? UserId
        => Principal?.FindFirstValue("sub") ??
           Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email
        => Principal?.FindFirstValue("email") ??
           Principal?.FindFirstValue(ClaimTypes.Email);

    public bool IsInRole(string role)
        => Principal?.IsInRole(role) ?? false;
}
