﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FreeSql;
using Microsoft.AspNetCore.Identity;

namespace IGeekFan.AspNetCore.Identity.FreeSql;
/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
public class IdentityDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityDbContext"/>.
    /// </summary>
    /// <param name="identityOptions"><see cref="IdentityOptions"/></param>
    /// <param name="fsql">fsql instance</param>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(IdentityOptions identityOptions, IFreeSql fsql, DbContextOptions options) : base(identityOptions, fsql, options) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContext" /> class.
    /// </summary>
    protected IdentityDbContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of the user objects.</typeparam>
public class IdentityDbContext<TUser> : IdentityDbContext<TUser, IdentityRole, string> where TUser : IdentityUser
{
    /// <summary>
    /// Initializes a new instance of <see cref="IdentityDbContext"/>.
    /// </summary>
    /// <param name="identityOptions"></param>
    /// <param name="fsql"></param>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(IdentityOptions identityOptions, IFreeSql fsql, DbContextOptions options) : base(
        identityOptions, fsql, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityDbContext" /> class.
    /// </summary>
    protected IdentityDbContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TRole">The type of role objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
public class IdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>, IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of the db context.
    /// </summary>
    /// <param name="identityOptions">The options to be used by a <see cref="IdentityOptions"/>.</param>
    /// <param name="fsql">The options to be used by a <see cref="IFreeSql"/>.</param>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(IdentityOptions identityOptions, IFreeSql fsql, DbContextOptions options) : base(identityOptions, fsql, options) { }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    protected IdentityDbContext() { }
}

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TRole">The type of role objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
/// <typeparam name="TUserClaim">The type of the user claim object.</typeparam>
/// <typeparam name="TUserRole">The type of the user role object.</typeparam>
/// <typeparam name="TUserLogin">The type of the user login object.</typeparam>
/// <typeparam name="TRoleClaim">The type of the role claim object.</typeparam>
/// <typeparam name="TUserToken">The type of the user token object.</typeparam>
public abstract class IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityUserContext<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="identityOptions"></param>
    /// <param name="fsql"></param>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public IdentityDbContext(IdentityOptions identityOptions, IFreeSql fsql, DbContextOptions options) : base(identityOptions, fsql, options) { }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    protected IdentityDbContext() { }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of User roles.
    /// </summary>
    public virtual DbSet<TUserRole> UserRoles { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of roles.
    /// </summary>
    public virtual DbSet<TRole> Roles { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of role claims.
    /// </summary>
    public virtual DbSet<TRoleClaim> RoleClaims { get; set; }

    /// <summary>
    /// Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="builder">
    /// The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ICodeFirst builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<TUser>(b =>
        {
            //b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        });

        builder.Entity<TRole>(b =>
        {
            b.HasKey(r => r.Id);
            b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
            b.ToTable("AspNetRoles");
            b.Property(r => r.ConcurrencyStamp).IsRowVersion();
            //b.Property(u => u.ConcurrencyStamp).IsRowVersion().Help().MapType(typeof(byte[]));
            b.Property(u => u.Name).HasMaxLength(256);
            b.Property(u => u.NormalizedName).HasMaxLength(256);

            //b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
            //b.HasMany<TRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
        });

        builder.Entity<TRoleClaim>(b =>
        {
            b.HasKey(rc => rc.Id);
            b.Property(r => r.Id).Help().IsIdentity(true);
            b.ToTable("AspNetRoleClaims");
        });

        builder.Entity<TUserRole>(b =>
        {
            b.HasKey(r => new { r.UserId, r.RoleId });
            b.ToTable("AspNetUserRoles");
        });
    }
}
