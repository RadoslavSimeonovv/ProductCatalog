namespace ProductCatalog.Api.Common;

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string CustomerOnly = "CustomerOnly";
    public const string AdminOrCustomer = "AdminOrCustomer";
}
