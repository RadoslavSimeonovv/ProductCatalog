using System.Data;

namespace ProductCatalog.Application.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}