using System;
using System.Collections.Generic;

using Listrr.Configuration;
using Listrr.Services;

using Moq;

using Xunit;

namespace Listrr.Tests.Services
{
    public class GitHubGraphServiceTests
    {
        [Theory]
        [MemberData(nameof(NullParameterData))]
        public void CreateGitHubGraphServiceWithNullParametersThowsNullReferenceException(GithubAPIConfiguration githubAPIConfiguration, LimitConfigurationList limitConfigurationList)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => { new GitHubGraphService(githubAPIConfiguration, limitConfigurationList); });
        }



        public static IEnumerable<object[]> NullParameterData()
        {
            yield return new[] { null, Mock.Of<LimitConfigurationList>() };
            yield return new[] { Mock.Of<GithubAPIConfiguration>(), null };
        }
    }
}