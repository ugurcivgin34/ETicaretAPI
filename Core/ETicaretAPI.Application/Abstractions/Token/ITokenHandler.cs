using ETicaretAPI.Application.DTOs;

namespace ETicaretAPI.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        TokenDto CreateAccessToken(int second);
        string CreateRefreshToken();
    }
}
