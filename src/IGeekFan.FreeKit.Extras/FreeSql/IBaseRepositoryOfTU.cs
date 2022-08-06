// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using FreeSql;
using FreeSql.Extensions.EntityUtil;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public interface IBaseRepository<TEntity, TKey, Ukey> : IBaseRepository<TEntity> where TEntity : class
{
    TEntity Get(TKey id, Ukey uid);
    TEntity Find(TKey id, Ukey uid);
    int Delete(TKey id, Ukey uid);
    Task<TEntity> GetAsync(TKey id, Ukey uid, CancellationToken cancellationToken = default);
    Task<TEntity> FindAsync(TKey id, Ukey uid, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(TKey id, Ukey uid, CancellationToken cancellationToken = default);
}

public abstract class BaseRepository<TEntity, TKey, Ukey> : BaseRepository<TEntity>,
    IBaseRepository<TEntity, TKey, Ukey>
    where TEntity : class
{
    protected BaseRepository(IFreeSql fsql, Expression<Func<TEntity, bool>> filter, Func<string, string> asTable = null)
        : base(fsql, filter, asTable)
    {
    }

    TEntity CheckTKeyAndReturnIdEntity(TKey id, Ukey uid)
    {
        var tb = Orm.CodeFirst.GetTableByEntity(EntityType);
        if (tb.Primarys.Length != 2)
            throw new Exception(DbContextStrings.EntityType_PrimaryKeyIsNotOne(EntityType.Name));
        if (tb.Primarys[0].CsType.NullableTypeOrThis() != typeof(TKey).NullableTypeOrThis())
            throw new Exception(DbContextStrings.EntityType_PrimaryKeyError(EntityType.Name, typeof(TKey).FullName));
        if (tb.Primarys[1].CsType.NullableTypeOrThis() != typeof(Ukey).NullableTypeOrThis())
            throw new Exception(DbContextStrings.EntityType_PrimaryKeyError(EntityType.Name, typeof(Ukey).FullName));
        var obj = Activator.CreateInstance(tb.Type);
        Orm.SetEntityValueWithPropertyName(tb.Type, obj, tb.Primarys[0].CsName, id);
        Orm.SetEntityValueWithPropertyName(tb.Type, obj, tb.Primarys[1].CsName, uid);
        var ret = obj as TEntity;
        if (ret == null)
            throw new Exception(DbContextStrings.EntityType_CannotConvert(EntityType.Name, typeof(TEntity).Name));
        return ret;
    }

    public virtual int Delete(TKey id, Ukey uid) => Delete(CheckTKeyAndReturnIdEntity(id, uid));

    public virtual TEntity Find(TKey id, Ukey uid) =>
        base.Select.WhereDynamic(CheckTKeyAndReturnIdEntity(id, uid)).ToOne();

    public TEntity Get(TKey id, Ukey uid) => base.Select.WhereDynamic(CheckTKeyAndReturnIdEntity(id, uid)).ToOne();

    public Task<TEntity> GetAsync(TKey id, Ukey uid, CancellationToken cancellationToken = default)
    {
        return base.Select.WhereDynamic(CheckTKeyAndReturnIdEntity(id, uid)).ToOneAsync(cancellationToken);
    }

    public Task<TEntity> FindAsync(TKey id, Ukey uid, CancellationToken cancellationToken = default)
    {
        return base.Select.WhereDynamic(CheckTKeyAndReturnIdEntity(id, uid)).ToOneAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(TKey id, Ukey uid, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(CheckTKeyAndReturnIdEntity(id, uid), cancellationToken);
    }
}

public class DefaultRepository<TEntity, TKey, Ukey> : BaseRepository<TEntity, TKey, Ukey> where TEntity : class
{
    public DefaultRepository(IFreeSql fsql) : base(fsql, null, null)
    {
    }

    public DefaultRepository(IFreeSql fsql, Expression<Func<TEntity, bool>> filter) : base(fsql, filter, null)
    {
    }

    public DefaultRepository(IFreeSql fsql, UnitOfWorkManager uowManger) : base(uowManger?.Orm ?? fsql, null, null)
    {
        uowManger?.Binding(this);
    }
}