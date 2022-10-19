using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identy.AppUser> _userManager;
        private readonly ITokenHandler _tokenHandler;

        public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identy.AppUser> userManager, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { "454442637002-7ai8q0cniq5v1tinfahmr6570rvqi044.apps.googleusercontent.com" }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings); //client’tan gelen ‘idToken’ bilgisi eşliğinde bu token’ın payload’ı ayrıştırılmaktadır. 

            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);//satırda ise dış kaynaktan gelen kullanıcı bilgilerini ‘AspNetUserLogins’ tablosuna kaydetmemizi sağlayacak olan bir ‘UserLoginInfo’ nesnesi oluşturulmaktadır.

            Domain.Entities.Identy.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);//FindByLoginAsync fonksiyonu ile ‘AspNetUserLogins’ tablosunda ‘UserLoginInfo’ nesnesindeki bilgilere karşılık bir kayıt olup olmadığı kontrol edilmekte ve kayıt varsa eğer giriş yaptırılmaktadır. Nihayetinde burada kullanıcı önceden aynı dış kaynaktan geldiyse uygulama tarafından tanınması ve direkt giriş yaptırılması gerekmektedir.

            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }//aralığında ise dış kaynaktan gelen kullanıcının önceden gelmediğine dair ‘AspNetUserLogins’ tablosunda bir kaydın olmaması durumu göz önüne alınarak, önce bu kullanıcı email’inde bir kullanıcı olup olmadığı değerlendirilmekte, yoksa bu kullanıcının kaydı gerçekleştirilmektedir. 



            if (result)
                await _userManager.AddLoginAsync(user, info); //AddLoginAsync fonksiyonu ile dış kaynaktan giriş yapan kullanıcının bilgileri eğer ‘AspNetUserLogins’ tablosunda yoksa işlenmektedir.
            else
                throw new Exception("Invalid external auyhentication");
            TokenDto token = _tokenHandler.CreateAccessToken(5); // Google’dan gelen kullanıcı doğrulandıysa yetkilendirmeyi sağlayacak manevra gerçekleştirilmektedir. Tabi biz burada JWT üretiyor ve gönderiyoruz 

            return new()
            {
                Token = token
            };
        }
    }
}
