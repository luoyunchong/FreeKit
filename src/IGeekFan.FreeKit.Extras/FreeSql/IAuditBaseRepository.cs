using FreeSql;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public interface IAuditBaseRepository<TEntity> : IBaseRepository<TEntity, Guid> where TEntity : class
{
}

public interface IAuditBaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
{
}