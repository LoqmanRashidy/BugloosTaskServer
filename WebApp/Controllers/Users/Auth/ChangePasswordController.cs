using System.Threading.Tasks;
using CommonLayer;
using Datalayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Users;

namespace WebApp.Controllers.Users
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    //[AllowAnonymous]
    //[IgnoreAntiforgeryToken]
    [EnableCors("BuglossPolicy")]
    public class ChangePasswordController : Controller
    {
        private readonly IUsersService _usersService;
        public ChangePasswordController(IUsersService usersService)
        {
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([FromBody]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _usersService.GetCurrentUserAsync();
            if (user == null)
            {
                return BadRequest("NotFound");
            }

            var result = await _usersService.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Error);
        }
    }
}