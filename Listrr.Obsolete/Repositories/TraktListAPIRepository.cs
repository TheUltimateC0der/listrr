using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TraktNet;
using TraktNet.Enums;
using TraktNet.Objects.Authentication;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;
using TraktNet.Objects.Post.Users.CustomListItems;
using TraktNet.Requests.Parameters;
using TraktNet.Utils;

namespace Listrr.Repositories
{
    public class TraktListAPIRepository : ITraktListAPIRepository
    {

        private readonly TraktClient _traktClient;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public TraktListAPIRepository(UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor, TraktAPIConfiguration traktApiConfiguration)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _traktApiConfiguration = traktApiConfiguration;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, traktApiConfiguration.ClientSecret);
        }


        public async Task<TraktList> Create(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            var response = await _traktClient.Users.CreateCustomListAsync(
                "me",
                model.Name,
                Constants.LIST_Description,
                TraktAccessScope.Public,
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

        public async Task Delete(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            await _traktClient.Users.DeleteCustomListAsync(model.Owner.UserName, model.Slug);
        }

        public async Task<TraktList> Get(uint id, IdentityUser user = null)
        {
            await PrepareForApiRequest(user);

            var response = await _traktClient.Users.GetCustomListAsync("me", id.ToString());

            if (!response.IsSuccess) throw response.Exception;

            return new TraktList()
            {
                Id = response.Value.Ids.Trakt,
                Slug = response.Value.Ids.Slug,
                Name = response.Value.Name,
                Items = response.Value.ItemCount,
                Likes = response.Value.Likes
            };
        }

        public async Task<IList<ITraktMovie>> MovieSearch(TraktList model)
        {
            var list = new List<ITraktMovie>();

            await MovieSearch(model, list);

            return list;
        }

        private async Task MovieSearch(TraktList model, IList<ITraktMovie> list)
        {
            uint? page = 0;

            TraktSearchField searchFields = new TraktSearchField();

            if (model.SearchByAlias) searchFields = searchFields | TraktSearchField.Aliases;
            if (model.SearchByBiography) searchFields = searchFields | TraktSearchField.Biography;
            if (model.SearchByDescription) searchFields = searchFields | TraktSearchField.Description;
            if (model.SearchByName) searchFields = searchFields | TraktSearchField.Name;
            if (model.SearchByOverview) searchFields = searchFields | TraktSearchField.Overview;
            if (model.SearchByPeople) searchFields = searchFields | TraktSearchField.People;
            if (model.SearchByTagline) searchFields = searchFields | TraktSearchField.Tagline;
            if (model.SearchByTitle) searchFields = searchFields | TraktSearchField.Title;
            if (model.SearchByTranslations) searchFields = searchFields | TraktSearchField.Translations;
            if (model.SearchByTagline) searchFields = searchFields | TraktSearchField.Tagline;

            while (true)
            {
                var result = await _traktClient.Search.GetTextQueryResultsAsync(
                    TraktSearchResultType.Movie,
                    model.Query,
                    searchFields,
                    new TraktSearchFilter(
                        model.Filter_Years.From,
                        model.Filter_Years.To,
                        model.Filter_Genres.Genres,
                        model.Filter_Languages.Languages,
                        model.Filter_Countries.Languages,
                        new Range<int>(
                            model.Filter_Runtimes.From,
                            model.Filter_Runtimes.To
                        ),
                        new Range<int>(
                            model.Filter_Ratings.From,
                            model.Filter_Ratings.To
                        )
                    ), new TraktExtendedInfo().SetFull(),
                    new TraktPagedParameters(page, _traktApiConfiguration.FetchLimitSearch)
                );

                if (!result.IsSuccess) throw result.Exception;

                foreach (var traktSearchResult in result.Value)
                {
                    if (traktSearchResult.Movie.Votes != null && model.Filter_Ratings.Votes > 0)
                        if (!(traktSearchResult.Movie.Votes >= model.Filter_Ratings.Votes))
                            continue;

                    if (model.ReverseFilter_Certifications_Movie.Certifications != null && !string.IsNullOrEmpty(traktSearchResult.Movie.Certification))
                        if (model.ReverseFilter_Certifications_Movie.Certifications.Contains(traktSearchResult.Movie.Certification))
                            continue;

                    if (model.ReverseFilter_Countries.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Movie.CountryCode))
                        if (model.ReverseFilter_Countries.Languages.Contains(traktSearchResult.Movie.CountryCode))
                            continue;

                    if (model.ReverseFilter_Genres.Genres != null && traktSearchResult.Movie.Genres != null)
                        if (model.ReverseFilter_Genres.Genres.Any(traktSearchResult.Movie.Genres.Contains))
                            continue;

                    if (model.ReverseFilter_Languages.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Movie.LanguageCode))
                        if (model.ReverseFilter_Languages.Languages.Contains(traktSearchResult.Movie.LanguageCode))
                            continue;

                    if (model.ReverseFilter_Translations.Translations != null && traktSearchResult.Movie.AvailableTranslationLanguageCodes != null)
                        if (model.ReverseFilter_Translations.Translations.Any(traktSearchResult.Movie.AvailableTranslationLanguageCodes.Contains))
                            continue;

                    if (!list.Contains(traktSearchResult.Movie, new TraktMovieComparer()))
                        list.Add(traktSearchResult.Movie);
                }

                if (result.PageCount == page) break;
                
                page++;

                await Task.Delay(_traktApiConfiguration.DelaySearch);
            }
        }

        public async Task<TraktList> Update(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            await _traktClient.Users.UpdateCustomListAsync(
                "me",
                model.Id.ToString(),
                model.Name,
                Constants.LIST_Description,
                TraktAccessScope.Public,
                false,
                false
            );

            return model;
        }

        public async Task<bool> Exists(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            try
            {
                await _traktClient.Users.GetCustomListAsync(
                    model.Owner.UserName,
                    model.Slug
                );
            }
            catch (TraktNet.Exceptions.TraktListNotFoundException)
            {
                return false;
            }
            

            return true;
        }


        public async Task<IList<ITraktMovie>> GetMovies(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            var result = new List<ITraktMovie>();

            await GetMovies(model, result);

            return result;
        }

        private async Task GetMovies(TraktList model, IList<ITraktMovie> list, uint? page = 0)
        {
            var result = await _traktClient.Users.GetCustomListItemsAsync(
                model.Owner.UserName,
                model.Slug,
                TraktListItemType.Movie,
                new TraktExtendedInfo().SetMetadata(),
                new TraktPagedParameters(
                    page,
                    _traktApiConfiguration.FetchLimitList
                )
            );

            if (!result.IsSuccess) throw result.Exception;

            foreach (var traktSearchResult in result.Value)
            {
                list.Add(traktSearchResult.Movie);
            }

            if (result.PageCount > page)
            {
                await Task.Delay(_traktApiConfiguration.DelayList);
                await GetMovies(model, list, page + 1);
            }

        }


        public async Task AddMovies(IList<ITraktMovie> movies, TraktList list)
        {
            await PrepareForApiRequest(list.Owner);
            
            var result = await _traktClient.Users.AddCustomListItemsAsync(
                list.Owner.UserName,
                list.Slug,
                TraktUserCustomListItemsPost.Builder().AddMovies(movies).Build()
            );

            if (!result.IsSuccess) throw result.Exception;
        }

        public async Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list)
        {
            await PrepareForApiRequest(list.Owner);

            var result = await _traktClient.Users.RemoveCustomListItemsAsync(
                list.Owner.UserName,
                list.Slug,
                TraktUserCustomListItemsPost.Builder().AddMovies(movies).Build()
            );

            if (!result.IsSuccess) throw result.Exception;
        }


        

        public async Task<IList<ITraktShow>> GetShows(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            var result = new List<ITraktShow>();

            await GetShows(model, result);

            return result;
        }

        private async Task GetShows(TraktList model, IList<ITraktShow> list, uint? page = 0)
        {
            var result = await _traktClient.Users.GetCustomListItemsAsync(
                model.Owner.UserName,
                model.Slug,
                TraktListItemType.Show,
                new TraktExtendedInfo().SetMetadata(),
                new TraktPagedParameters(
                    page,
                    _traktApiConfiguration.FetchLimitList
                )
            );

            if (!result.IsSuccess) throw result.Exception;

            foreach (var traktSearchResult in result.Value)
            {
                list.Add(traktSearchResult.Show);
            }

            if (result.PageCount > page)
            {
                await Task.Delay(_traktApiConfiguration.DelayList);
                await GetShows(model, list, page + 1);
            }

        }


        public async Task AddShows(IList<ITraktShow> shows, TraktList list)
        {
            await PrepareForApiRequest(list.Owner);

            var result = await _traktClient.Users.AddCustomListItemsAsync(
                list.Owner.UserName,
                list.Slug,
                TraktUserCustomListItemsPost.Builder().AddShows(shows).Build()
            );

            if (!result.IsSuccess) throw result.Exception;
        }

        public async Task RemoveShows(IEnumerable<ITraktShow> shows, TraktList list)
        {
            await PrepareForApiRequest(list.Owner);

            var result = await _traktClient.Users.RemoveCustomListItemsAsync(
                list.Owner.UserName,
                list.Slug,
                TraktUserCustomListItemsPost.Builder().AddShows(shows).Build()
            );

            if (!result.IsSuccess) throw result.Exception;
        }


        public async Task<IList<ITraktShow>> ShowSearch(TraktList model)
        {
            var list = new List<ITraktShow>();

            await ShowSearch(model, list);

            return list;
        }

        private async Task ShowSearch(TraktList model, IList<ITraktShow> list)
        {
            uint? page = 0;

            TraktSearchField searchFields = new TraktSearchField();

            if (model.SearchByAlias) searchFields |= TraktSearchField.Aliases;
            if (model.SearchByBiography) searchFields |= TraktSearchField.Biography;
            if (model.SearchByDescription) searchFields |= TraktSearchField.Description;
            if (model.SearchByName) searchFields |= TraktSearchField.Name;
            if (model.SearchByOverview) searchFields |= TraktSearchField.Overview;
            if (model.SearchByPeople) searchFields |= TraktSearchField.People;
            if (model.SearchByTagline) searchFields |= TraktSearchField.Tagline;
            if (model.SearchByTitle) searchFields |= TraktSearchField.Title;
            if (model.SearchByTranslations) searchFields |= TraktSearchField.Translations;


            var traktShowStatus = new List<TraktNet.Enums.TraktShowStatus>();

            if(model.Filter_Status.Status?.Contains("canceled") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.Canceled);
            if(model.Filter_Status.Status?.Contains("ended") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.Ended);
            if(model.Filter_Status.Status?.Contains("in production") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.InProduction);
            if(model.Filter_Status.Status?.Contains("returning series") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.ReturningSeries);
            
            while (true)
            {
                var result = await _traktClient.Search.GetTextQueryResultsAsync(
                    TraktSearchResultType.Show,
                    model.Query,
                    searchFields,
                    new TraktSearchFilter(
                        model.Filter_Years.From,
                        model.Filter_Years.To,
                        model.Filter_Genres.Genres,
                        model.Filter_Languages.Languages,
                        model.Filter_Countries.Languages,
                        new Range<int>(
                            model.Filter_Runtimes.From,
                            model.Filter_Runtimes.To
                        ),
                        new Range<int>(
                            model.Filter_Ratings.From,
                            model.Filter_Ratings.To
                        ),
                        model.Filter_Certifications_Show.Certifications,
                        model.Filter_Networks.Networks, 
                        traktShowStatus.Count != 0 ? traktShowStatus.ToArray() : null
                    ), new TraktExtendedInfo().SetFull(),
                    new TraktPagedParameters(page, _traktApiConfiguration.FetchLimitSearch)
                );


                if (!result.IsSuccess) throw result.Exception;

                foreach (var traktSearchResult in result.Value)
                {
                    if (traktSearchResult.Show.Votes != null && model.Filter_Ratings.Votes > 0)
                        if (!(traktSearchResult.Show.Votes >= model.Filter_Ratings.Votes))
                            continue;

                    if (model.ReverseFilter_Certifications_Show.Certifications != null && !string.IsNullOrEmpty(traktSearchResult.Show.Certification))
                        if(model.ReverseFilter_Certifications_Show.Certifications.Contains(traktSearchResult.Show.Certification))
                            continue;

                    if (model.ReverseFilter_Countries.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Show.CountryCode))
                        if (model.ReverseFilter_Countries.Languages.Contains(traktSearchResult.Show.CountryCode))
                            continue;

                    if (model.ReverseFilter_Genres.Genres != null && traktSearchResult.Show.Genres != null)
                        if (model.ReverseFilter_Genres.Genres.Any(traktSearchResult.Show.Genres.Contains))
                            continue;

                    if (model.ReverseFilter_Languages.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Show.LanguageCode))
                        if (model.ReverseFilter_Languages.Languages.Contains(traktSearchResult.Show.LanguageCode))
                            continue;

                    if (model.ReverseFilter_Networks.Networks != null && !string.IsNullOrEmpty(traktSearchResult.Show.Network))
                        if (model.ReverseFilter_Networks.Networks.Contains(traktSearchResult.Show.Network))
                            continue;

                    if (model.ReverseFilter_Status.Status != null && traktSearchResult.Show.Status != null)
                        if (model.ReverseFilter_Status.Status.Contains(traktSearchResult.Show.Status.ObjectName))
                            continue;

                    if (model.ReverseFilter_Translations.Translations != null && traktSearchResult.Show.AvailableTranslationLanguageCodes != null)
                        if (model.ReverseFilter_Translations.Translations.Any(traktSearchResult.Show.AvailableTranslationLanguageCodes.Contains))
                            continue;


                    if (!list.Contains(traktSearchResult.Show, new TraktShowComparer()))
                        list.Add(traktSearchResult.Show);
                }

                if (result.PageCount == page) break;

                page++;

                await Task.Delay(_traktApiConfiguration.DelaySearch);
            }
        }



        private async Task PrepareForApiRequest(IdentityUser user = null)
        {
            if (user == null && _httpContextAccessor.HttpContext != null)
            {
                user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            }

            if (user != null)
            {
                var expiresAtToken = await _userManager.GetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_ExpiresAt);
                var access_token = await _userManager.GetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_AccessToken);
                var refresh_token = await _userManager.GetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_RefreshToken);

                var expiresAt = DateTime.Parse(expiresAtToken);

                if (expiresAt < DateTime.Now)
                {
                    //Refresh the token
                }

                _traktClient.Authorization = TraktAuthorization.CreateWith(access_token);
            }
        }

    }
}