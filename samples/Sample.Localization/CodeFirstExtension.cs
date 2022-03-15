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
                        new LocalCulture("en-US","en-US") ,
                        new LocalCulture("ja-JP","ja-JP")
                        {
                            Resources=new List<LocalResource>()
                            {
                                new LocalResource("Hello","こんにちは"),
                                new LocalResource("Request Localization Sample","リクエストローカライズ"),
                            }
                        },
                        new LocalCulture("fr-FR","fr-FR")
                        {
                            Resources=new List<LocalResource>()
                            {
                                new LocalResource("Hello","Bonjour"),
                                new LocalResource("Request Localization Sample","Demander un échantillon localisé"),
                            }
                        },
                        new LocalCulture("zh-CN","中文")
                        {
                            Resources=new List<LocalResource>()
                            {
                                new LocalResource("Hello","你好"),
                                new LocalResource("Request Localization Sample","请求资源示例"),
                            }
                        }
                    });
            });
            return @this;
        }
    }
}
