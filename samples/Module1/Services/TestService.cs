namespace Module1.Services
{
    public class TestService : ITestService
    {
        private readonly IFreeSql _fsql;
        public TestService(IFreeSql fsql)
        {
            _fsql = fsql;
        }
        public bool ExecuteConnectTest()
        {
            return _fsql.Ado.ExecuteConnectTest();
        }
    }
}
