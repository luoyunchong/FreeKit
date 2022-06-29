using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Extras.Security
{
    public class CurrentUserTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public CurrentUserTest(ITestOutputHelper testOut)
        {
            testOutputHelper = testOut;
        }

        [Fact]
        public void GetUserId()
        {

        }
    }
}
