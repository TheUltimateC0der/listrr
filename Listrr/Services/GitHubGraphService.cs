using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

using Listrr.API.GitHub;
using Listrr.Configuration;

using GraphQLRequest = GraphQL.GraphQLRequest;

namespace Listrr.Services
{
    public class GitHubGraphService : IGitHubGraphService
    {
        private readonly GithubAPIConfiguration _githubApiConfiguration;
        private readonly LimitConfigurationList _limitConfigurationList;

        public GitHubGraphService(GithubAPIConfiguration githubApiConfiguration, LimitConfigurationList limitConfigurationList)
        {
            _githubApiConfiguration = githubApiConfiguration ?? throw new ArgumentNullException(nameof(githubApiConfiguration));
            _limitConfigurationList = limitConfigurationList ?? throw new ArgumentNullException(nameof(limitConfigurationList));
        }


        public async Task<IDictionary<string, LimitConfiguration>> GetDonor()
        {
            var result = new Dictionary<string, LimitConfiguration>();

            var donorRequest = new GraphQLRequest
            {
                Query = @"
                query {
                  viewer {
                    sponsorshipsAsMaintainer (includePrivate: true, first: 100) {
                      nodes {
                        sponsor {
                          login,
                          databaseId
                        },
                        tier {
                          monthlyPriceInDollars
                        }
                      }
                    }
                  }
                }"
            };

            var grapqlClient = new GraphQLHttpClient("https://api.github.com/graphql", new NewtonsoftJsonSerializer());
            grapqlClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_githubApiConfiguration.Token}");
            grapqlClient.HttpClient.DefaultRequestHeaders.Add("User-Agent", "listrr.pro graphql client");

            var graphqlResponse = await grapqlClient.SendQueryAsync<GitHubDonorResponse>(donorRequest);

            foreach (var node in graphqlResponse.Data.Viewer.SponsorshipsAsMaintainer.Nodes)
            {
                var limitConfig = _limitConfigurationList.LimitConfigurations.FirstOrDefault(x => x.Amount == Convert.ToInt32(node.Tier.MonthlyPriceInDollars));

                if (limitConfig != null)
                    result.Add(node.Sponsor.DatabaseId.ToString(), limitConfig);
            }

            return result;
        }
    }
}