using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identy;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Domain.Entities.Identy.AppUser> _userManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly SignInManager<Domain.Entities.Identy.AppUser> _signInManager;



        public AuthService(HttpClient httpClient, IConfiguration configuration, UserManager<Domain.Entities.Identy.AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory, SignInManager<AppUser> signInManager)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient(); //HttpClient direk IoC den çekilemediği için factroy u kullandık 
            _signInManager = signInManager;
        }

        async Task<TokenDto> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo info, int accessTokenLifeTime)
        {

            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = email
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }
            if (result)
            {
                await _userManager.AddLoginAsync(user, info);

                TokenDto token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                return token;
            }
            throw new Exception("Invalid external authentication");
        }



        public async Task<TokenDto> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["ExternalLoginSettings:Facebook:Client_ID"]}&client_secret={_configuration["ExternalLoginSettings:Facebook:Client_Secret"]}&grant_type=client_credentials");

            FacebookAccessTokenResponseDto? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponseDto>(accessTokenResponse);

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

            FacebookUserAccessTokenValidationDto? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidationDto>(userAccessTokenValidation);

            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");

                FacebookUserInfoResponseDto? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponseDto>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");
                Domain.Entities.Identy.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, info, accessTokenLifeTime);

            }
            throw new Exception("Invalid external authentication.");
        }


        public async Task<TokenDto> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:Client_ID"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings); //client’tan gelen ‘idToken’ bilgisi eşliğinde bu token’ın payload’ı ayrıştırılmaktadır. 

            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");//satırda ise dış kaynaktan gelen kullanıcı bilgilerini ‘AspNetUserLogins’ tablosuna kaydetmemizi sağlayacak olan bir ‘UserLoginInfo’ nesnesi oluşturulmaktadır.

            Domain.Entities.Identy.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);//FindByLoginAsync fonksiyonu ile ‘AspNetUserLogins’ tablosunda ‘UserLoginInfo’ nesnesindeki bilgilere karşılık bir kayıt olup olmadığı kontrol edilmekte ve kayıt varsa eğer giriş yaptırılmaktadır. Nihayetinde burada kullanıcı önceden aynı dış kaynaktan geldiyse uygulama tarafından tanınması ve direkt giriş yaptırılması gerekmektedir.

            //bool result = user != null;
            //if (user == null)
            //{
            //    user = await _userManager.FindByEmailAsync(payload.Email);
            //    if (user == null)
            //    {
            //        user = new()
            //        {
            //            Id = Guid.NewGuid().ToString(),
            //            Email = payload.Email,
            //            UserName = payload.Email,
            //            NameSurname = payload.Name
            //        };
            //        var identityResult = await _userManager.CreateAsync(user);
            //        result = identityResult.Succeeded;
            //    }
            //}//aralığında ise dış kaynaktan gelen kullanıcının önceden gelmediğine dair ‘AspNetUserLogins’ tablosunda bir kaydın olmaması durumu göz önüne alınarak, önce bu kullanıcı email’inde bir kullanıcı olup olmadığı değerlendirilmekte, yoksa bu kullanıcının kaydı gerçekleştirilmektedir. 



            //if (result)
            //    await _userManager.AddLoginAsync(user, info); //AddLoginAsync fonksiyonu ile dış kaynaktan giriş yapan kullanıcının bilgileri eğer ‘AspNetUserLogins’ tablosunda yoksa işlenmektedir.
            //else
            //    throw new Exception("Invalid external auyhentication");
            //TokenDto token = _tokenHandler.CreateAccessToken(accessTokenLifeTime); // Google’dan gelen kullanıcı doğrulandıysa yetkilendirmeyi sağlayacak manevra gerçekleştirilmektedir. Tabi biz burada JWT üretiyor ve gönderiyoruz 

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);
        }

        public async Task<TokenDto> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            Domain.Entities.Identy.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);//kullanıcı şifresini yanlış girerse locklayım mı sorusuna hayır 
            if (result.Succeeded) //Authentication başarılı!
            {
                TokenDto token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                return token;
            }
            throw new AuthenticationErrrorException();
        }
    }
}
