using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Onyx.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Onyx.Repositories
{
    public class UserAuthRepository : IAuthRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserAuthRepository(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public string CreateJWT(IdentityUser user, List<string> roles)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Email, user.Email));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RepositoryResult<string>> LoginUser(LoginRequestDto loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
                return new RepositoryResult<string>
                {
                    Data = $"User [{loginRequest.Username}] does not exist",
                    Success = false,
                    Message = "$User [{loginRequest.Username}] does not exist"
                };

            var passwordResult = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!passwordResult)
                return new RepositoryResult<string>
                {
                    Data = "Incorrect password",
                    Success = passwordResult,
                    Message = "Incorrect password"
                };

            var roles = await _userManager.GetRolesAsync(user);

            if (roles == null)
                return new RepositoryResult<string>
                {
                    Data = "User has no roles",
                    Success = false,
                    Message = $"No roles are assigned to user {user.UserName}"
                };

            var token = CreateJWT(user, roles.ToList());

            return new RepositoryResult<string>
            {
                Data = token,
                Success = true,
                Message = ""
            };
        }

        public async Task<RepositoryResult<string>> RegisterUser(RegisterRequestDto registerRequest)
        {
            var newIdentityUser = new IdentityUser
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email
            };

            if (registerRequest.Roles == null || registerRequest.Roles.Count() == 0)
                return new RepositoryResult<string>
                {
                    Data = "No roles specified",
                    Success = false,
                    Message = "No roles specified"
                };

            var createUserResult = await _userManager.CreateAsync(newIdentityUser, registerRequest.Password);

            if (!createUserResult.Succeeded)
                return new RepositoryResult<string>
                {
                    Data = $"Failed to create user [{registerRequest.Username}]",
                    Success = false,
                    Message = _identityErrorToString(createUserResult.Errors)
                };

            var roleAssignmentResult = await _userManager.AddToRolesAsync(newIdentityUser, registerRequest.Roles);

            if (!roleAssignmentResult.Succeeded)
                return new RepositoryResult<string>
                {
                    Data = $"Failed to assign role to user [{registerRequest.Username}]",
                    Success = false,
                    Message = _identityErrorToString(roleAssignmentResult.Errors)
                };

            return new RepositoryResult<string>
            {
                Data = $"New user [{registerRequest.Username}] created",
                Success = true,
                Message = ""
            };
        }

        private string _identityErrorToString(IEnumerable<IdentityError> errors)
        {
            var result = "";

            if (errors == null)
                return result;

            foreach (var error in errors)
                result += $"{error.Code}, {error.Description}\n";

            return result;
        }
    }
}
