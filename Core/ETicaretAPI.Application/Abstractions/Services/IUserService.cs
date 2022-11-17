using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Domain.Entities.Identy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<CreateUserResponseDto> CreateAsnc(CreateUserDto model);
        Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate); //Refresh token ilk başta null olarak atanacak, sonra işlem yapılacağı için update yapacağız.
        Task UpdatePasswordAsync(string userId, string resetToken, string newPassword);
    }
}
