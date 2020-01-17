using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client;
using GraphQL.Common.Request;

namespace Listrr.Services
{
    public class GitHubGraphService : IGitHubGraphService
    {
        public async Task<List<string>> GetDonor()
        {
            var result = new List<string>();

            var donorRequest = new GraphQLRequest(){Query = @"
                query { 
                  viewer { 
                    sponsorshipsAsMaintainer (includePrivate: true, first: 100) {
                      nodes {
                        sponsor {
                          login
                        }
                      }
                    }
                  }
                }"
            };

            var grapqlClient = new GraphQLClient("https://");
            var graphqlResponse = await grapqlClient.GetAsync(donorRequest);

            foreach (var node in graphqlResponse.Data.viewer.sponsorshipsAsMaintainer.nodes)
            {
                result.Add(node.sponsor.login);
            }

            return result;
        }
    }
}