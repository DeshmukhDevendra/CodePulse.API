using Microsoft.AspNetCore.Identity;

namespace CodePulse.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        public string CreateJwtToken(IdentityUser identityUser, List<string> roles);
    }
}
