using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.AccountDto;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.Repositories.IUserRepository;

namespace VideoProjectCore6.Controllers.Account
{
    public class ProviderDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Url { get; set; } = string.Empty;
    }

    [AllowAnonymous]
    [Controller]
    [Route(ControllerRoute)]
    public class OIDCAuthController(ILogger<OIDCAuthController> logger, IUserRepository userRepository, IConfiguration configuration) : ControllerBase
    {
        private readonly ILogger<OIDCAuthController> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly string _backendBaseUrl = configuration["CONFOMEET_BASE_URL"]!;
        public const string ControllerRoute = "/api/v1/web/Auth";
        private const string OIDCLogInAction = "OIDCLogIn";
        public const string OIDCLoggedOutAction = "SuccessLoggedOut";
        public const string OIDCLoggedInAction = "SuccessLoggedIn";

        [HttpGet(OIDCLogInAction)]
        public IActionResult OIDCLogIn([FromQuery] string RedirectUri)
        {
            AuthenticationProperties authProp = new()
            {
                RedirectUri = BuildRouteToActionWithRedirect(OIDCLoggedInAction, RedirectUri),
            };
            return Challenge(authProp, "oidc");
        }

        [HttpGet(OIDCLoggedInAction)]
        public async Task<IActionResult> SuccessLoggedIn([FromQuery] string ConfomeetRedirectUri) {
            var authResult = await HttpContext.AuthenticateAsync("oidc");
            if (!authResult.Succeeded) {
                return BadRequest("Authentication failed");
            }

            string parseError = "Bad request";
            bool parseClaim(string claimName, out string? value)
            {
                value = authResult.Principal!.FindFirstValue(claimName);
                if (string.IsNullOrEmpty(value))
                {
                    parseError = $"OIDC Provider provides unusable ID token: ${claimName} is missing or empty";
                    return false;
                }
                return true;
            }

            if (   !parseClaim("email", out string? email)
                || !parseClaim("sub", out string? sub)
                || !parseClaim("name", out string? name)
            )
                return BadRequest(parseError);

            var logInResultDto = await _userRepository.LogInExternal(sub!, "oidc", email!, name!);
            FillSessionWithUserInfo(logInResultDto);

            var redirectUri = !string.IsNullOrEmpty(ConfomeetRedirectUri) ? ConfomeetRedirectUri : _configuration["CONFOMEET_BASE_URL"]!;
            return Redirect(redirectUri);
        }

        [HttpGet("ExternalAuthProviders")]
        public IActionResult GetExternalAuthProviders() {
            if (string.IsNullOrEmpty(_configuration["CONFOMEET_OIDC_AUTHORITY"]) || string.IsNullOrEmpty(_configuration["CONFOMEET_OIDC_CLIENT_ID"]))
                return Ok(new List<string>());
            List<ProviderDTO> providers =
            [
                new ProviderDTO {
                    Name = _configuration["CONFOMEET_OIDC_NAME"] ?? _configuration["CONFOMEET_OIDC_CLIENT_ID"]!,
                    Url = Utility.Uri.CombineUri(_backendBaseUrl, ControllerRoute, OIDCLogInAction)
                }
            ];
            return Ok(new APIResult().SuccessMe(1, "", false, APIResult.RESPONSE_CODE.OK, providers));
        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut([FromQuery] string RedirectUri) {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(_configuration["CONFOMEET_OIDC_AUTHORITY"])) {
                // We are authenticated and have oidc integration enabled.
                string? id_token = await HttpContext.GetTokenAsync("oidc", "id_token");
                if (string.IsNullOrEmpty(id_token)) // If no id_token provided, then we are authenticated locally
                    return Redirect(RedirectUri);

                AuthenticationProperties authProp = new()
                {
                    RedirectUri = BuildRouteToActionWithRedirect(OIDCLoggedOutAction, RedirectUri),
                };
                await HttpContext.SignOutAsync("oidc", authProp);

                string redirectLocation = Response.Headers.Location!;
                redirectLocation += "&id_token_hint=" + id_token;
                Response.Headers.Location = redirectLocation;
                return Redirect(Response.Headers.Location!);
            }
            return Redirect(RedirectUri);
        }

        [HttpGet(OIDCLoggedOutAction)]
        public IActionResult SuccessLoggedOut([FromQuery] string ConfomeetRedirectUri)
        {
            var redirectUri = !string.IsNullOrEmpty(ConfomeetRedirectUri) ? ConfomeetRedirectUri : _configuration["CONFOMEET_BASE_URL"]!;
            return Redirect(redirectUri);
        }

        private static readonly JsonSerializerOptions AuthInfoSerializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private void FillSessionWithUserInfo(LogInResultDto logInResult) {
            var authUser = JsonSerializer.Serialize(logInResult, typeof(LogInResultDto), AuthInfoSerializeOptions);
            var b64AuthUser = Convert.ToBase64String(Encoding.UTF8.GetBytes(authUser));
            Response.Cookies.Append("authUser", b64AuthUser, new CookieOptions{
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                Secure = true,
            });
        }

        private string BuildRouteToActionWithRedirect(string actionName, string redirectUri) {
            var uri = Utility.Uri.CombineUri(Request.PathBase, ControllerRoute, actionName);
            if (!string.IsNullOrEmpty(redirectUri)) {
                uri += "?ConfomeetRedirectUri=";
                uri += HttpUtility.UrlEncode(redirectUri);
            }
            return uri;
        }
    }
}