using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
	public class RestaurantSeeder
	{
		private RestaurantDbContext _dbContext;

		public RestaurantSeeder(RestaurantDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public void Seed()
		{
			if (_dbContext.Database.CanConnect())
			{
				if(_dbContext.Database.IsRelational()) { 
					var pendingMigrations = _dbContext.Database.GetPendingMigrations();
					if(pendingMigrations != null && pendingMigrations.Any()) 
					{
						_dbContext.Database.Migrate();
					}
				}

				if (!_dbContext.Restaurants.Any())
				{
					var restaurants = GetRestaurants();
					_dbContext.AddRange(restaurants);
					_dbContext.SaveChanges();
				}

				if (!_dbContext.Users.Any())
				{
					var roles = GetRoles();
					_dbContext.AddRange(roles);
					_dbContext.SaveChanges();
				}
			}
		}

		private IEnumerable<Role> GetRoles()
		{
			return new List<Role>()
			{
				new Role()
				{
					Name = "User"
				},
				new Role()
				{
					Name = "Manager"
				},
				new Role()
				{
					Name = "Admin"
				}
			};
		}

		private IEnumerable<Restaurant> GetRestaurants()
		{
			var restaurants = new List<Restaurant>();
			for(int i = 0; i < 50; i++)
			{
				restaurants.Add(new Restaurant()
				{
					Name = $"KFC {i}",
					Category = "Fast Food",
					Description = "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant",
					ContactEmail = "contact@kfc.com",
					HasDelivery = true,
					Dishes = new List<Dish>()
					{
						new Dish()
						{
							Name = "Nashville Hot Chicken",
							Price = 10.30M
						},

						new Dish()
						{
							Name = "Chicken Nuggets",
							Price = 5.30M
						}
					},
					Address = new Address()
					{
						City = $"Kraków {i}",
						Street = "Długa 5",
						PostalCode = "30-001"
					}
				});
			}
			return restaurants;
		}
	}
}
