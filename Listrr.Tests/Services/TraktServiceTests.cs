using System;
using System.Collections.Generic;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using Xunit;

namespace Listrr.Tests.Services
{
    public class TraktServiceTests
    {
        [Theory]
        [MemberData(nameof(NullParameterData))]
        public void CreateTraktServiceWithNullParametersThowsNullReferenceException(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TraktAPIConfiguration traktApiConfiguration)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => { new TraktService(userManager, httpContextAccessor, traktApiConfiguration); });
        }



        public static IEnumerable<object[]> NullParameterData()
        {
            yield return new[] { null, null, Mock.Of<TraktAPIConfiguration>() };
            yield return new[] { null, Mock.Of<HttpContextAccessor>(), null };
            yield return new[] { Mock.Of<UserManager<User>>(), null, null };
        }
    }
}