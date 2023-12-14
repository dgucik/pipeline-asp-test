using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class RestaurantController : ControllerBase
	{
		private IRestaurantService _service;

		public RestaurantController(IRestaurantService service) 
		{
			_service = service;
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult<PagedResult<RestaurantDto>> Get([FromQuery] RestaurantQuery query) 
		{ 
			var restaurants = _service.GetAllRestaurants(query);
			return Ok(restaurants);
		}

		[HttpPost]
		public ActionResult Create([FromBody]CreateRestaurantDto dto)
		{
			var id = _service.CreateRestaurant(dto);
			return Created($"api/[controller]/{id}", null);
		}

		[HttpGet("{id}")]
		public ActionResult<RestaurantDto> Get([FromRoute]int id)
		{
			var restaurant = _service.GetRestaurantById(id);
			return Ok(restaurant);
		}
	}
}
