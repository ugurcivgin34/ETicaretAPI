using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Authencations
{

    //Dış kaynak operasyonlarını yöneten
    public interface IExternalAuthentication
    {
        Task<DTOs.TokenDto> FacebookLoginAsync(string authToken,int accessTokenLifeTime);
        Task<DTOs.TokenDto> GoogleLoginAsync(string idToken, int accessTokenLifeTime);
    }
}
