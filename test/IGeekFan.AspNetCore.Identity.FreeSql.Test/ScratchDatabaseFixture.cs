using FreeSql.Internal;
using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test
{
    public class ScratchDatabaseFixture
    {
        public string CreateConnection => $"DataSource=D{Guid.NewGuid()}.db";
        public string Connection { get; }
        public ScratchDatabaseFixture()
        {
            Connection = $"DataSource=D{Guid.NewGuid()}.db";
        }
    }
}
