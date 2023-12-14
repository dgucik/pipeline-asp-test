using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Linq.Expressions;

namespace RestaurantAPI.Services
{
	public interface IRestaurantService
	{
		int CreateRestaurant(CreateRestaurantDto dto);
		PagedResult<RestaurantDto> GetAllRestaurants(RestaurantQuery query);
		RestaurantDto GetRestaurantById(int id);
	}

	public interface IRestaurantService1
	{
		int CreateRestaurant(CreateRestaurantDto dto);
		IEnumerable<RestaurantDto> GetAllRestaurants();
		RestaurantDto GetRestaurantById(int id);
	}

	public class RestaurantService : IRestaurantService
	{
		private RestaurantDbContext _dbContext;
		private IMapper _mapper;
		private ILogger<RestaurantService> _logger;

		public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_logger = logger;
		}

		public PagedResult<RestaurantDto> GetAllRestaurants(RestaurantQuery query)
		{
			_logger.LogInformation("Get ALL Invoked");

			var baseQuery = _dbContext.Restaurants
				.Include(r => r.Dishes)
				.Include(r => r.Address)
				.Where(r => (query.SearchPhrase == null) || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
							|| r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

			if (!string.IsNullOrEmpty(query.SortBy))
			{
				var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>()
				{
					{nameof(Restaurant.Name), r => r.Name },
					{nameof(Restaurant.Description), r => r.Description },
					{nameof(Restaurant.Category), r => r.Category }
				};

				var selectedColumn = columnsSelectors[query.SortBy];

				baseQuery = query.SortDirection == SortDirection.ASC ? 
					baseQuery.OrderBy(selectedColumn)
					: baseQuery.OrderBy(selectedColumn);
			}

			var restaurants = baseQuery
				.Skip(query.PageSize * (query.PageNumber - 1))
				.Take(query.PageSize)
				.ToList();

			var totalItemsCount = baseQuery.Count();

			var dto = _mapper.Map<List<RestaurantDto>>(restaurants);

			var result = new PagedResult<RestaurantDto>(dto, baseQuery.Count(),query.PageSize, query.PageNumber);

			return result;
		}

		public RestaurantDto GetRestaurantById(int id)
		{
			var restaurant = _dbContext.Restaurants
				.FirstOrDefault(r => r.Id == id);

			if (restaurant == null)
			{
				throw new NotFoundException($"Restaurant with id: {id} doesn't exist");
			}

			var dto = _mapper.Map<RestaurantDto>(restaurant);
			return dto;
		}

		public int CreateRestaurant(CreateRestaurantDto dto)
		{
			var restaurant = _mapper.Map<Restaurant>(dto);
			_dbContext.Restaurants.Add(restaurant);
			_dbContext.SaveChanges();
			return restaurant.Id;
		}
	}
}
