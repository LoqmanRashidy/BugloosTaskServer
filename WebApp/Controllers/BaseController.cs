using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLayer;
using CommonLayer.Custom;
using CommonLayer.Extension;
using Datalayer.Models;
using Datalayer.ViewModels;
using DataLayer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ServiceLayer.BaseSystem;
using ServicesLayer.BaseSystem;
using static CommonLayer.EnumExtensions;

namespace WebApp.Controllers.BaseSystem
{
    [ApiController]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnableCors("BuglossPolicy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = CustomPolicy.AdminTeacherStudent)]
    public partial class BaseController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IOptionsSnapshot<ApiSettings> _configurationSetting;
        private readonly ISettingService _settingService;
        public BaseController(
            IHttpContextAccessor httpContextAccessor,
            ISettingService settingService,
            IOptionsSnapshot<ApiSettings> configurationSetting)
        {
            _settingService = settingService;
            _settingService.CheckArgumentIsNull(nameof(settingService));

            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor.CheckArgumentIsNull(nameof(httpContextAccessor));

            _configurationSetting = configurationSetting;
            _configurationSetting.CheckArgumentIsNull(nameof(configurationSetting));

        }



        #region Setting


        [HttpGet]
        [Route("GetAllSetting")]
        [ProducesResponseType(typeof(List<Setting>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSetting()
        {
            try
            {
                var data = await _settingService.FindAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }
        }

        [HttpGet]
        [Route("GetSetting")]
        [ProducesResponseType(typeof(Setting), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSetting(long id)
        {
            try
            {
                var data = await _settingService.FindByIdAsync(id);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { msg = ex.Message, error = true });
            }
        }

        [HttpGet]
        [Route("GetSettingByKey")]
        [ProducesResponseType(typeof(Setting), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSettingByKey(string key)
        {
            try
            {
                var data = await _settingService.FindByKeyIdAsync(key);
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(new { msg = ex.Message, error = true });
            }
        }


        [HttpPost]
        [Route("AddEditSetting")]
        [ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddEditSetting([FromBody] Setting setting)
        {
            try
            {
                await _settingService.AddOrUpdateAsync(setting);
                return Ok(setting);
            }
            catch (Exception ex)
            {

                return BadRequest(new { msg = ex.Message, error = true });
            }
        }
        [HttpDelete]
        [Route("DeleteSetting")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> DeleteSetting(long id)
        {
            try
            {
                var del = await _settingService.RemoveAsync(id);
                if (del.IsSuccess == true)
                {
                    return Ok(ResourceFa.DeleteSuccess);
                }
                else
                {
                    return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ResourceFa.DeleteForChildError, del.childs) });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ex.Message) });
            }
        }

        [HttpDelete]
        [Route("DeleteSettingBykey")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> DeleteSettingBykey(string key)
        {
            try
            {
                var del = await _settingService.RemoveByKeyAsync(key);
                if (del.IsSuccess == true)
                {
                    return Ok(ResourceFa.DeleteSuccess);
                }
                else
                {
                    return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ResourceFa.DeleteForChildError, del.childs) });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Code = ResourceFa.DeleteCode.code, msg = string.Format(ex.Message) });
            }
        }
        #endregion Setting

 

    }

}
