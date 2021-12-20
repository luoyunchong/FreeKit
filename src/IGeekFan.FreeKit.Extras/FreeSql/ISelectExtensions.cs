using FreeSql;
using FreeSql.Internal.Model;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public static class ISelectExtensions
{
    public static bool AddIfNotContains<T>(this ICollection<T> @this, T value)
    {
        if (!@this.Contains(value))
        {
            @this.Add(value);
            return true;
        }

        return false;
    }
    public static ISelect<T> AsTable<T>(this ISelect<T> @this, string tableName, int count) where T : class
    {
        string[] tableNames = Array.Empty<string>();
        for (int i = 0; i < count; i++)
        {
            tableNames.AddIfNotContains($"{tableName}_{i}");
        }

        @this.AsTable(tableNames);
        return @this;
    }

    public static ISelect<T> AsTable<T>(this ISelect<T> @this, params string[] tableNames) where T : class
    {
        tableNames?.ToList().ForEach(tableName =>
        {
            @this.AsTable((type, oldname) =>
            {
                if (type == typeof(T)) return tableName;
                return null;
            });
        });
        return @this;
    }
}
