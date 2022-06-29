# IGeekFan.AspNetCore.Identity.FreeSql

`asp.net core 6` 的`identity`的`freesql`实现
- 安装包
```bash
dotnet add package IGeekFan.AspNetCore.Identity.FreeSql
```
- 新增FreeSql相关包
```bash
dotnet add package FreeSql.Provider.MySqlConnector
```
### 扩展用户、角色
```csharp
public class AppUser : IdentityUser<Guid>
{
}
public class AppRole : IdentityRole<Guid>
{

}
public class IdentityContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public IdentityContext(IOptions<IdentityOptions> identityOptions, IFreeSql fsql, DbContextOptions options)
    : base(identityOptions.Value, fsql, options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        //这里直接指定一个静态的 IFreeSql 对象即可，切勿重新 Build()
    }

    protected override void OnModelCreating(ICodeFirst codefirst)
    {
        base.OnModelCreating(codefirst);
        codefirst.ApplyConfiguration(new AppUserConfiguration());
        codefirst.ApplyConfiguration(new AppRoleConfiguration());
        codefirst.SyncStructure(typeof(AppUser),typeof(AppRole))
    }
}

```
### 配置用户、角色的配置
```csharp
public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EfCoreTableFluent<AppUser> model)
    {
        model.ToTable("app_user");
    }
}
public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EfCoreTableFluent<AppRole> model)
    {
        model.ToTable("app_role");
    }
}
```


- appsettings.json
该配置通过扩展方法`UseConnectionString`读取如下配置，DefaultDB配置0代表使用配置串MySql。需要安装`FreeSql.Provider.MySqlConnector`
```json
"ConnectionStrings": {
    "DefaultDB": "0",
    "DataType": {
        "MySql": 0,
        "SqlServer": 1,
        "PostgreSQL": 2,
        "Oracle": 3,
        "Sqlite": 4
    },
    "MySql": "Data Source=localhost;Port=3306;User ID=root;Password=root;Initial Catalog=file;Charset=utf8mb4;SslMode=none;Max pool size=1;Connection LifeTime=20",
    "SqlServer": "Data Source=.;User ID=sa;Password=123456;Integrated Security=True;Initial Catalog=LinCMS;Pooling=true;Min Pool Size=1",
    "PostgreSQL": "Host=localhost;Port=5432;Username=postgres;Password=123456; Database=lincms;Pooling=true;Minimum Pool Size=1",
    "Oracle": "user id=root;password=root; data source=//127.0.0.1:1521/ORCL;Pooling=true;Min Pool Size=1",
    "Sqlite": "Data Source=|DataDirectory|\\lincms.db; Attachs=lincms.db; Pooling=true;Min Pool Size=1"
},
```

### 配置Identity+FreeSql



- 新增一个扩展方法，引用aspnetcore identity 相关服务
```csharp
public static IServiceCollection AddFreeSql(this IServiceCollection services, IConfiguration configuration)
{
    IFreeSql fsql = new FreeSqlBuilder()
            .UseConnectionString(configuration)
            .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
            .UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
            .UseMonitorCommand(cmd =>
            {
                Trace.WriteLine(cmd.CommandText + ";");
            })
            .Build();

    fsql.GlobalFilter.Apply<ISoftDelete>("IsDeleted", a => a.IsDeleted == false);

    services.AddSingleton<IFreeSql>(fsql);
    services.AddFreeRepository();
    services.AddScoped<UnitOfWorkManager>();

    //只有实例化了ToDoContext，才能正常调用OnModelCreating，不然直接使用仓储，无法调用DbContext中的OnModelCreating方法，，配置的TodoConfiguration 就会没有生效
    services.AddFreeDbContext<IdentityContext>(options => options
                .UseFreeSql(fsql)
                .UseOptions(new DbContextOptions()
                {
                    EnableAddOrUpdateNavigateList = true
                })
    );

    services.AddIdentityCore<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddFreeSqlStores<IdentityContext>();

    //fsql.CodeFirst.ApplyConfiguration(new TodoConfiguration());

    return services;
}
```