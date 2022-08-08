using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using FreeSql;
using Microsoft.Extensions.Hosting;
using Sample.Localization;
using IGeekFan.Localization.FreeSql;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

//FreeSql+数据库本地化
var fsql = new FreeSqlBuilder()
    .UseConnectionString(DataType.Sqlite, builder.Configuration.GetSection("ConnectionStrings:DB").Value)
    .UseAutoSyncStructure(true)
    .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
    .Build();

fsql.CodeFirst.SeedData();
builder.Services.AddSingleton(fsql);

builder.Services.TryAddSingleton<IStringLocalizerFactory, FreeSqlStringLocalizerFactory>();
builder.Services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

var supportedCultures = new List<CultureInfo>
{
    new("en-US"),
    new("ja-JP"),
    new("fr-FR"),
    new("zh-CN")
};
var options = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("zh-CN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(options);

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();




