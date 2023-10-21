using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models;
using WEBAPI.Models.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WEBAPI.Services
{
    public interface IUserAdminService
    {
        AdminAuthenticateResponse Authenticate(AdminAuthenticateRequest model);
        IEnumerable<UserAdmin> GetAll();

        UserAdmin GetByBearerToken(string Username);
        TokenInfoResponse CheckTokenByBearerToken(string Username);
        UserAdmin GetById(int id);

        UserAdmin GetUserByToken(string Token);

        UserAdmin GetUserByTokenValidation(string Token);
        void Register(AdminRegisterRequest model);
        void Update(int id, AdminUpdateRequest model);
        void Delete(int id);
        void Logout(int id);
        void updateUserToken(string Username, string TokenId, string UserToken, DateTime MinuteExpire);
    }

    public class UserAdminService : IUserAdminService
    {
        private DataContext _context;        
        private IConfiguration _config;

        public UserAdminService(
            DataContext context,            
            IConfiguration config)
        {
            _context = context;            
            _config = config;
        }

        public AdminAuthenticateResponse Authenticate(AdminAuthenticateRequest model)
        {
            var useradmin = _context.UserAdmins.SingleOrDefault(x => x.UserName == model.Username);

            // validate
            if (useradmin == null || !BCrypt.Net.BCrypt.Verify(model.Password, useradmin.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["CS:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, useradmin.UserName),
                    new Claim(ClaimTypes.Role, useradmin.Role),
                    new Claim("id", useradmin.Id.ToString())
                }),
                IssuedAt = DateTime.UtcNow,
                Issuer = _config["CS:Issuer"],
                Audience = _config["CS:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["AppSettings:MinuteExpire"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            useradmin.TokenID = tokenHandler.WriteToken(token);

            _context.UserAdmins.Update(useradmin).Property(x => x.Id).IsModified = false;
            
            _context.SaveChanges();

            AdminAuthenticateResponse response = new AdminAuthenticateResponse {
                Id = useradmin.Id,
                Username = model.Username,
                Token = useradmin.TokenID,
                Role = useradmin.Role,
                ExpireToken = useradmin.RefreshTokenExpiryTime
                };

            return response;
        }

        public void updateUserToken(string Username, string TokenId, string RefreshTokenId, DateTime MinuteExpire)
        {
            var user = _context.UserAdmins.SingleOrDefault(x => x.UserName == Username);
            if (user != null)
            {
                user.TokenID = TokenId;
                user.RefreshToken = RefreshTokenId;

                user.RefreshTokenExpiryTime = MinuteExpire;

                _context.UserAdmins.Update(user).Property(x => x.Id).IsModified = false;

                _context.SaveChanges();
            }
        }

        public IEnumerable<UserAdmin> GetAll()
        {
            return _context.UserAdmins;
        }

        public UserAdmin GetByBearerToken(string Username)
        {            
            var user = _context.UserAdmins.Where(x=>x.UserName == Username).FirstOrDefault();
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;        
        }

        public TokenInfoResponse CheckTokenByBearerToken(string Username)
        {
            var user = _context.UserAdmins.Where(x => x.UserName == Username).FirstOrDefault();
            if (user == null) throw new KeyNotFoundException("User not found");
            bool isExpire = false;

            DateTime TokenDate = Convert.ToDateTime(user.RefreshTokenExpiryTime);
            DateTime TodayDate = Convert.ToDateTime(DateTime.Now.ToString());

            isExpire = ValidateToken(user.TokenID);

            TokenInfoResponse response = new TokenInfoResponse()
            {
                AccessToken = user.TokenID,
                RefreshToken = user.RefreshToken,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                UserRole = user.Role,
                UserId = user.Id,
                ExpireDate = user.RefreshTokenExpiryTime,
                TodaysDate = TodayDate,
                isExpired = isExpire            
            };

            return response;
        }

        public TokenValidationParameters GetValidationParameters()
        {
            var key = Encoding.ASCII.GetBytes(_config["CS:Key"]);
            return new TokenValidationParameters()
            {   
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = _config["CS:Issuer"],
                ValidAudience = _config["CS:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key) // The same key as the one that generate the token
            };
        }

        private bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);


            return CheckTokenIsValid(authToken);
        }

        public static long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ticks;
        }

        public static bool CheckTokenIsValid(string token)
        {
            var tokenTicks = GetTokenExpirationTime(token);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            var now = DateTime.Now.ToUniversalTime();

            var valid = tokenDate >= now;

            return valid;
        }


        public UserAdmin GetUserByToken(string Token)
        {
            var user = _context.UserAdmins.Where(x => x.TokenID == Token).FirstOrDefault();
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        public UserAdmin GetUserByTokenValidation(string Token)
        {
            var user = _context.UserAdmins.Where(x => x.TokenID == Token).FirstOrDefault();            
            return user;
        }

        public UserAdmin GetById(int id)
        {
            return getUser(id);
        }

        public void Register(AdminRegisterRequest model)
        {
            // validate
            if (_context.UserAdmins.Any(x => x.UserName == model.UserName))
                throw new AppException("Username '" + model.UserName + "' is already taken");

            // map model to new user object
            //var useradmin = _mapper.Map<UserAdmin>(model);

            // hash password
            //useradmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            UserAdmin useradmin = new UserAdmin
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                UserName = model.UserName,
                Role = model.Role                
            };

            
            
            // save user
            _context.UserAdmins.Add(useradmin);

            _context.SaveChanges();
        }

        public void Logout(int id)
        {
            //CB-09302023 Get User via id
            var user = getUser(id);
            user.TokenID = null;
            _context.UserAdmins.Update(user);
            _context.SaveChanges();
        }

        public void Update(int id,AdminUpdateRequest model)
        {
            var useradmin = getUser(id);

            // validate
            if (model.Username != useradmin.UserName && _context.UserAdmins.Any(x => x.UserName == model.Username))
                throw new AppException("Username '" + model.Username + "' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                useradmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // copy model to user and save
            //_mapper.Map(model, user);

            useradmin.UserName = model.Username;
            useradmin.Role = model.Role;
            

            _context.UserAdmins.Update(useradmin);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = getUser(id);
            _context.UserAdmins.Remove(user);
            _context.SaveChanges();
        }

        // helper methods

        private UserAdmin getUser(int id)
        {
            var user = _context.UserAdmins.Find(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

    }
}
