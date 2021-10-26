using CommonLayer;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ServiceLayer.Users
{
    public interface IAntiForgeryCookieService
    {
        void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims);
        void DeleteAntiForgeryCookies();
    }

    public class AntiForgeryCookieService: IAntiForgeryCookieService
    {
        private const string XsrfTokenKey = "XSRF-TOKEN";

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAntiforgery _antiforgery;
        private readonly IOptions<AntiforgeryOptions> _antiforgeryOptions;
        public AntiForgeryCookieService(
            IHttpContextAccessor contextAccessor,
            IAntiforgery antiforgery,
            IOptions<AntiforgeryOptions> antiforgeryOptions)
        {
            _contextAccessor = contextAccessor;
            _contextAccessor.CheckArgumentIsNull(nameof(contextAccessor));

            _antiforgery = antiforgery;
            _antiforgery.CheckArgumentIsNull(nameof(antiforgery));

            _antiforgeryOptions = antiforgeryOptions;
            _antiforgeryOptions.CheckArgumentIsNull(nameof(antiforgeryOptions));
        }

        public void RegenerateAntiForgeryCookies(IEnumerable<Claim> claims)
        {
            var httpContext = _contextAccessor.HttpContext;
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme));
            var tokens = _antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(
                key: XsrfTokenKey,
                value: tokens.RequestToken,
                options: new CookieOptions
                {
                    HttpOnly = false, // Now JavaScript is able to read the cookie
                      //Path = "/",
                      //Domain = "http://localhost:4200/#/project/manage",
                      //Secure = false // set to false if not using SSL
                });
        }
   
        public void DeleteAntiForgeryCookies()
        {
            var cookeis = _contextAccessor.HttpContext.Response.Cookies;
            cookeis.Delete(_antiforgeryOptions.Value.Cookie.Name);
            cookeis.Delete(XsrfTokenKey);
        }
    }
}
