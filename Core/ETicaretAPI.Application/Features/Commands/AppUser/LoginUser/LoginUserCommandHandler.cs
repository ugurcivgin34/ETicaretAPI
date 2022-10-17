﻿using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identy.AppUser> _userManager;
        private readonly SignInManager<Domain.Entities.Identy.AppUser> _signInManager;

        public LoginUserCommandHandler(UserManager<Domain.Entities.Identy.AppUser> userManager, SignInManager<Domain.Entities.Identy.AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Identy.AppUser user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);

            if (user == null)
                throw new NotFoundUserException();


            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);//kullanıcı şifresini yanlış girerse locklayım mı sorusuna hayır diyip false yaptık.

            if (result.Succeeded) //Authentication başarılı!
            {
                //..... Yetkileri belirlememiz gerekiyor!
            }
            return new();
        }
    }
}
