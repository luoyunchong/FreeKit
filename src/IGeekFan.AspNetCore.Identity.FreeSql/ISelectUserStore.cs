using FreeSql;
using Microsoft.AspNetCore.Identity;

namespace IGeekFan.AspNetCore.Identity.FreeSql;

public interface ISelectUserStore<TUser> : IUserStore<TUser>, IDisposable where TUser : class
{
    //
    // 摘要:
    //     Returns an FreeSql.ISelect`1 collection of users.
    //
    // 值:
    //     An FreeSql.ISelect`1 collection of users.
    ISelect<TUser> Users
    {
        get;
    }
}

//
// 摘要:
//     Provides an abstraction for querying roles in a Role store.
//
// 类型参数:
//   TRole:
//     The type encapsulating a role.
public interface ISelectRoleStore<TRole> : IRoleStore<TRole>, IDisposable where TRole : class
{
    //
    // 摘要:
    //     Returns an System.Linq.IQueryable`1 collection of roles.
    //
    // 值:
    //     An System.Linq.IQueryable`1 collection of roles.
    ISelect<TRole> Roles
    {
        get;
    }
}
