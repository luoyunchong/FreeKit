using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Security;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace IGeekFan.FreeKit.xUnit.Extras.Security
{
    public class CurrentUserTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ICurrentUser _currentUser;
        private readonly ICurrentUser<Guid> _currentUserGuid;

        public CurrentUserTest(ITestOutputHelper testOut, ICurrentUser currentUser, ICurrentUser<Guid> currentUserGuid)
        {
            _testOutputHelper = testOut;
            _currentUser = currentUser;
            _currentUserGuid = currentUserGuid;
        }

        [Fact]
        public void GetUserId()
        {
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(_currentUser));
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(_currentUserGuid));
            string? nameIdentifier = _currentUser.FindClaim(ClaimTypes.NameIdentifier)?.Value;
            Assert.Equal(nameIdentifier,_currentUser.Id);
        }
    }
}