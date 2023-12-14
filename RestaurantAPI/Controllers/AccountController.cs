using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private IAccountService _service;

		public AccountController(IAccountService service)
        {
			_service = service;

		}

		[HttpPost("Register")]
		public ActionResult Register([FromBody]RegisterUserDto dto)
		{
			var userId = _service.RegisterUser(dto);
			return Created($"api/[controller]/{userId}", null);
		}

		[HttpPost("Login")]
		public ActionResult<string> Login(LoginDto dto)
		{
			var token = _service.GenerateJwt(dto);
			return Ok(token);
		}
    }
}
