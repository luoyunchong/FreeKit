using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace IGeekFan.AspNetCore.Identity.FreeSql.Test;

public class ApiConsistencyTest : ApiConsistencyTestBase
{
    protected override Assembly TargetAssembly => typeof(IdentityUser).GetTypeInfo().Assembly;
}
