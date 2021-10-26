using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLayer;
using CommonLayer.Custom;
using Datalayer.Models.Users;
using Datalayer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.Users;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [EnableCors("BuglossPolicy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = CustomRoles.Admin)]

    public class UsersController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUsersService _usersService;
        private readonly IRolesService _rolesService;

        public UsersController(
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment,
            IUsersService usersService,
            IRolesService rolesService)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor.CheckArgumentIsNull(nameof(httpContextAccessor));

            _hostingEnvironment = hostingEnvironment;
            _hostingEnvironment.CheckArgumentIsNull(nameof(hostingEnvironment));


            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _rolesService = rolesService;
            _rolesService.CheckArgumentIsNull(nameof(rolesService));

            

        }


        #region User

      

        [HttpGet]
        [Route("GetAllUser")]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var list = await _usersService.FindAllAsync();
                var data = _usersService.JsonList(list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpGet]
        [Route("GetTest")]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTest()
        {
            try
            {
                var data = await Task.Run(() =>  new[] { "test1", "test2"} );
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpGet]
        [Route("GetUser")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUser(long id)
        {
            try
            {
                var entity = await _usersService.FindByIdAsync(id);
                entity.Password = "";
                var data = _usersService.JsonSingle(entity);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpPost]
        [Route("AddEditUser")]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddEditUser([FromBody] User user)
        {
            try
            {               
                await _usersService.AddOrUpdateAsync(user);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ChangePasswordUser")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> ChangePasswordUser([FromBody] ChangePasswordByAdmin ChangePasswordByAdmin)
        {
            try
            {
                await _usersService.ChangePasswordUser(ChangePasswordByAdmin);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                var (IsSuccess, childs) = await _usersService.RemoveAsync(id);
                if (IsSuccess == true)
                {
                    return Ok(new { msg = ResourceFa.DeleteSuccess, error = false, delete = true });
                }
                else
                {
                    return BadRequest(new { msg = string.Format(ResourceFa.DeleteForChildError, childs), error = false, delete = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }
        }

        [HttpPost]
        [Route("UploadUserImage")]
        public async Task<IActionResult> UploadUserImage(List<IFormFile> files)
        {
            try
            {                
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"user-pic-upload", fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                            }
                        }
                    }
                    return Ok(new { msg = "files upload success", error = false });
                }
                return BadRequest(new { msg = "files is null to upload !", error = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        #endregion User

        #region Role

    
        [HttpGet]
        [Route("GetAllRole")]
        [ProducesResponseType(typeof(List<Role>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                var list = await _rolesService.FindAllAsync();
                var data = _rolesService.JsonList(list);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }

        }

        [HttpGet]
        [Route("GetRole")]
        [ProducesResponseType(typeof(Role), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRole(long id)
        {
            try
            {
                var entity = await _rolesService.FindByIdAsync(id);
                var data = _rolesService.JsonSingle(entity);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }
        }

        [HttpPost]
        [Route("AddEditRole")]
        [ProducesResponseType(typeof(Role), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddEditRole([FromBody] Role role)
        {
            try
            {
                await _rolesService.AddOrUpdateAsync(role);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteRole")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteRole([FromBody] long id)
        {
            try
            {
                var del = await _rolesService.RemoveAsync(id);
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

        #endregion Role
        
      

    }
}