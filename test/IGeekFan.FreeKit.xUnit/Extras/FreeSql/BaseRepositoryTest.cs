using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.xUnit.Models;
using Xunit;

namespace IGeekFan.FreeKit.xUnit.Extras.FreeSql
{
    public class BaseRepositoryTest
    {
        private readonly IBaseRepository<UserRole, int, int> _repository;
        public BaseRepositoryTest(IBaseRepository<UserRole, int, int> repository)
        {
            _repository = repository;
        }

        [Fact]
        public void GetTest()
        {
            _repository.Insert(new UserRole() { UserId = 1, RoleId = 1, CreateTime = DateTime.Now });
            UserRole userRole = _repository.Get(1, 1);
            _repository.Delete(1, 1);
        }
    }
}
