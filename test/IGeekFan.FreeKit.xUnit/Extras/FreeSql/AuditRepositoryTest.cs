﻿using IGeekFan.FreeKit.Extras.AuditEntity;
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
        public void InsertEntityListTest()
        {
            var list = new List<Todo>()
            {
                new Todo {Message = "这是一个要完成的TODO______1", NotifictionTime = null, IsDone = false},
                new Todo {Message = "这是一个要完成的TODO______2", NotifictionTime = null, IsDone = false}
            };
            _repository.Insert(list);
            _repository.Delete(list);
        }

        [Fact]
        public async Task InsertEntityListAsyncTest()
        {
            var list = new List<Todo>()
            {
                new Todo {Message = "这是一个要完成的TODO______1", NotifictionTime = null, IsDone = false},
                new Todo {Message = "这是一个要完成的TODO______2", NotifictionTime = null, IsDone = false}
            };
            await _repository.InsertAsync(list);
            await _repository.DeleteAsync(list);
        }

        [Fact]
        public void InsertDeleteEntityExpressionTest()
        {
            _repository.Insert(new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false});
            Todo todo = _repository.Select.OrderByDescending(r => r.CreateTime).First();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));
            _repository.Delete(r => r.Id == todo.Id);

            _repository.Insert(new Todo {Message = "这是一个要完成的TODO2", NotifictionTime = null, IsDone = false});
            Todo todo2 = _repository.Select.OrderByDescending(r => r.CreateTime).First();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo2));
            _repository.Delete(r => r.IsDone == false);
        }

        [Fact]
        public async Task InsertDeleteEntityExpressionAsyncTest()
        {
            await _repository.InsertAsync(new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false});
            Todo todo = await _repository.Select.OrderByDescending(r => r.CreateTime).FirstAsync();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));
            await _repository.DeleteAsync(r => r.Id == todo.Id);


            await _repository.InsertAsync(new Todo {Message = "这是一个要完成的TODO2", NotifictionTime = null, IsDone = false});
            Todo todo2 = await _repository.Select.OrderByDescending(r => r.CreateTime).FirstAsync();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo2));
            await _repository.DeleteAsync(r => r.IsDone == false);
        }

        [Fact]
        public async Task InsertDeleteEntityAsyncTest()
        {
            await _repository.InsertAsync(new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false});
            Todo todo = await _repository.Select.OrderByDescending(r => r.CreateTime).FirstAsync();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));
            await _repository.DeleteAsync(todo);
        }

        [Fact]
        public void InsertDeleteEntityTest()
        {
            _repository.Insert(new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false});
            Todo todo = _repository.Select.OrderByDescending(r => r.CreateTime).First();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));
            _repository.Delete(todo);
        }

        [Fact]
        public async Task InsertDeleteEntityPrimaryAsyncTest()
        {
            await _repository.InsertAsync(new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false});
            Todo todo = await _repository.Select.OrderByDescending(r => r.CreateTime).FirstAsync();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));
            await _repository.DeleteAsync(todo.Id);
        }

        [Fact]
        public void InsertDeleteEntityPrimaryTest()
        {
            _repository.Insert(new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false});
            Todo todo = _repository.Select.OrderByDescending(r => r.CreateTime).First();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(todo));
            _repository.Delete(todo.Id);
        }

        [Fact]
        public void InsertOrUpdateTest()
        {
            Todo todo = new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false};
            _repository.InsertOrUpdate(todo);
        }

        [Fact]
        public async Task InsertOrUpdateAsyncTest()
        {
            Todo todo = new Todo {Message = "这是一个要完成的TODO", NotifictionTime = null, IsDone = false};
            await _repository.InsertOrUpdateAsync(todo);
        }
    }
}