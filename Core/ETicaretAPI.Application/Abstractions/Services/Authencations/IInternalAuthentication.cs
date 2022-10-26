using ETicaretAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Authencations
{
    //İç kaynak operasyonlarını yöneten 
    public interface IInternalAuthentication
    {
        Task<TokenDto> LoginAsync(string userNameOrEmail,string password,int accessTokenLifeTime);
        Task<TokenDto> RefreshTokenLoginAsync(string refreshToken);
    }
}
