using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Listrr.API.Trakt;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Refit;
using TraktNet;
using TraktNet.Enums;
using TraktNet.Objects.Authentication;
using TraktNet.Requests.Parameters;

namespace Listrr.Repositories
{
    public class TraktListAPIRepository : ITraktListAPIRepository
    {

        private readonly TraktClient traktClient;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;

        public TraktListAPIRepository(UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;

            traktClient = new TraktClient(configuration["Trakt:ClientID"], configuration["Trakt:ClientSecret"]);
        }


        public async Task<TraktList> Create(TraktList model)
        {
            await PrepareForAPIRequest();

            var response = await traktClient.Users.CreateCustomListAsync(
                "me",
                model.Name,
                Constants.LIST_Description,
                TraktAccessScope.Private,
                false,
                false
            );

            if (!response.IsSuccess) throw response.Exception;

            model.Id = response.Value.Ids.Trakt;
            model.Slug = response.Value.Ids.Slug;
            model.LastProcessed = DateTime.Now;
            model.Process = true;

            return model;
        }

        public async Task<TraktList> Get(uint id)
        {
            await PrepareForAPIRequest();

            var response = await traktClient.Users.GetCustomListAsync("me", id.ToString());

            if (!response.IsSuccess) throw response.Exception;

            return new TraktList()
            {
                Id = response.Value.Ids.Trakt,
                Slug = response.Value.Ids.Slug,
                Name = response.Value.Name
            };
        }

        public async Task<TraktList> Update(TraktList model)
        {
            await PrepareForAPIRequest();

            await traktClient.Users.UpdateCustomListAsync(
                "me",
                model.Id.ToString(),
                model.Name,
                Constants.LIST_Description,
                TraktAccessScope.Private,
                false,
                false
            );

            return model;
        }

        private async Task PrepareForAPIRequest()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

            var expiresAtToken = await userManager.GetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_ExpiresAt);
            var access_token = await userManager.GetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_AccessToken);
            var refresh_token = await userManager.GetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_RefreshToken);

            var expiresAt = DateTime.Parse(expiresAtToken);

            if (expiresAt < DateTime.Now)
            {
                //Refresh the token
            }

            traktClient.Authorization = TraktAuthorization.CreateWith(access_token);
        }
    }
}