using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FreeSql;
using IGeekFan.FreeKit.Extras.AuditEntity;
using IGeekFan.FreeKit.Extras.FreeSql;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Extras.FreeSql
{
    public class Todo : FullAuditEntity
    {
        public string Message { get; set; }
        public DateTime? NotifictionTime { get; set; }

        public bool IsDone { get; set; }

    }
    public class AuditRepositoryTest
    {
        private readonly IAuditBaseRepository<Todo> _repository;
        private readonly ITestOutputHelper _testOutputHelper;
        public AuditRepositoryTest(IAuditBaseRepository<Todo> repository, ITestOutputHelper testOutputHelper)
        {
           
            _repository = repository;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetTest()
        {
            _repository.Insert(new Todo { Message = "这是一个要完成和TODO", NotifictionTime = null, IsDone = false });

            Todo todo = _repository.Select.First();

            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));

        }
    }
}
