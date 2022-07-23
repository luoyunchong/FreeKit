
using IGeekFan.FreeKit.Extras.Dependency;

namespace Module1.Services
{
    public interface ITestService : ITransientDependency
    {
        bool ExecuteConnectTest();
    }
}