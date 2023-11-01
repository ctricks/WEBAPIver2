using Microsoft.EntityFrameworkCore.Query.Internal;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.WalletTransactions;

namespace WEBAPI.Services
{
    public interface IWalletTransactionsService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<WalletTxn> GetAll();
        WalletTxn GetById(int id);
        AuthenticateResponse newUserWallet(AuthenticateRequest model);
        void Create(RegisterRequest model);
        //void Update(int id, UpdateRequest model);
        void Delete(int id);
    }

    public class WalletTransactionsService : IWalletTransactionsService
    {
        private DataContext _context;        

        public WalletTransactionsService(
            DataContext context)
        {
            _context = context;            
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            //CB-09302023 Check UserCreds via TokenID, if user has null token in database... you need to login first
            var user = _context.Users.Any(x => x.TokenID == model.TokenID);

            // validate if found
            if (user == false)
                throw new AppException("User is not login. Please login again");

            AuthenticateResponse AuthRes = new AuthenticateResponse()
            {
                Message = "Transaction is Valid"
            };

            var response = AuthRes;

            //response.Message = "Wallet Transaction successfully granted";

            return response;
        }

        public IEnumerable<WalletTxn> GetAll()
        {
            return _context.WalletTxns;
        }

        public WalletTxn GetById(int id)
        {
            return getWallet(id);
        }
        public AuthenticateResponse newUserWallet(AuthenticateRequest model)
        {
            return newuserwallet(model);
        }


        public void Create(RegisterRequest model)
        {
            //CB-09292023 user TokenID 
            //Validate tokenID via user
            if (!_context.UserAdmins.Any(x => x.TokenID == model.UserTokenID))
                throw new AppException("TokenID is not found. Please login first");

            // save wallet Transactions
            WalletTxn wallettrans = new WalletTxn();

            wallettrans.amount = model.available_balance;
            wallettrans.account_bal = model.total_balance;            
            wallettrans.TransactionType = model.TransactionType;
            

            var userinfo = getUsersViaToken(model.UserTokenID);

            if (userinfo == null)
                throw new AppException("Token is invalid. Please login");

            var UserWallet = getUWallet(userinfo.UserName);

            if (userinfo == null)
            {
                //CB-09302023 Update the Userwallet then the wallet transaction as Additional
                UserWallet userwallet = new UserWallet();
                userwallet.WalletTrans = new List<WalletTxn>();
                userwallet.WalletTrans.Add(wallettrans);
                userwallet.available_balance = model.available_balance;
                userwallet.total_balance = model.total_balance;
                userwallet.UserId = userinfo.Id;
                userwallet.UserName = userinfo.UserName;

                _context.UserWallet.Add(userwallet);
            }
            else
            {
                //CB-10012023 If Record exists, it will update the wallet balance using userid
                wallettrans.UserIDRef = userinfo.Id;

                //CB-09302023 Update the Userwallet then the wallet transaction as Additional
                UserWallet userwallet = getUWallet(userinfo.UserName);

                userwallet.WalletTrans = new List<WalletTxn>();
                userwallet.WalletTrans.Add(wallettrans);
                userwallet.available_balance = model.available_balance;
                userwallet.total_balance = model.total_balance;
                userwallet.UserId = userinfo.Id;
                userwallet.UpdateDate = DateTime.Now;

                _context.UserWallet.Update(userwallet);
            }

            _context.SaveChanges();
        }

        //public void Update(int id, UpdateRequest model)
        //{
            //var user = getUser(id);

            //// validate
            //if (model.Username != user.UserName && _context.Users.Any(x => x.UserName == model.Username))
            //    throw new AppException("Username '" + model.Username + "' is already taken");

            //// hash password if it was entered
            //if (!string.IsNullOrEmpty(model.Password))
            //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            //// copy model to user and save
            //_mapper.Map(model, user);
            //_context.Users.Update(user);
            //_context.SaveChanges();
        //}

        public void Delete(int id)
        {
            var wallettxn = getWallet(id);
            _context.WalletTxns.Remove(wallettxn);
            _context.SaveChanges();
        }

        // helper methods
        // for query wallet transaction via id
        private WalletTxn getWallet(int id)
        {
            var wallettxn = _context.WalletTxns.Where(x=>x.UserIDRef == id).FirstOrDefault();
            if (wallettxn == null) throw new KeyNotFoundException("Wallet Transaction not found");
            return wallettxn;
        }
        private AuthenticateResponse newuserwallet(AuthenticateRequest model)
        {
            var userinfo = getUsersViaToken(model.TokenID);

            if (userinfo == null) throw new KeyNotFoundException("User Token is invalid. Please check first");


            var wallettxn = _context.UserWallet.Where(x => x.UserId == userinfo.Id).FirstOrDefault();

            AuthenticateResponse response = new AuthenticateResponse();

            if (wallettxn == null)
            {
                UserWallet newWallet = new UserWallet()
                {
                    UserId = userinfo.Id,
                    available_balance = 0,
                    total_balance = 0
                };
                wallettxn = newWallet;
                _context.UserWallet.Add(newWallet);
                _context.SaveChanges();
                response.Message = "New user wallet successfully created";
                response.TotalBalance = newWallet.total_balance;
                response.AvailableBalance = newWallet.available_balance;                
            }
            else
            {
                response.Message = "User wallet already has account";
                response.TotalBalance = wallettxn.total_balance;
                response.AvailableBalance = wallettxn.available_balance;
            }
            
            
            

            return response;
        }
        private UserWallet getUWallet(string Username)
        {
            var userwallet = _context.UserWallet.Where(x=>x.UserName == Username).FirstOrDefault();
            if (userwallet == null) throw new KeyNotFoundException("User Wallet not found");
            return userwallet;
        }
        private UserAdmin getUsersViaToken(string TokenID)
        {
            var userwallet = _context.UserAdmins.Where(x=>x.TokenID == TokenID).FirstOrDefault();
            if (userwallet == null) throw new KeyNotFoundException("User not found");
            return userwallet;
        }
    }
}
