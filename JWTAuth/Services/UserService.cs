using BCrypt.Net;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.Users;

namespace WEBAPI.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<UserAdmin> GetAll();
        IEnumerable<UserAdmin> GetById(int id);
        void Register(RegisterRequest model);
        void Update(int id, UpdateRequest model);
        void Delete(int id);
        void Logout(string TokenId);
        void LogoutPhone(string PhoneNumber);
        void updateUserToken(string Username,string TokenId, string UserToken,DateTime MinuteExpire);
    }

    public class UserService : IUserService
    {
        private DataContext _context;        
        private IConfiguration _config;        


        public UserService(
            DataContext context,            
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            //CB-10212023 Change to all user will be added
            //var user = _context.Users.Where(x => x.PhoneNumber == model.PhoneNumber).FirstOrDefault();

            var user = _context.UserAdmins.Where(x => x.UserName == model.PhoneNumber).FirstOrDefault();
            
            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("PhoneNumber is incorrect");

            // authentication successful
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["CS:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("id", user.Id.ToString())
                }),
                IssuedAt = DateTime.UtcNow,
                Issuer = _config["CS:Issuer"],
                Audience = _config["CS:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["AppSettings:MinuteExpire"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.TokenID = tokenHandler.WriteToken(token);

            _context.UserAdmins.Update(user).Property(x=>x.Id).IsModified = false;            
            
            //_context.Users.Update(user);
            _context.SaveChanges(true);

            AuthenticateResponse response = new AuthenticateResponse
            {
                Id = user.Id,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Token = user.TokenID,
                Role = user.Role
            };

            return response;
        }

        public void updateUserToken(string PhoneNumber,string TokenId,string RefreshTokenId,DateTime MinuteExpire)
        {
            var user = _context.UserAdmins.SingleOrDefault(x => x.PhoneNumber == PhoneNumber);
            if(user != null)
            {
                user.TokenID = TokenId;
                user.RefreshToken = RefreshTokenId;

                user.RefreshTokenExpiryTime = MinuteExpire;

                _context.UserAdmins.Update(user).Property(x=>x.Id).IsModified = false;

                _context.SaveChanges();
            }
        }
       
        public IEnumerable<UserAdmin> GetAll()
        {
            return _context.UserAdmins;
        }

        public IEnumerable<UserAdmin> GetById(int id)
        {
            var userinfo = _context.UserAdmins.Where(x=>x.Id == id).FirstOrDefault();

            if(userinfo == null)
                throw new AppException("No User found. Please check your ID");

            List<UserAdmin> Userinfo = new List<UserAdmin>();

            Userinfo.Add(userinfo);

            return Userinfo;
        }

        public void Register(RegisterRequest model)
        {
            // validate
            if (_context.UserAdmins.Any(x => x.UserName == model.PhoneNumber))
                throw new AppException("Phone Number '" + model.PhoneNumber+ "' is already taken");

            // map model to new user object
            var user = new UserAdmin()
            {
                PhoneNumber = model.PhoneNumber,
                UserName = model.PhoneNumber,
                Role = model.Role.ToLower()
            }; 

            // hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            
            // save ewallet            

            List<UserWallet>wallet = new List<UserWallet>();

            UserWallet uwallet = new UserWallet { available_balance = 0,total_balance=0, UserName = user.UserName};
            
            wallet.Add(uwallet);

            user.UWallet = wallet;
           
            // save user
            _context.UserAdmins.Add(user);
            
            _context.SaveChanges();



        }

        public void Logout(string TokenId)
        {
            //CB-09302023 Get User via id
            var user = _context.UserAdmins.Where(x => x.TokenID == TokenId).FirstOrDefault();
            user.TokenID = null;
            user.RefreshToken = null;
            _context.UserAdmins.Update(user);
            _context.SaveChanges();
        }

        public void LogoutPhone(string Username)
        {
            //CB-09302023 Get User via id
            var user = _context.UserAdmins.Where(x => x.UserName == Username).FirstOrDefault();
            user.TokenID = null;
            user.RefreshToken = null;
            _context.UserAdmins.Update(user);
            _context.SaveChanges();
        }

        public void Update(int id, UpdateRequest model)
        {
            var user = _context.UserAdmins.Find(id);

            // validate
            if (model.PhoneNumber != user.PhoneNumber && _context.Users.Any(x => x.PhoneNumber == model.PhoneNumber))
                throw new AppException("Phone NUmber '" + model.PhoneNumber + "' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            //update user object
            user.Id = id;
            user.UserName = model.PhoneNumber;
            user.PhoneNumber = model.PhoneNumber;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);


            _context.UserAdmins.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.UserAdmins.Find(id);
            _context.UserAdmins.Remove(user);
            _context.SaveChanges();
        }

        // helper methods
        // CB-10042023 make it as class function for calling validation
        //private User getUser(int id)
        //{
        //    var user = _context.Users.Find(id);
        //    if (user == null) throw new KeyNotFoundException("User not found");
        //    return user;
        //}


    }
}
