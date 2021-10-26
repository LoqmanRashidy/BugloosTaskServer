using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLayer;
using CommonLayer.Custom;
using CommonLayer.Extension;
using Datalayer.Models;
using Datalayer.Models.Users;
using Datalayer.ViewModels;
using DataLayer.Context;
using DataLayer.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceLayer.BaseSystem;
using ServiceLayer.Users;
using static CommonLayer.EnumExtensions;

namespace WebApp.Controllers.BaseSystem
{
    [ApiController]
    [Route("api/[controller]")]
    [IgnoreAntiforgeryToken]
    [EnableCors("BuglossPolicy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = CustomPolicy.AdminTeacherStudent)]
    public partial class PersonController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IOptionsSnapshot<UploadDownloadRoot> _configuration;
        private readonly IOptionsSnapshot<ApiSettings> _configurationSetting;
        private readonly IPersonService _personService;
        private readonly IUsersService _usersService;
        private readonly IRolesService _rolesService;
        private readonly IServiceScopeFactory _scopeFactory;
        public PersonController(
            IHttpContextAccessor httpContextAccessor,
            IPersonService personService,
            IUsersService usersService,
            IRolesService rolesService,
            IOptionsSnapshot<UploadDownloadRoot> configuration,
            IServiceScopeFactory scopeFactory,
            IOptionsSnapshot<ApiSettings> configurationSetting)
        {
            _scopeFactory = scopeFactory;
            _personService = personService;
            _personService.CheckArgumentIsNull(nameof(personService));

            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));

            _rolesService = rolesService;
            _rolesService.CheckArgumentIsNull(nameof(rolesService));


            _configuration = configuration;
            _configuration.CheckArgumentIsNull(nameof(configuration));

            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor.CheckArgumentIsNull(nameof(httpContextAccessor));

            _configurationSetting = configurationSetting;
            _configurationSetting.CheckArgumentIsNull(nameof(configurationSetting));

        }



        #region Person


        [HttpGet]
        [Route("GetAllPerson")]
        [ProducesResponseType(typeof(List<Person>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPerson()
        {
            try
            {
                var data = await _personService.FindAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, error = true });
            }
        }


        [HttpGet]
        [Route("GetPerson")]
        [ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPerson(long id)
        {
            try
            {
                var data = await _personService.FindByIdAsync(id);
                var data1 = _personService.JsonSingle(data);
                return Ok(data1);
            }
            catch (Exception ex)
            {

                return BadRequest(new { msg = ex.Message, error = true });
            }
        }


        [Route("AddEditWithUploadPerson")]
        [ProducesResponseType(typeof(Person), (int)HttpStatusCode.OK)]
        [HttpPost]
        public async Task<IActionResult> AddEditWithUploadPerson()
        {

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
                    {
                        try
                        {
                            var httpRequest = HttpContext.Request;
                            var _person = httpRequest.Form["Person"];
                            StringValues _type = httpRequest.Form["Type"];
                            Person person = JsonConvert.DeserializeObject<Person>(_person);
                            Nullable<Guid> systemFileName = null;
                            List<FileType> fileType = new List<FileType>();
                            string url = _configuration.Value.PersonRoot;
                            string filePath = null;
                            var uploads = Path.Combine(Directory.GetCurrentDirectory(), url);
                            if (!Directory.Exists(uploads))
                            {
                                Directory.CreateDirectory(uploads);
                            }
                            if (_type.Count > 0)
                            {
                                foreach (var item in _type)
                                {
                                    fileType.Add(JsonConvert.DeserializeObject<FileType>(item));
                                }
                            }
                            IFormFileCollection files = null;

                            long personId = person.Id;

                            if (_httpContextAccessor.HttpContext.Request.Form.Files.Count > 0)
                            {
                                files = _httpContextAccessor.HttpContext.Request.Form.Files;
                                int i = 0;

                                if (files != null)
                                {
                                    foreach (var file in files)
                                    {
                                        if (file.Length > 0)
                                        {
                                            var fileName = Path.GetFileName(file.FileName);
                                            FileType item = fileType.Where(x => x.Name == fileName && x.IsRead == false).FirstOrDefault();
                                            var itemIndex = fileType.FindIndex(x => x.Name == fileName && x.Type == item.Type && x.IsRead == false);
                                            item.IsRead = true;
                                            fileType[itemIndex] = item;
                                            if (personId > 0)
                                            {
                                                systemFileName = person.FileId;
                                                filePath = Path.Combine(uploads, systemFileName.ToString() + Path.GetExtension(person.FileName));
                                                var fileInfo = new System.IO.FileInfo(filePath);
                                                fileInfo.Delete();
                                                person.FileName = file.FileName;
                                                person.Ext = Path.GetExtension(file.FileName);

                                            }
                                            else
                                            {
                                                systemFileName = Guid.NewGuid();
                                                person.FileId = systemFileName.Value;
                                                person.FileName = file.FileName;
                                                person.Ext = Path.GetExtension(file.FileName);


                                            }
                                            
                                            filePath = Path.Combine(uploads, systemFileName.ToString() + Path.GetExtension(file.FileName));
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await file.CopyToAsync(fileStream);
                                            }
                                        }
                                    }

                                }

                            }

                            await _personService.AddOrUpdateAsync(person, context);

                            if (personId<0)
                            {
                                User agoUser = await _usersService.FindUserByUserNameAsync(person.Mobile, context);
                                
                                if (agoUser == null)
                                {
                                   
                                    User user = new User();
                                    user.PersonId = person.Id;
                                    user.Username = person.Mobile;
                                    user.Name = person.Name;
                                    user.LastName = person.LastName;
                                    user.IsActive = true;
                                    user.IsSystem = true;
                                    user.LastLoggedIn = null;
                                    user.Mobile = person.Mobile;
                                    user.Password = person.Password;
                                    Role role = _rolesService.FindConditionAsync(q => q.EnTitle == CustomRoles.Student, context).Result.FirstOrDefault();
                                    var userrole = new UserRole { RoleId = role.Id, UserId = user.Id };
                                    user.UserRoles.Add(userrole);
                                    await _usersService.AddOrUpdateAsync(user, context);
                                }
                                else
                                    return BadRequest(new { msg = ResourceFa.ErrorUserDuplicated.message, error = true, code = ResourceFa.ErrorUserDuplicated.code });

                            }
                          

                            transaction.Commit();
                            return Ok(person);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            if (ex.HResult == -2146233088)
                                return BadRequest(new { msg = ResourceFa.ErrorSameTransactionAgoRuning.message, error = true, code = ResourceFa.ErrorSameTransactionAgoRuning.code });
                            else
                                return BadRequest(new { msg = ex.Message, error = true, code = ex.HResult });
                        }
                    }
                }
            }
        }

        [HttpDelete]
        [Route("DeletePerson")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> DeletePerson(long id)
        {
            try
            {
                var del = await _personService.RemoveAsync(id);
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

        #endregion Person

    }

}
