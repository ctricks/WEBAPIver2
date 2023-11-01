using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.BetTransaction;

namespace WEBAPI.Services
{
    public interface IBetTransactionService
    {
        void Create(BetTransactionRequest model);
        public IEnumerable<UserBetTxn> GetAll();

        public List<BetTransactionByFightNumberResponse> GetBetUserByFightNumber(int FightNumber, DateTime MatchDate);

        public IEnumerable<BetTransactionByColorsResponse> GetBetColorByFightNumber(int fightNumber, DateTime FightDate);
    }

    public class BetTransactionService : IBetTransactionService
    {
        private DataContext _context;        

        public BetTransactionService(
            DataContext context)
        {
            _context = context;            
        }
        public void Create(BetTransactionRequest model)
        {
            //CB-10072023 Validate userID if exists
            var user = _context.UserAdmins.Where(x => x.Id == model.UserId).FirstOrDefault();
            if (user == null) throw new AppException("User is invalid. Please use your valid account");

            //CB-10072023 Get Color id from config
            var betcolor = _context.BetColorConfigs.Where(
                    x => x.ColorName.ToUpper() == model.BetColorName
                ).FirstOrDefault();
            if (betcolor == null) throw new AppException("Color not found. Please check your entry");

            //CB-10072023 Get Match via matchnumber
            var fightmatch = _context.FightMatches.Where(
                    x => x.MatchNumber == model.FightMatchNumber && 
                    x.MatchDate >= model.FightDate.Date && x.MatchDate < model.FightDate.Date.AddDays(1)
                    ).FirstOrDefault();
            if (fightmatch == null) throw new AppException("Fight match not found. Please check your entry");

            //CB-10072023 Get WalletAvailable Balance to check if possible to bet
            var wallet = _context.UserWallet.Where(x => x.UserId == model.UserId && x.available_balance > 0).FirstOrDefault();
            if (wallet == null) throw new AppException("Insufficient balance in wallet. Total Available: 0.00");

            double walletBalane = (double)wallet.available_balance;

            if (walletBalane < model.BetAmount) throw new AppException("Insufficient balance in wallet. Total Available: " + walletBalane);

            //CB-10072023 Validate FightMatchId if current status must be Open
            var userbet = _context.UserBetTxns.Where(
                    x => x.UserId == model.UserId &&
                    x.BetColorId == betcolor.Id &&
                    x.BetDate >= model.FightDate.Date &&
                    x.FightMatchId == fightmatch.Id
                ).FirstOrDefault();

            List<UserWallet> MyWallet = new List<UserWallet>();
            List<WalletTxn> WalletTxt = new List<WalletTxn>();

            WalletTxn wallettrans = new WalletTxn()
            {
                TransactionType = "BET",
                account_bal = walletBalane - model.BetAmount,
                amount = model.BetAmount,
                UserIDRef = model.UserId
            };

            WalletTxt.Add(wallettrans);

            UserWallet uwallet = new UserWallet()
            {
                available_balance = walletBalane - model.BetAmount,
                UserId = model.UserId,
                total_balance = walletBalane,
                WalletTrans = WalletTxt                
            };

            MyWallet.Add(uwallet);

            if (userbet == null)
            {
                UserBetTxn bettrans = new UserBetTxn() {
                    FightMatchId = fightmatch.Id,
                    BetDate = model.FightDate,
                    BetColorId = betcolor.Id,
                    BetAmount = model.BetAmount,
                    UserId = model.UserId,
                    UWallet = MyWallet
                };
                
                _context.UserBetTxns.Add(bettrans);
                
            }
            else
            {
                double totalAmountBet = userbet.BetAmount + model.BetAmount;

                userbet.BetAmount = totalAmountBet;

                userbet.UWallet = MyWallet;


                _context.UserBetTxns.Update(userbet);
            }

            //if (DeductWallet(model.BetAmount, model.UserId))
            //{
                _context.SaveChanges();
            //}
        }

        public IEnumerable<UserBetTxn> GetAll()
        {
            return _context.UserBetTxns;
        }

        public List<BetTransactionByFightNumberResponse> GetBetUserByFightNumber(int FightNumber,DateTime MatchDate)
        {
            List<BetTransactionByFightNumberResponse> ResponseList = new List<BetTransactionByFightNumberResponse>();

            var fightmatch = _context.FightMatches.Where(
                    x => x.MatchNumber == FightNumber &&
                    x.MatchDate >= MatchDate.Date && x.MatchDate < MatchDate.Date.AddDays(1)
                    ).FirstOrDefault();
            if (fightmatch == null) throw new AppException("Fight match not found. Please check your entry");

            var UserBetMatches = _context.UserBetTxns.Where(x => x.FightMatchId == fightmatch.Id);
            var ColorConfig = _context.BetColorConfigs.ToList();


            foreach (UserBetTxn UserBetMatch in UserBetMatches)
            {
                BetTransactionByFightNumberResponse response = new BetTransactionByFightNumberResponse();

                response.UserId = UserBetMatch.UserId;
                response.FightMatchNumber = FightNumber;
                response.FightDate = MatchDate;
                response.BetAmount = UserBetMatch.BetAmount;
                response.BetColorName = ColorConfig.Where(x => x.Id == UserBetMatch.BetColorId).FirstOrDefault().ColorName.ToString();

                ResponseList.Add(response);
            }            
            return ResponseList;
        }
        
        public IEnumerable<BetTransactionByColorsResponse> GetBetColorByFightNumber(int FightNumber, DateTime FightDate)
        {
            List<BetTransactionByColorsResponse> ResponseList = new List<BetTransactionByColorsResponse>();

            var fightmatch = _context.FightMatches.Where(
                    x => x.MatchNumber == FightNumber &&
                    x.MatchDate >= FightDate.Date && x.MatchDate < FightDate.Date.AddDays(1)
                    ).FirstOrDefault();
            if (fightmatch == null) throw new AppException("Fight match not found. Please check your entry");

            
            var ColorConfig = _context.BetColorConfigs.ToList();

            foreach (BetColorConfigs colorBet in ColorConfig)
            {
                var UserBetMatches = _context.UserBetTxns.Where(x => x.FightMatchId == fightmatch.Id && x.BetColorId == colorBet.Id);
                double SumTotal =+ UserBetMatches.Where(x => x.BetColorId == colorBet.Id).FirstOrDefault().BetAmount;

                foreach (UserBetTxn UserBetMatch in UserBetMatches)
                {
                    BetTransactionByColorsResponse response = new BetTransactionByColorsResponse();

                    response.FightNumber = FightNumber;
                    response.FightDate = FightDate;
                    response.TotalAmount = SumTotal;
                    response.ColorName = colorBet.ColorName;

                    ResponseList.Add(response);
                }
            }
            return ResponseList;
        }
        private bool DeductWallet(double Amount,int userId)
        {
            bool result = false;
            try
            {
                var wallet = _context.UserWallet.Where(x => x.UserId == userId && x.available_balance > 0 && x.available_balance >= Amount).FirstOrDefault();
                if (wallet != null)
                {
                    double availablebalance = (double)wallet.available_balance;
                    availablebalance -= Amount;
                    wallet.available_balance = availablebalance;

                    List<WalletTxn>myWallet  = new List<WalletTxn>();

                    WalletTxn wallettxn = new WalletTxn()
                    {
                        amount = Amount,
                        account_bal = availablebalance,
                        UserIDRef = userId,
                    };

                    myWallet.Add(wallettxn);

                    wallet.WalletTrans = myWallet;

                    _context.Add(wallet);
                    
                    _context.SaveChanges();


                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
    }
}
