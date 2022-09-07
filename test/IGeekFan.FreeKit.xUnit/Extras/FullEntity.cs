using IGeekFan.FreeKit.Extras.AuditEntity;
using Xunit.Abstractions;
using Xunit;
using IGeekFan.FreeKit.Extras.Extensions;

namespace IGeekFan.FreeKit.xUnit.Extras
{
    public class FullEntity : FullAuditEntity,ITenant
    {
        public Guid? TenantId { get; set; }
    }

    public class UseFullEntity : FullAuditEntity<Guid, Guid>
    {
    }

    public class FullAuditEntityTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FullAuditEntityTest(ITestOutputHelper testOut)
        {
            _testOutputHelper = testOut;
        }

        [Fact]
        public async Task OutputTestFull()
        {
            var full = new UseFullEntity();
            full.CreateUserId = Guid.NewGuid();
            full.UpdateUserId = Guid.NewGuid();
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
            full.CreateUserId = Guid.Empty;
            full.UpdateUserId = Guid.Empty;
            full.DeleteUserId = null;

            full.CreateTime = DateTime.Now;
            full.UpdateTime = DateTime.Now;
            full.DeleteTime = DateTime.Now;

            full.IsDeleted = false;
            var x = typeof(FullAuditEntity).IsAssignableFrom(typeof(FullEntity));
            _testOutputHelper.WriteLine(x + "");
            x = typeof(FullEntity).HasImplementedRawGeneric(typeof(IDeleteAuditEntity<>));
            _testOutputHelper.WriteLine(x + "");

             x = full is ICreateAuditEntity<Guid> createAuditEntity;
             _testOutputHelper.WriteLine(x + "");
            await Task.CompletedTask;
        }
    }

}