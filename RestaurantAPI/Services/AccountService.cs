using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services
{
	public interface IAccountService
	{
		string GenerateJwt(LoginDto dto);
		int RegisterUser(RegisterUserDto dto);
	}

	public class AccountService : IAccountService
	{
		private RestaurantDbContext _dbContext;
		private IMapper _mapper;
		private IPasswordHasher<User> _passwordHasher;
		private AuthenticationSettings _authenticationSettings;

		public AccountService(RestaurantDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, 
			AuthenticationSettings authenticationSettings)
		{
			_dbContext = dbContext;
			_mapper = mapper;
			_passwordHasher = passwordHasher;
			_authenticationSettings = authenticationSettings;
		}

		public int RegisterUser(RegisterUserDto dto)
		{
			var newUser = new User()
			{
				Email = dto.Email,
				RoleId = dto.RoleId
			};

			var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
			newUser.PasswordHash = hashedPassword;

			_dbContext.Users.Add(newUser);
			_dbContext.SaveChanges();

			return newUser.Id;
		}

		public string GenerateJwt(LoginDto dto)
		{
			var user = _dbContext.Users
				.Include(u => u.Role)
				.FirstOrDefault(u => u.Email == dto.Email);
			if (user == null)
			{
				throw new Exception("Wrong creds");
			}

			var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
			if (result == PasswordVerificationResult.Failed)
			{
				throw new Exception("Wrong creds");
			}

			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Role, user.Role.Name)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
			var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

			var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
				_authenticationSettings.JwtIssuer,
				claims,
				expires: expires,
				signingCredentials: cred);

			var tokenHandler = new JwtSecurityTokenHandler();
			return tokenHandler.WriteToken(token);
		}
	}
}
