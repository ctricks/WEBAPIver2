using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.Users;

namespace WEBAPI.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        TokenResponse Refresh(TokenRequest model);
        TokenResponse RefreshAdmin(TokenRequest model);
    }

    public class TokenService : ITokenService
    {
        private readonly AppSettings _appSettings;
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public TokenService(IOptions<AppSettings> appSettings, DataContext context,IConfiguration config)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _config = config;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            double expireMinute = 0;

            if(double.TryParse(_appSettings.MinuteExpire.ToString(),out expireMinute)) { expireMinute = double.Parse(_appSettings.MinuteExpire.ToString()); }

            if (!string.IsNullOrEmpty(_appSettings.MinuteExpire))
                expireMinute = Double.Parse(_appSettings.MinuteExpire);

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                //For JWT Configuration JSON
                issuer: _config["CS:Issuer"],
                audience: _config["CS:Audience"], 
                claims: claims,
                expires: DateTime.Now.AddMinutes(expireMinute),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["CS:key"]);

            var tokenValidationParameters = new TokenValidationParameters()
            { 
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = _config["CS:Issuer"],
                ValidAudience = _config["CS:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key) // The same key as the one that generate the token
            };


            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public TokenResponse Refresh(TokenRequest model)
        {
            if (model is null)
                throw new AppException("Invalid Model. Please check your request");

            string accessToken = model.AccessToken;
            string refreshToken = model.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identities.FirstOrDefault().Claims.FirstOrDefault().Value; //this is mapped to the Name claim by default

            var user = _context.Users.Where(x => x.PhoneNumber == username && x.TokenID == accessToken).FirstOrDefault();

            if (user == null)
                throw new AppException("Invalid User. Please check your user to refresh token");
            
            var newAccessToken = GenerateAccessToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            //Save ID's on DB for next validation
            user.TokenID = newAccessToken;
            user.RefreshToken = newRefreshToken;
            
            _context.SaveChanges();

            TokenResponse response = new TokenResponse { 
                AccessToken = newAccessToken, 
                RefreshToken = newRefreshToken,  
                ExpireDate= user.RefreshTokenExpiryTime,
                UserId = user.Id,
                UserRole = user.Role,
                PhoneNumber = user.PhoneNumber
            };

            return response;
        }

        public TokenResponse RefreshAdmin(TokenRequest model)
        {
            if (model is null)
                throw new AppException("Invalid Model. Please check your request");

            string accessToken = model.AccessToken;
            string refreshToken = model.RefreshToken;

            var principal = GetPrincipalFromExpiredToken(accessToken);

            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = _context.UserAdmins.Where(x => x.UserName == username && x.TokenID == accessToken).FirstOrDefault();

            if (user == null)
                throw new AppException("Invalid User. Please check your user to refresh token");

            var newAccessToken = GenerateAccessToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            //Save ID's on DB for next validation
            user.TokenID = newAccessToken;
            user.RefreshToken = newRefreshToken;

            _context.SaveChanges();

            TokenResponse response = new TokenResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken, ExpireDate = user.RefreshTokenExpiryTime };

            return response;
        }


    }
}
