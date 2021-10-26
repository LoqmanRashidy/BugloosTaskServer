using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonLayer;
using CommonLayer.Custom;
using Datalayer.Models;
using Datalayer.Models.Users;
using Datalayer.ViewModels;
using DataLayer.Context;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using ServiceLayer.BaseSystem;
using ServiceLayer.Users;
using ServicesLayer.BaseSystem;
using ServicesLayer.Users;
using static CommonLayer.EnumExtensions;

namespace WebApp.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("BuglossPolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IOptionsSnapshot<ApiSettings> _apiSettingsConfig;
        private readonly IUsersService _usersService;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IPersonService _personService;
        private readonly IUnitOfWork _uow;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly IAntiForgeryCookieService _antiforgery;
        private IAntiforgery __antiForgery;
        private readonly ISettingService _settingService;
        public AccountController(
           IOptionsSnapshot<ApiSettings> apiSettingsConfig,
           IUsersService usersService,
           ITokenStoreService tokenStoreService,
           IPersonService personService,
           ITokenFactoryService tokenFactoryService,
           IUnitOfWork uow,
           IAntiForgeryCookieService antiforgery,
           IAntiforgery antiForgery,
        ISettingService settingService)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _tokenStoreService = tokenStoreService;
            _tokenStoreService.CheckArgumentIsNull(nameof(tokenStoreService));

            _personService = personService;
            _personService.CheckArgumentIsNull(nameof(personService));

            _uow = uow;
            _uow.CheckArgumentIsNull(nameof(_uow));

            _antiforgery = antiforgery;
            _antiforgery.CheckArgumentIsNull(nameof(antiforgery));

            __antiForgery = antiForgery;
            __antiForgery.CheckArgumentIsNull(nameof(antiForgery));

            _tokenFactoryService = tokenFactoryService;
            _tokenFactoryService.CheckArgumentIsNull(nameof(tokenFactoryService));

            _settingService = settingService;
            _settingService.CheckArgumentIsNull(nameof(settingService));

            _apiSettingsConfig = apiSettingsConfig;
            _apiSettingsConfig.CheckArgumentIsNull(nameof(apiSettingsConfig));

        }

        #region Login

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] Login loginUser)
        {
            //if (!ReCaptchaClass.Validate(loginUser.RecaptchaToken, _apiSettingsConfig.Value.SafeDomains.ToList(), HttpContext.Request.HttpContext.Connection.RemoteIpAddress))
            //{
            //    return BadRequest("درحال حاضر قادر به ثبت درخواست‌های شما نمی‌باشیم.");
            //}
            try
            {
                Person person = null;
                if (loginUser == null)
                {
                    return BadRequest(new { ResourceFa.Error.message, status = Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest });
                }

                User user = await _usersService.FindUserAsync(loginUser.Username, loginUser.Password);

                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new { message = ResourceFa.ErrorUserNotExist, status = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized });
                }
                else
                {
                    person = await _personService.FindByIdAsync(user.PersonId);
                }

                JwtTokensData result = await _tokenFactoryService.CreateJwtTokensAsync(user);

                await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);
                await _uow.SaveChangesAsync();

                _antiforgery.RegenerateAntiForgeryCookies(result.Claims);

                Globals.User = _usersService.JsonSingle(user);
                if (person != null)
                    Globals.Student = person;
                else Globals.Student = null;
                return Ok(
                   new
                   {
                       access_token = result.AccessToken,
                       refresh_token = result.RefreshToken,
                       data = Globals.User,
                       person = Globals.Student,
                       @const = await _settingService.FindAllAsync(),
                       ResourceFa.SuccessUserAuthorization.message,
                       status = Microsoft.AspNetCore.Http.StatusCodes.Status200OK
                   });

            }
            catch (Exception ex)
            {

                return BadRequest(new { ResourceFa.Error.message, status = Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest });
            }
          
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        [IgnoreAntiforgeryToken]
        public IActionResult AntiForgery()
        {
            var tokens = __antiForgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = false
            });
            return NoContent();

        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken([FromBody] JToken jsonBody)
        {
            string refreshTokenValue = jsonBody.Value<string>("refreshToken");
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return BadRequest("refreshToken is not set.");
            }
            var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
            if (token == null)
            {
                return Unauthorized(new { message = ResourceFa.ErrorTokenNotExist.message, status = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized });
            }

            JwtTokensData result = await _tokenFactoryService.CreateJwtTokensAsync(token.User);
            await _tokenStoreService.AddUserTokenAsync(token.User, result.RefreshTokenSerial, result.AccessToken, _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue));
            await _uow.SaveChangesAsync();

            _antiforgery.RegenerateAntiForgeryCookies(result.Claims);

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken, data = token.User, message = ResourceFa.SuccessUserAuthorization.message, status = Microsoft.AspNetCore.Http.StatusCodes.Status200OK });
        }
        #endregion Login
    }
}
