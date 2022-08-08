using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.FreeSql;
using Xunit;

namespace IGeekFan.FreeKit.xUnit.Extras.FreeSql
{
    public class UserRole
    {
        [Column(IsPrimary = true)]
        public int UserId { get; set; }
        [Column(IsPrimary = true)]
        public int RoleId { get; set; }
        public DateTime CreateTime { get; set; }
    }

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
