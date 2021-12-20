
using IGeekFan.FreeKit.Extras.Dependency;

namespace Module1
{
    public interface ITestService : ITransientDependency
    {
        bool ExecuteConnectTest();
    }
}