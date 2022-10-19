using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Domain.Entities.Identy;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        private readonly UserManager<Domain.Entities.Identy.AppRole> _userManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly HttpClient _httpClient;

        public FacebookLoginCommandHandler(UserManager<AppRole> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient(); //HttpClienti üzerinden nesne oluşturduk
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/patch/pauth/aaccess_token?client_id=546631843676576&client_secret=d3438100dc962c8f34a765e7d7deef3c&grant_type=client_credentials");

            FacebookAccessTokenResponseDto facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponseDto>(accessTokenResponse);

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookAccessTokenResponse.AccessToken}");

           FacebookUserAccessTokenValidationDto validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidationDto>(userAccessTokenValidation);

            if (validation.Data.IsValid)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");

                FacebookUserInfoResponseDto facebookUserInfoResponse=JsonSerializer.Deserialize<FacebookUserInfoResponseDto>(userInfoResponse);
            }

            return new()
            {
                Token = token
            };
        }
    }
}
