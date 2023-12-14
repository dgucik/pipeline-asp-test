using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantAPI.IntegrationTests.Helpers;

namespace RestaurantAPI.IntegrationTests
{
	public class RestaurantControllerTests : 
		IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly HttpClient _client;

		public RestaurantControllerTests(WebApplicationFactory<Program> factory)
        {
			_client = factory
				.WithWebHostBuilder(builder =>
				{
					builder.ConfigureServices(services =>
					{
						var dbContextOptions = services
							.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

						services.Remove(dbContextOptions);

						services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

						services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

						services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
					});
				})
				.CreateClient();
		}

		[Fact]
		public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
		{
			var model = new CreateRestaurantDto()
			{
				Name = "TestRestaurant",
				City = "Kraków",
				Street = "Długa 5",
				Category = "FastFood",
				PostalCode = "32-551",
				Description = "Description",
				ContactEmail = "Jan@NowakR.pl",
				ContactNumber = "712-324-242"
			};

			var httpContent = model.ToJsonHttpContent();

			var response = await _client.PostAsync("/api/restaurant", httpContent);

			response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
			response.Headers.Location.Should().NotBeNull();
		}

		[Fact]
		public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
		{
			var model = new CreateRestaurantDto()
			{
				Name = "TestRestaurant",
				City = "Kraków",
				Street = "Długa 5",
			};

			var httpContent = model.ToJsonHttpContent();

			var response = await _client.PostAsync("/api/restaurant", httpContent);

			response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task GetAll_WithQueryParameters_ReturnsOkResult()
		{
			var response = await _client.GetAsync("/api/restaurant?PageNumber=1&PageSize=5");

			response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
		}
	}
}