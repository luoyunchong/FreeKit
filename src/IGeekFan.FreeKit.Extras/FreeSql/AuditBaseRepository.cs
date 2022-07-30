using System.Linq.Expressions;
using FreeSql;
using IGeekFan.FreeKit.Extras.AuditEntity;
using IGeekFan.FreeKit.Extras.Extensions;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.Extras.FreeSql;

/// <summary>
/// 审计仓储：实现如果实体类
/// 继承了ICreateAuditEntity  则自动增加创建时间/人信息
/// 继承了IUpdateAuditEntity，更新时，修改更新时间/人
/// 继承了IDeleteAuditEntity，删除时，自动改成软删除。仅注入此仓储或继承此仓储的实现才能实现如上功能。
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class AuditBaseRepository<TEntity, TKey> : DefaultRepository<TEntity, TKey>,
    IAuditBaseRepository<TEntity, TKey>
    where TEntity : class, new()
{
    /// <summary>
    ///  当前登录人信息
    /// </summary>
    protected readonly ICurrentUser CurrentUser;

    protected readonly bool isDeleteAudit;
    protected readonly bool isUpdateAudit;

    public AuditBaseRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(
        unitOfWorkManager?.Orm, unitOfWorkManager)
    {
        CurrentUser = currentUser;
        isDeleteAudit = typeof(TEntity).HasImplementedRawGeneric(typeof(ISoftDelete)) ||
                        typeof(TEntity).HasImplementedRawGeneric(typeof(IDeleteAuditEntity<>));
        isUpdateAudit = typeof(TEntity).HasImplementedRawGeneric(typeof(IUpdateAuditEntity<>));
    }

    protected abstract void BeforeInsert(TEntity entity);
    protected abstract void BeforeUpdate(TEntity entity);
    protected abstract void BeforeDelete(TEntity entity);

    #region Insert

    public override TEntity Insert(TEntity entity)
    {
        BeforeInsert(entity);
        return base.Insert(entity);
    }

    public override Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        BeforeInsert(entity);
        return base.InsertAsync(entity, cancellationToken);
    }

    public override List<TEntity> Insert(IEnumerable<TEntity> entities)
    {
        foreach (TEntity entity in entities)
        {
            BeforeInsert(entity);
        }

        return base.Insert(entities);
    }

    public override Task<List<TEntity>> InsertAsync(IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        foreach (TEntity entity in entities)
        {
            BeforeInsert(entity);
        }

        return base.InsertAsync(entities, cancellationToken);
    }

    #endregion

    #region Update

    public override int Update(TEntity entity)
    {
        BeforeUpdate(entity);
        return base.Update(entity);
    }

    public override Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        BeforeUpdate(entity);
        return base.UpdateAsync(entity, cancellationToken);
    }

    public override int Update(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            BeforeUpdate(entity);
        }

        return base.Update(entities);
    }

    public override Task<int> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            BeforeUpdate(entity);
        }

        return base.UpdateAsync(entities, cancellationToken);
    }

    #endregion

    #region Delete

    public override int Delete(TKey id)
    {
        if (!isDeleteAudit) return base.Delete(id);
        TEntity entity = Get(id);
        BeforeDelete(entity);
        return base.Update(entity);
    }

    public override int Delete(TEntity entity)
    {
        if (!isDeleteAudit) return base.Delete(entity);
        BeforeDelete(entity);
        return base.Update(entity);
    }

    public override int Delete(IEnumerable<TEntity> entities)
    {
        if (!entities.Any() || !isDeleteAudit) return base.Delete(entities);
        Attach(entities);
        foreach (TEntity entity in entities)
        {
            BeforeDelete(entity);
        }

        return base.Update(entities);
    }

    public override async Task<int> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        if (!isDeleteAudit) return await base.DeleteAsync(id, cancellationToken);
        TEntity entity = await GetAsync(id, cancellationToken);
        BeforeDelete(entity);
        return await base.UpdateAsync(entity, cancellationToken);
    }

    public override Task<int> DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (!entities.Any() || !isDeleteAudit) return base.DeleteAsync(entities, cancellationToken);
        Attach(entities);
        foreach (TEntity entity in entities)
        {
            BeforeDelete(entity);
        }

        return base.UpdateAsync(entities, cancellationToken);
    }

    public override Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!isDeleteAudit) return base.DeleteAsync(entity, cancellationToken);
        BeforeDelete(entity);
        return base.UpdateAsync(entity, cancellationToken);
    }

    public override int Delete(Expression<Func<TEntity, bool>> predicate)
    {
        if (!isDeleteAudit) return base.Delete(predicate);
        List<TEntity> items = base.Select.Where(predicate).ToList();
        if (items.Count == 0)
        {
            return 0;
        }

        foreach (var entity in items)
        {
            BeforeDelete(entity);
        }

        return base.Update(items);
    }

    public override async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        if (!isDeleteAudit) return await base.DeleteAsync(predicate, cancellationToken);

        List<TEntity> items = await base.Select.Where(predicate).ToListAsync(cancellationToken);
        if (items.Count == 0)
        {
            return 0;
        }

        foreach (var entity in items)
        {
            BeforeDelete(entity);
        }

        return await base.UpdateAsync(items, cancellationToken);
    }

    #endregion

    #region InsertOrUpdate
    public override TEntity InsertOrUpdate(TEntity entity)
    {
        BeforeInsert(entity);
        return base.InsertOrUpdate(entity);
    }

    public override async Task<TEntity> InsertOrUpdateAsync(TEntity entity,
        CancellationToken cancellationToken = default)
    {
        BeforeInsert(entity);
        return await base.InsertOrUpdateAsync(entity, cancellationToken);
    }

    #endregion
}