using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Exceptions;
using Listrr.Repositories;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using TraktNet;
using TraktNet.Enums;
using TraktNet.Exceptions;
using TraktNet.Objects.Authentication;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;
using TraktNet.Objects.Post.Users.CustomListItems;
using TraktNet.Requests.Parameters;
using TraktNet.Utils;

namespace Listrr.Services
{
    public class TraktService : ITraktService
    {
        private readonly TraktClient _traktClient;
        private readonly UserManager<User> _userManager;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly IUserLimitService _userLimitService;
        private readonly IIMDbRepository _imDbRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public TraktService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, TraktAPIConfiguration traktApiConfiguration, IUserLimitService userLimitService, IIMDbRepository imDbRepository)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _traktApiConfiguration = traktApiConfiguration ?? throw new ArgumentNullException(nameof(traktApiConfiguration));
            _userLimitService = userLimitService ?? throw new ArgumentNullException(nameof(userLimitService));
            _imDbRepository = imDbRepository ?? throw new ArgumentNullException(nameof(imDbRepository));

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, traktApiConfiguration.ClientSecret);
        }


        public async Task<TraktList> Create(TraktList model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await PrepareForApiRequest(model.Owner);

            var response = await _traktClient.Users.CreateCustomListAsync(
                "me",
                model.Name,
                $"{Constants.LIST_Description}\r\n\r\n{model.GetDescriptionText()}",
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

        public async Task<TraktList> Get(TraktList model)
        {
            await PrepareForApiRequest(model.Owner);

            var response = await _traktClient.Users.GetCustomListAsync("me", model.Id.ToString());

            if (!response.IsSuccess) throw response.Exception;

            model.Id = response.Value.Ids.Trakt;
            model.Slug = response.Value.Ids.Slug;
            model.Name = response.Value.Name;
            model.Items = response.Value.ItemCount;
            model.Likes = response.Value.Likes;

            return model;
        }

        public async Task<IList<ITraktMovie>> MovieSearch(TraktList model)
        {
            var list = new List<ITraktMovie>();

            await MovieSearch(model, list);

            return list;
        }

        public async Task<ITraktMovie> MovieSearch(TraktList model, string name, int year)
        {
            var result = await _traktClient.Search.GetTextQueryResultsAsync(
                TraktSearchResultType.Movie,
                name,
                null,
                new TraktSearchFilter(year, year),
                new TraktExtendedInfo().SetFull(),
                new TraktPagedParameters(null, 1)
            );

            if (!result.IsSuccess) throw result.Exception;

            return result.Value.FirstOrDefault()?.Movie;
        }




        private async Task MovieSearch(TraktList model, IList<ITraktMovie> list)
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
            if (model.SearchByTagline) searchFields |= TraktSearchField.Tagline;

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
                            model.Filter_Ratings_Trakt.From,
                            model.Filter_Ratings_Trakt.To
                        )
                    ), new TraktExtendedInfo().SetFull(),
                    new TraktPagedParameters(page, _traktApiConfiguration.FetchLimitSearch)
                );

                if (!result.IsSuccess) throw result.Exception;

                var userLimits = _userLimitService.Get(model.Owner.Level);

                foreach (var traktSearchResult in result.Value)
                {
                    if (traktSearchResult.Movie?.Votes != null && model.Filter_Ratings_Trakt.Votes > 0)
                        if (!(traktSearchResult.Movie.Votes >= model.Filter_Ratings_Trakt.Votes))
                            continue;

                    if (userLimits.ExclusionFilters)
                    {
                        if (model.ReverseFilter_Certifications_Movie?.Certifications != null && !string.IsNullOrEmpty(traktSearchResult.Movie.Certification))
                            if (model.ReverseFilter_Certifications_Movie.Certifications.Contains(traktSearchResult.Movie.Certification))
                                continue;

                        if (model.ReverseFilter_Countries?.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Movie.CountryCode))
                            if (model.ReverseFilter_Countries.Languages.Contains(traktSearchResult.Movie.CountryCode))
                                continue;

                        if (model.ReverseFilter_Genres?.Genres != null && traktSearchResult.Movie.Genres != null)
                            if (model.ReverseFilter_Genres.Genres.Any(traktSearchResult.Movie.Genres.Contains))
                                continue;

                        if (model.ReverseFilter_Languages?.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Movie.LanguageCode))
                            if (model.ReverseFilter_Languages.Languages.Contains(traktSearchResult.Movie.LanguageCode))
                                continue;

                        if (model.ReverseFilter_Translations?.Translations != null && traktSearchResult.Movie.AvailableTranslationLanguageCodes != null)
                            if (model.ReverseFilter_Translations.Translations.Any(traktSearchResult.Movie.AvailableTranslationLanguageCodes.Contains))
                                continue;
                    }

                    if (userLimits.IMDbRatings)
                    {
                        if (model.Filter_Ratings_IMDb != null)
                        {
                            if (!string.IsNullOrEmpty(traktSearchResult.Movie.Ids.Imdb))
                            {
                                var imdbRating = await _imDbRepository.Get(traktSearchResult.Movie.Ids.Imdb);
                                if (imdbRating != null)
                                {
                                    if (model.Filter_Ratings_IMDb.Votes > 0)
                                        if (!(imdbRating.Votes >= model.Filter_Ratings_IMDb.Votes))
                                            continue;

                                    if (model.Filter_Ratings_IMDb.From > 0)
                                        if (!(imdbRating.Rating >= model.Filter_Ratings_IMDb.From))
                                            continue;

                                    if (model.Filter_Ratings_IMDb.To > 0)
                                        if (!(imdbRating.Rating <= model.Filter_Ratings_IMDb.To))
                                            continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

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
                $"{Constants.LIST_Description}\r\n\r\n{model.GetDescriptionText()}",
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

            list.Items += result.Value.Added.Movies;
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

            list.Items -= result.Value.Deleted.Movies;
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
                model.Id.ToString(),
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
                if (traktSearchResult.Show.Year.HasValue && traktSearchResult.Show.Year.ToString().Length == 1)
                    traktSearchResult.Show.Year *= 1000;

                if (traktSearchResult.Show.Year.HasValue && traktSearchResult.Show.Year.ToString().Length == 2)
                    traktSearchResult.Show.Year *= 100;

                if (traktSearchResult.Show.Year.HasValue && traktSearchResult.Show.Year.ToString().Length == 3)
                    traktSearchResult.Show.Year *= 10;


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

            list.Items += result.Value.Added.Shows;
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

            list.Items -= result.Value.Deleted.Shows;
        }


        public async Task<IList<ITraktShow>> ShowSearch(TraktList model)
        {
            var list = new List<ITraktShow>();

            await ShowSearch(model, list);

            return list;
        }

        public async Task<ITraktShow> ShowSearch(TraktList model, string name, int year)
        {
            var result = await _traktClient.Search.GetTextQueryResultsAsync(
                TraktSearchResultType.Show,
                name,
                null,
                new TraktSearchFilter(year, year),
                new TraktExtendedInfo().SetFull(),
                new TraktPagedParameters(null, 1)
            );

            if (!result.IsSuccess) throw result.Exception;

            return result.Value.FirstOrDefault()?.Show;
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

            if (model.Filter_Status.Status?.Contains("canceled") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.Canceled);
            if (model.Filter_Status.Status?.Contains("ended") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.Ended);
            if (model.Filter_Status.Status?.Contains("in production") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.InProduction);
            if (model.Filter_Status.Status?.Contains("returning series") == true) traktShowStatus.Add(TraktNet.Enums.TraktShowStatus.ReturningSeries);

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
                            model.Filter_Ratings_Trakt.From,
                            model.Filter_Ratings_Trakt.To
                        ),
                        model.Filter_Certifications_Show.Certifications,
                        model.Filter_Networks.Networks,
                        traktShowStatus.Count != 0 ? traktShowStatus.ToArray() : null
                    ), new TraktExtendedInfo().SetFull(),
                    new TraktPagedParameters(page, _traktApiConfiguration.FetchLimitSearch)
                );


                if (!result.IsSuccess) throw result.Exception;

                var userLimits = _userLimitService.Get(model.Owner.Level);

                foreach (var traktSearchResult in result.Value)
                {
                    if (traktSearchResult.Show?.Votes != null && model.Filter_Ratings_Trakt.Votes > 0)
                        if (!(traktSearchResult.Show.Votes >= model.Filter_Ratings_Trakt.Votes))
                            continue;

                    if (userLimits.ExclusionFilters)
                    {
                        if (model.ReverseFilter_Certifications_Show?.Certifications != null && !string.IsNullOrEmpty(traktSearchResult.Show.Certification))
                            if (model.ReverseFilter_Certifications_Show.Certifications.Contains(traktSearchResult.Show.Certification))
                                continue;

                        if (model.ReverseFilter_Countries?.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Show.CountryCode))
                            if (model.ReverseFilter_Countries.Languages.Contains(traktSearchResult.Show.CountryCode))
                                continue;

                        if (model.ReverseFilter_Genres?.Genres != null && traktSearchResult.Show.Genres != null)
                            if (model.ReverseFilter_Genres.Genres.Any(traktSearchResult.Show.Genres.Contains))
                                continue;

                        if (model.ReverseFilter_Languages?.Languages != null && !string.IsNullOrEmpty(traktSearchResult.Show.LanguageCode))
                            if (model.ReverseFilter_Languages.Languages.Contains(traktSearchResult.Show.LanguageCode))
                                continue;

                        if (model.ReverseFilter_Networks?.Networks != null && !string.IsNullOrEmpty(traktSearchResult.Show.Network))
                            if (model.ReverseFilter_Networks.Networks.Contains(traktSearchResult.Show.Network))
                                continue;

                        if (model.ReverseFilter_Status?.Status != null && traktSearchResult.Show.Status != null)
                            if (model.ReverseFilter_Status.Status.Contains(traktSearchResult.Show.Status.ObjectName))
                                continue;

                        if (model.ReverseFilter_Translations?.Translations != null && traktSearchResult.Show.AvailableTranslationLanguageCodes != null)
                            if (model.ReverseFilter_Translations.Translations.Any(traktSearchResult.Show.AvailableTranslationLanguageCodes.Contains))
                                continue;
                    }

                    if (userLimits.IMDbRatings)
                    {
                        if (model.Filter_Ratings_IMDb != null)
                        {
                            if (!string.IsNullOrEmpty(traktSearchResult.Show.Ids.Imdb))
                            {
                                var imdbRating = await _imDbRepository.Get(traktSearchResult.Show.Ids.Imdb);
                                if (imdbRating != null)
                                {
                                    if (model.Filter_Ratings_IMDb.Votes > 0)
                                        if (!(imdbRating.Votes >= model.Filter_Ratings_IMDb.Votes))
                                            continue;

                                    if (model.Filter_Ratings_IMDb.From > 0)
                                        if (!(imdbRating.Rating >= model.Filter_Ratings_IMDb.From))
                                            continue;

                                    if (model.Filter_Ratings_IMDb.To > 0)
                                        if (!(imdbRating.Rating <= model.Filter_Ratings_IMDb.To))
                                            continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    if (!list.Contains(traktSearchResult.Show, new TraktShowComparer()))
                        list.Add(traktSearchResult.Show);
                }

                if (result.PageCount == page) break;

                page++;

                await Task.Delay(_traktApiConfiguration.DelaySearch);
            }
        }



        private async Task PrepareForApiRequest(User user = null)
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

                _traktClient.Authorization = TraktAuthorization.CreateWith(access_token, refresh_token);
                _traktClient.Configuration.ForceAuthorization = true;

                var expiresAt = DateTime.ParseExact(expiresAtToken, new[] { "yyyy-MM-ddTHH:mm:ss.fffffffK", "MM/dd/yyyy HH:mm:ss" }, null, DateTimeStyles.None);
                if (expiresAt < DateTime.Now.AddDays(-5))
                {
                    try
                    {
                        var tokenResponse = await _traktClient.Authentication.RefreshAuthorizationAsync();

                        if (tokenResponse.IsSuccess)
                        {
                            await _userManager.SetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_ExpiresAt, tokenResponse.Value.CreatedAt.AddSeconds(Convert.ToInt32(tokenResponse.Value.ExpiresInSeconds)).ToString(CultureInfo.InvariantCulture));
                            await _userManager.SetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_AccessToken, tokenResponse.Value.AccessToken);
                            await _userManager.SetAuthenticationTokenAsync(user, Constants.TOKEN_LoginProvider, Constants.TOKEN_RefreshToken, tokenResponse.Value.RefreshToken);
                        }
                    }
                    catch (TraktBadRequestException e)
                    {
                        throw new RefreshTokenBadRequestException();
                    }
                }
            }
        }
    }
}