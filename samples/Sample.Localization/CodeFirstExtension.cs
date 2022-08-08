using FreeSql;
using IGeekFan.Localization.FreeSql.Models;
using System.Collections.Generic;

namespace Sample.Localization
{
    public static class CodeFirstExtension
    {
        public static ICodeFirst SeedData(this ICodeFirst @this)
        {
            @this.Entity<LocalCulture>(e =>
            {
                e.HasData(new List<LocalCulture>()
                    {
                        new("en-US","en-US") ,
                        new("ja-JP","ja-JP")
                        {
                            Resources=new List<LocalResource>()
                            {
                                new("Hello","こんにちは"),
                                new("Request Localization Sample","リクエストローカライズ"),
                            }
                        },
                        new("fr-FR","fr-FR")
                        {
                            Resources=new List<LocalResource>()
                            {
                                new("Hello","Bonjour"),
                                new("Request Localization Sample","Demander un échantillon localisé"),
                            }
                        },
                        new("zh-CN","中文")
                        {
                            Resources=new List<LocalResource>()
                            {
                                new("Hello","你好"),
                                new("Request Localization Sample","这是一个请求资源示例"),
                            }
                        }
                    });
            });
            return @this;
        }
    }
}
