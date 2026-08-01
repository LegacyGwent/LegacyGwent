using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Cynthia.Card.Server.Controllers
{
    [Route("culture")]
    public class CultureController : Controller
    {
        [HttpGet("set")]
        public IActionResult Set(string culture, string returnUrl = "/")
        {
            var selectedCulture = string.Equals(culture, "en-US", StringComparison.OrdinalIgnoreCase)
                ? "en-US"
                : "zh-CN";

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(selectedCulture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    IsEssential = true,
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps
                });

            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/");
        }
    }
}
