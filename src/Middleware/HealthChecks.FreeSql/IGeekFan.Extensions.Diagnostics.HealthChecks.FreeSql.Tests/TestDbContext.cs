// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using FreeSql;

namespace IGeekFan.Extensions.Diagnostics.HealthChecks;
public class TestDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
}

public class Blog
{
    public int Id { get; set; }

    public int Name { get; set; }
}
