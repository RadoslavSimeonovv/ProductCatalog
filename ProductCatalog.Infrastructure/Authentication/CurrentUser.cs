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

    public bool IsAuthenticated 
        => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? UserId 
        => _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

    public string? Email 
        => _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");
}
