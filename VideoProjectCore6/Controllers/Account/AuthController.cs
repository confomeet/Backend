using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
    public class AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IConfiguration configuration) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;
        private readonly string _backendBaseUrl = configuration["CurrentHostName"]!;
        public const string ControllerRoute = "/api/v1/web/Auth";
        private const string OIDCLogInAction = "OIDCLogIn";
        public const string OIDCLoggedOutAction = "SuccessLoggedOut";
        public const string OIDCLoggedInAction = "Success";

        [HttpGet(OIDCLogInAction)]
        public IActionResult OIDCLogIn()
        {
            AuthenticationProperties authProp = new()
            {
                RedirectUri = Utility.Uri.CombineUri(_backendBaseUrl, ControllerRoute, OIDCLoggedInAction)
            };
            return Challenge(authProp, "oidc");
        }

        [HttpGet(OIDCLoggedOutAction)]
        public IActionResult SuccessLoggedOut()
        {
            return Redirect(_configuration["CurrentHostName"]!);
        }

        [HttpGet(OIDCLoggedInAction)]
        public async Task<IActionResult> Success() {
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

            return Redirect("/meet");
        }

        [HttpGet("ExternalAuthProviders")]
        public IActionResult GetExternalAuthProviders() {
            if (string.IsNullOrEmpty(_configuration["oidc:authority"]) || string.IsNullOrEmpty(_configuration["oidc:client_id"]))
                return Ok(new List<string>());
            List<ProviderDTO> providers =
            [
                new ProviderDTO {
                    Name = _configuration["oidc:name"] ?? _configuration["oidc:client_id"]!,
                    Url = Utility.Uri.CombineUri(_backendBaseUrl, ControllerRoute, OIDCLogInAction)
                }
            ];
            return Ok(new APIResult().SuccessMe(1, "", false, APIResult.RESPONSE_CODE.OK, providers));
        }

        [HttpGet("LogOut")]
        public async Task<IActionResult> LogOut() {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated) {
                string id_token = (await HttpContext.GetTokenAsync("oidc", "id_token"))!;

                AuthenticationProperties authProp = new()
                {
                    RedirectUri = Utility.Uri.CombineUri(_backendBaseUrl, ControllerRoute, OIDCLoggedOutAction)
                };
                await HttpContext.SignOutAsync("oidc", authProp);

                string redirectLocation = Response.Headers.Location!;
                redirectLocation += "&id_token_hint=" + id_token;
                Response.Headers.Location = redirectLocation;

                return Redirect(Response.Headers.Location!);
            }
            return Ok(new APIResult().SuccessMe(1));
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
    }
}