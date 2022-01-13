using IGeekFan.FreeKit.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using Xunit;

namespace IGeekFan.FreeKit.xUnit.Extras
{
    public class FullEntity : FullAduitEntity
    {
    }
    public class UseFullEntity : FullAduitEntity<Guid, Guid?>
    {
    }

    public class FullAduitEntityTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        public FullAduitEntityTest(ITestOutputHelper testOut)
        {
            testOutputHelper = testOut;
        }

        [Fact]
        public async Task OutputTestFull()
        {
            var full = new UseFullEntity();
            full.CreateUserId = null;
            full.UpdateUserId = null;
            full.DeleteUserId = null;

            full.CreateTime = DateTime.Now;
            full.UpdateTime = DateTime.Now;
            full.DeleteTime = DateTime.Now;

            full.IsDeleted = false;
            await Task.CompletedTask;
        }

        [Fact]
        public async Task OutputTest()
        {
            var full = new FullEntity();
            full.CreateUserId = null;
            full.UpdateUserId = null;
            full.DeleteUserId = null;

            full.CreateTime = DateTime.Now;
            full.UpdateTime = DateTime.Now;
            full.DeleteTime = DateTime.Now;

            full.IsDeleted = false;
            await Task.CompletedTask;
        }
    }
}
