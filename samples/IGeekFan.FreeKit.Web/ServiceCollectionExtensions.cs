using System.Diagnostics;
using FreeSql;
using FreeSql.Internal;
using IGeekFan.FreeKit.Extras.CaseQuery;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Modularity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace IGeekFan.FreeKit.Web
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddModuleServices(this IServiceCollection services, IConfiguration c)
        {

            // Register a convention allowing to us to prefix routes to modules.
            services.AddTransient<IPostConfigureOptions<MvcOptions>, ModuleRoutingMvcOptionsPostConfigure>();

            // Adds module1 with the route prefix module-1
            services.AddModule<Module1.Module1Startup>("module-1");

            // Adds module2 with the route prefix module-2
            services.AddModule<Module2.Module2Startup>("module-2");

            return services;
        }

        public static IServiceCollection AddFreeSql(this IServiceCollection services, IConfiguration c)
        {
            services.Configure<UnitOfWorkDefualtOptions>(c =>
            {
                c.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
                c.Propagation = Propagation.Required;
            });

            Func<IServiceProvider, IFreeSql> fsql = r =>
            {
                IFreeSql fsql = new FreeSqlBuilder()
                      .UseConnectionString(DataType.Sqlite, c["ConnectionStrings:DefaultConnection"])
                      .UseAutoSyncStructure(true)
                      .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                      .UseMonitorCommand(
                          cmd => Trace.WriteLine("\r\n线程" + Thread.CurrentThread.ManagedThreadId + ": " + cmd.CommandText)
                    ).Build();
                return fsql;
            };
            services.AddSingleton(fsql);
            services.AddFreeRepository();
            services.AddScoped<UnitOfWorkManager>();

            return services;
        }


        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration c)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, CamelCaseApiDescriptionProvider>());


            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseQueryStrings = true;
                options.LowercaseUrls = true;
            });

            services.AddControllers(options =>
            {
                options.ValueProviderFactories.Add(new CamelCaseValueProviderFactory());
                //options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration c)
        {
            //Swagger重写PascalCase，改成小写开头模式
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, CamelCaseApiDescriptionProvider>());
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "IGeekFan.FreeKit.Web - HTTP API",
                    Version = "v1",
                    Description = "The Todo Microservice HTTP API. This is a Data-Driven/CRUD microservice sample"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id =  "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey

                });

            });

            return services;
        }
    }
}
