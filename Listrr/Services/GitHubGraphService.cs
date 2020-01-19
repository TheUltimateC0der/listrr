using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;
using Listrr.Configuration;

namespace Listrr.Services
{
    public class GitHubGraphService : IGitHubGraphService
    {
        private readonly GithubAPIConfiguration _githubApiConfiguration;

        public GitHubGraphService(GithubAPIConfiguration githubApiConfiguration)
        {
            _githubApiConfiguration = githubApiConfiguration;
        }


        public async Task<List<string>> GetDonor()
        {
            var result = new List<string>();

            var donorRequest = new GraphQLRequest(){Query = @"
                query { 
                  viewer { 
                    sponsorshipsAsMaintainer (includePrivate: true, first: 100) {
                      nodes {
                        sponsor {
                          login,
                          databaseId
                        }
                      }
                    }
                  }
                }"
            };

            
            var grapqlClient = new GraphQLClient("https://api.github.com/graphql");
            grapqlClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_githubApiConfiguration.Token}");
            grapqlClient.DefaultRequestHeaders.Add("User-Agent", "listrr.pro graphql client");

            var graphqlResponse = await grapqlClient.PostAsync(donorRequest);

            foreach (var node in graphqlResponse.Data.viewer.sponsorshipsAsMaintainer.nodes)
            {
                result.Add(node.sponsor.databaseId.ToString());
            }

            return result;
        }
    }
}