using IGeekFan.FreeKit.Extras.FreeSql;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Extras.FreeSql
{
    public class ReflexHelperTest
    {
        private readonly ITestOutputHelper _helper;

        public ReflexHelperTest(ITestOutputHelper helper)
        {
            _helper = helper;
        }


        [Fact]
        public void GetTypesByTableAttributeTest()
        {
            Type[] types = ReflexHelper.GetTypesByTableAttribute(typeof(ReflexHelperTest));
            _helper.WriteLine(types.Length.ToString());
        }

        [Fact]
        public void GetTypesByNameSpaceTest()
        {
            Type[] types = ReflexHelper.GetTypesByNameSpace(typeof(ReflexHelperTest), new List<string>
            {
                "IGeekFan.FreeKit.xUnit.Models"
            });
            _helper.WriteLine(types.Length.ToString());
        }
    }
}
