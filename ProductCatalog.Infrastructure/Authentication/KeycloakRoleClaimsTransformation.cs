using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace ProductCatalog.Infrastructure.Authentication;

internal sealed class KeycloakRoleClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var realmAccessClaim = principal.FindFirstValue("realm_access");

        if (string.IsNullOrEmpty(realmAccessClaim))
            return Task.FromResult(principal);

        using var document = JsonDocument.Parse(realmAccessClaim);

        if (!document.RootElement.TryGetProperty("roles", out var roles))
            return Task.FromResult(principal);

        var identity = new ClaimsIdentity();

        foreach (var role in roles.EnumerateArray())
        {
            var roleValue = role.GetString();
            if (!string.IsNullOrEmpty(roleValue))
                identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
        }

        principal.AddIdentity(identity);

        return Task.FromResult(principal);
    }
}
