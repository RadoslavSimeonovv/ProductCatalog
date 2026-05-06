using System.Data;
using static Dapper.SqlMapper;

namespace ProductCatalog.Infrastructure.Data;

internal sealed class StringEnumTypeHandler<T> : TypeHandler<T>
    where T : struct, Enum
{
    public override void SetValue(IDbDataParameter parameter, T value)
        => parameter.Value = value.ToString();

    public override T Parse(object value)
        => Enum.Parse<T>((string)value, ignoreCase: true);
}
