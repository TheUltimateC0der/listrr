using System.ComponentModel.DataAnnotations;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;

using Listrr.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Listrr.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public IActionResult OnGetAsync(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true);
            var userNameClaim = info.Principal.Claims.First(x => x.Type == Constants.Trakt_Claim_Ids_Slug);

            if (signInResult.Succeeded)
            {
                await UpdateTokens(
                    await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey),
                    info.LoginProvider,
                    info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_AccessToken)?.Value,
                    info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_RefreshToken)?.Value,
                    info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_ExpiresAt)?.Value
                );
                
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (signInResult.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                var user = new User { UserName = userNameClaim.Value };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, true);

                        await UpdateTokens(
                            await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey),
                            info.LoginProvider,
                            info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_AccessToken)?.Value,
                            info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_RefreshToken)?.Value,
                            info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_ExpiresAt)?.Value
                        );

                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }
        }


        private async Task UpdateTokens(User user, string loginProvider, string accessToken, string refreshToken, string expiresAt)
        {
            await _userManager.SetAuthenticationTokenAsync(user, loginProvider, Constants.TOKEN_AccessToken, accessToken);
            await _userManager.SetAuthenticationTokenAsync(user, loginProvider, Constants.TOKEN_RefreshToken, refreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, loginProvider, Constants.TOKEN_ExpiresAt, expiresAt);
        }
    }
}
