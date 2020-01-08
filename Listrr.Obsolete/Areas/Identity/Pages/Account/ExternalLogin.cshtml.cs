using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
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
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true);
            if (signInResult.Succeeded)
            {
                await UpdateTokens(
                    await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey),
                    info.LoginProvider,
                    info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_AccessToken).Value,
                    info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_RefreshToken).Value,
                    info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_ExpiresAt).Value
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
                var user = new IdentityUser { UserName = info.ProviderKey };
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
                            info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_AccessToken).Value,
                            info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_RefreshToken).Value,
                            info.AuthenticationTokens.FirstOrDefault(x => x.Name == Constants.TOKEN_ExpiresAt).Value
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

        private async Task UpdateTokens(IdentityUser user, string loginProvider, string access_token, string refresh_token, string expires_at)
        {
            await _userManager.SetAuthenticationTokenAsync(user, loginProvider, Constants.TOKEN_AccessToken, access_token);
            await _userManager.SetAuthenticationTokenAsync(user, loginProvider, Constants.TOKEN_RefreshToken, refresh_token);
            await _userManager.SetAuthenticationTokenAsync(user, loginProvider, Constants.TOKEN_ExpiresAt, expires_at);
        }
    }
}