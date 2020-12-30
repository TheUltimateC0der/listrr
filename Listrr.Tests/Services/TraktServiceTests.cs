using Listrr.Configuration;
using Listrr.Data;
using Listrr.Repositories;
using Listrr.Services;
using Listrr.Tests.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using System;
using System.Collections.Generic;

using Xunit;

namespace Listrr.Tests.Services
{
    public class TraktServiceTests
    {
        [Theory]
        [MemberData(nameof(NullParameterData))]
        public void CreateTraktServiceWithNullParametersThowsNullReferenceException(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TraktAPIConfiguration traktApiConfiguration, IUserLimitService userLimitService, IIMDbRepository imDbRepository)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => { new TraktService(userManager, httpContextAccessor, traktApiConfiguration, userLimitService, imDbRepository); });
        }

        [Fact]
        public void CreateTraktServiceIsNotNull()
        {
            // Arrange
            var userManagerMock = UserManagerHelpers.MockUserManager<User>();
            var httpContextAccessorMock = Mock.Of<IHttpContextAccessor>();
            var traktApiConfigurationMock = Mock.Of<TraktAPIConfiguration>();
            var userLimitServiceMock = Mock.Of<IUserLimitService>();
            var imdbRespositoryeMock = Mock.Of<IIMDbRepository>();

            // Act
            var traktService = new TraktService(userManagerMock.Object, httpContextAccessorMock, traktApiConfigurationMock, userLimitServiceMock, imdbRespositoryeMock);

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
            var userLimitServiceMock = Mock.Of<IUserLimitService>();
            var imdbRespositoryeMock = Mock.Of<IIMDbRepository>();

            // Act
            var traktService = new TraktService(userManagerMock.Object, httpContextAccessorMock, traktApiConfigurationMock, userLimitServiceMock, imdbRespositoryeMock);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => { await traktService.Create(null); });
        }


        public static IEnumerable<object[]> NullParameterData()
        {
            yield return new object[] { null, Mock.Of<HttpContextAccessor>(), Mock.Of<TraktAPIConfiguration>(), Mock.Of<IUserLimitService>(), Mock.Of<IIMDbRepository>() };
            yield return new object[] { UserManagerHelpers.TestUserManager<User>(), null, Mock.Of<TraktAPIConfiguration>(), Mock.Of<IUserLimitService>(), Mock.Of<IIMDbRepository>() };
            yield return new object[] { UserManagerHelpers.TestUserManager<User>(), Mock.Of<HttpContextAccessor>(), null, Mock.Of<IUserLimitService>(), Mock.Of<IIMDbRepository>() };
            yield return new object[] { UserManagerHelpers.TestUserManager<User>(), Mock.Of<HttpContextAccessor>(), Mock.Of<TraktAPIConfiguration>(), null, Mock.Of<IIMDbRepository>() };
            yield return new object[] { UserManagerHelpers.TestUserManager<User>(), Mock.Of<HttpContextAccessor>(), Mock.Of<TraktAPIConfiguration>(), Mock.Of<IUserLimitService>(), null };
        }
    }
}