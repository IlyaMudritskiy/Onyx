using Microsoft.AspNetCore.Identity;
using Onyx.Models.DTOs;

namespace Onyx.Repositories
{
    public interface IAuthRepository
    {
        Task<RepositoryResult<string>> RegisterUser(RegisterRequestDto registerRequest);
        Task<RepositoryResult<string>> LoginUser(LoginRequestDto loginRequest);
        string CreateJWT(IdentityUser user, List<string> roles);
    }
}
