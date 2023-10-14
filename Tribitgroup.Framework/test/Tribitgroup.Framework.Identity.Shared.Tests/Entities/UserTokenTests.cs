using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Identity.Shared.DomainServices;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.Identity.Shared.Tests.Entities
{
    public class UserTokenTests
    {
        [Fact]
        public async Task CreateUserTokenAsync_With_Correct_Data_Should_Return_UserToken()
        {
            var validUntil = DateTime.UtcNow.AddMinutes(10);
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var userToken = await new UserTokenDomainService().CreateUserTokenAsync(
                new Guid(),
                new UserDevice(),
                token,
                validUntil,
                true
                );

            Assert.NotNull(userToken);
            userToken.Id.ShouldNotBe(Guid.Empty);
            userToken.ValidUntil.ShouldBe(validUntil);
            userToken.TokenHash.ShouldBe(token.ToSHA512());
            userToken.IsActive.ShouldBeTrue();
        }
    }
}
