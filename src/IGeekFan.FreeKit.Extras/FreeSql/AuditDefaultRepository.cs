// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql;
using IGeekFan.FreeKit.Extras.AuditEntity;
using IGeekFan.FreeKit.Extras.Security;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public class AuditDefaultRepository<TEntity, TKey, TUkey> : AuditBaseRepository<TEntity, TKey>
    where TEntity : class, new()
    where TUkey : struct, IEquatable<TUkey>
{
    public AuditDefaultRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(
        unitOfWorkManager, currentUser)
    {
    }

    protected override void BeforeInsert(TEntity entity)
    {
        if (entity is ITenant tenant)
        {
            tenant.TenantId = CurrentUser.TenantId;
        }
        if (entity is ICreateAuditEntity<TUkey> createAduit)
        {
            createAduit.CreateTime = DateTime.Now;
            createAduit.CreateUserId = CurrentUser.FindUserId<TUkey>();
            createAduit.CreateUserName = CurrentUser.UserName;
        }

        BeforeUpdate(entity);
    }

    protected override void BeforeUpdate(TEntity entity)
    {
        if (!IsUpdateAudit) return;

        if (entity is IUpdateAuditEntity<TUkey> updateAudit)
        {
            updateAudit.UpdateTime = DateTime.Now;
            updateAudit.UpdateUserName = CurrentUser.UserName;
            updateAudit.UpdateUserId = CurrentUser.FindUserId<TUkey>();
        }
    }

    protected override void BeforeDelete(TEntity entity)
    {
        if (!IsDeleteAudit) return;
        if (entity is ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
        }

        if (entity is IDeleteAuditEntity<TUkey> deleteAudit)
        {
            deleteAudit.DeleteUserId = CurrentUser.FindUserId<TUkey>();
            deleteAudit.DeleteUserName = CurrentUser.UserName;
            deleteAudit.DeleteTime = DateTime.Now;
        }
    }
}