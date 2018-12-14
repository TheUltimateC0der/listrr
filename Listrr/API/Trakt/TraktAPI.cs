using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.API.Trakt.Models;
using Listrr.API.Trakt.Models.Filters;
using Refit;

namespace Listrr.API.Trakt
{
    public interface ITraktAPI
    {

        [Get("/search/movie")]
        Task<List<Movie>> SearchMovies(
            string title,
            string tagline,
            string overview,
            string people,
            string translations,
            string aliases,
            YearsCommonFilter yearsCommonFilter,
            RuntimesCommonFilter runtimesCommonFilter,
            RatingsCommonFilter ratingsCommonFilter,
            LanguagesCommonFilter languagesCommonFilter,
            GenresCommonFilter genresCommonFilter,
            CountriesCommonFilter countriesCommonFilter,
            CertificationsMovieFilter certificationsMovieFilter
        );

        [Get("/search/show")]
        Task<List<Movie>> SearchShows(
            string title,
            string overview,
            string people,
            string translations,
            string aliases,
            YearsCommonFilter yearsCommonFilter,
            RuntimesCommonFilter runtimesCommonFilter,
            RatingsCommonFilter ratingsCommonFilter,
            LanguagesCommonFilter languagesCommonFilter,
            GenresCommonFilter genresCommonFilter,
            CountriesCommonFilter countriesCommonFilter,
            StatusShowFilter statusShowFilter,
            NetworksShowFilter networksShowFilter,
            CertificationsShowFilter certificationsShowFilter
        );


    }
}