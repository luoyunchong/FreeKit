using IGeekFan.FreeKit.Extras.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Extras.Extensions;

public class EnumerableExtensionsTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EnumerableExtensionsTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void LoopIndexTest()
    {
        // Create a list with integer values
        List<int> myValues = new List<int>()
        {
            23, 34, 45, 56, 67
        };

        // Loop through the list and its index using an item/index tuple
        foreach (var (item, index) in myValues.LoopIndex())
        {
            _testOutputHelper.WriteLine($"{index}: {item}");
        }
    }
}