using GraphQL;
using GraphQL.Client.Http;

using Listrr.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Services
{
    public class GitHubGraphService : IGitHubGraphService
    {
        private readonly GithubAPIConfiguration _githubApiConfiguration;
        private readonly LimitConfigurationList _limitConfigurationList;

        public GitHubGraphService(GithubAPIConfiguration githubApiConfiguration, LimitConfigurationList limitConfigurationList)
        {
            _githubApiConfiguration = githubApiConfiguration;
            _limitConfigurationList = limitConfigurationList;
        }


        public async Task<IDictionary<string, LimitConfiguration>> GetDonor()
        {
            var result = new Dictionary<string, LimitConfiguration>();

            var donorRequest = new GraphQLRequest {Query = @"
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

            var grapqlClient = new GraphQLHttpClient("https://api.github.com/graphql");
            grapqlClient.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_githubApiConfiguration.Token}");
            grapqlClient.HttpClient.DefaultRequestHeaders.Add("User-Agent", "listrr.pro graphql client");

            var graphqlResponse = await grapqlClient.SendQueryAsync<dynamic>(donorRequest);

            foreach (var node in graphqlResponse.Data.viewer.sponsorshipsAsMaintainer.nodes)
            {
                var limitConfig = _limitConfigurationList.LimitConfigurations.FirstOrDefault(x => x.Amount == Convert.ToInt32(node.tier.monthlyPriceInDollars));

                if(limitConfig != null)
                    result.Add(node.sponsor.databaseId.ToString(), limitConfig);
            }

            return result;
        }
    }
}