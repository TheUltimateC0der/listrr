using System;
using System.Collections.Generic;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Services;
using Listrr.Tests.Helpers;

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

        [Fact]
        public void CreateTraktServiceIsNotNull()
        {
            // Arrange
            var userManagerMock = UserManagerHelpers.MockUserManager<User>();
            var httpContextAccessorMock = Mock.Of<IHttpContextAccessor>();
            var traktApiConfigurationMock = Mock.Of<TraktAPIConfiguration>();

            // Act
            var traktService = new TraktService(userManagerMock.Object, httpContextAccessorMock, traktApiConfigurationMock);

            // Assert
            Assert.NotNull(traktService);
        }

        [Fact]
        public async void Create_WithNullModelThrowsException()
        {
            // Arrange
            var userManagerMock = UserManagerHelpers.MockUserManager<User>();
            var httpContextAccessorMock = Mock.Of<IHttpContextAccessor>();
            var traktApiConfigurationMock = Mock.Of<TraktAPIConfiguration>();

            // Act
            var traktService = new TraktService(userManagerMock.Object, httpContextAccessorMock, traktApiConfigurationMock);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => { await traktService.Create(null); });
        }


        public static IEnumerable<object[]> NullParameterData()
        {
            yield return new object[] { null, Mock.Of<HttpContextAccessor>(), Mock.Of<TraktAPIConfiguration>() };
            yield return new object[] { UserManagerHelpers.TestUserManager<User>(), null, Mock.Of<TraktAPIConfiguration>() };
            yield return new object[] { UserManagerHelpers.TestUserManager<User>(), Mock.Of<HttpContextAccessor>(), null };
        }
    }
}