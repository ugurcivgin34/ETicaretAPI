using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Domain.Entities.Identy;

namespace ETicaretAPI.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        TokenDto CreateAccessToken(int second,AppUser user);
        string CreateRefreshToken();
    }
}
