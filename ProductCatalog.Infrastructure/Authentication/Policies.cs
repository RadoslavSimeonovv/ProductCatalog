namespace ProductCatalog.Infrastructure.Authentication;

internal static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string CustomerOnly = "CustomerOnly";
    public const string AdminOrCustomer = "AdminOrCustomer";
}
