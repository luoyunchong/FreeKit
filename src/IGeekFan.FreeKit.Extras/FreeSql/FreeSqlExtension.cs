using System.Diagnostics;
using FreeSql;
using Microsoft.Extensions.Configuration;

namespace IGeekFan.FreeKit.Extras.FreeSql;

public static class FreeSqlExtension
{
    public static FreeSqlBuilder UseConnectionString(this FreeSqlBuilder @this, IConfiguration configuration)
    {
        IConfigurationSection dbTypeCode = configuration.GetSection("ConnectionStrings:DefaultDB");
        if (Enum.TryParse(dbTypeCode.Value, out DataType dataType))
        {
            if (!Enum.IsDefined(typeof(DataType), dataType))
            {
                Trace.WriteLine($"数据库配置ConnectionStrings:DefaultDB:{dataType}无效");
            }

            IConfigurationSection configurationSection = configuration.GetSection($"ConnectionStrings:{dataType}");
            @this.UseConnectionString(dataType, configurationSection.Value);
        }
        else
        {
            Trace.WriteLine($"数据库配置ConnectionStrings:DefaultDB:{dbTypeCode.Value}无效");
        }

        return @this;
    }

}
