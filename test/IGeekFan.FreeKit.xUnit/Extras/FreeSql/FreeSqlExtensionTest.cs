using FreeSql;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IGeekFan.FreeKit.xUnit.Extras.FreeSql
{
    public class FreeSqlExtensionTest
    {
        private readonly IConfiguration configuration;

        public FreeSqlExtensionTest(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Fact]
        public void UseConnectionStringTest()
        {

            var fsql1 = new FreeSqlBuilder().UseConnectionString(configuration);
            var fsql2 = new FreeSqlBuilder().UseConnectionString(configuration, "CsLog");
        }
    }
}
