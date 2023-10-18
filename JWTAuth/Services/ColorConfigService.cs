using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.BetColor;

namespace WEBAPI.Services
{
    public interface IColorConfigService
    {
        IEnumerable<BetColorConfigs> GetAll();

        void SetDefault();
        BetColorConfigs GetById(int id);
        void Create(UpdateRequest model);
        void Update(UpdateRequest model);
        void Delete(int id,string TokenId);
    }
    public class ColorConfigService : IColorConfigService
    {
        private DataContext _context;
        
        public ColorConfigService(
            DataContext context)
        {
            _context = context;                      
        }
        public void Create(UpdateRequest model)
        {
            //CB-10042023 Validate if exist value
            if (_context.BetColorConfigs.Any(x => x.ColorName == model.ColorName))
                throw new AppException("Status is already exists. Please check your entry");

            _context.BetColorConfigs.Add(new BetColorConfigs() { ColorName = model.ColorName });
            _context.SaveChanges();
        }

        public IEnumerable<BetColorConfigs> GetAll()
        {
            return _context.BetColorConfigs;
        }

        public void SetDefault()
        {
            //CB-10042023 For migration uses Truncate Table and call this for default values
            //Check if exists
            string[] ColorValues = new string[] { "RED", "WHITE" };

            object? ColorValue;

            foreach (string colorVal in ColorValues)
            {
                ColorValue = _context.BetColorConfigs.Where(x => x.ColorName == colorVal).FirstOrDefault();
                if (ColorValue == null)
                    _context.BetColorConfigs.Add(new BetColorConfigs { ColorName = colorVal });
            }

            _context.SaveChanges();

        }


        public BetColorConfigs GetById(int id)
        {
            return getMatchStatus(id);
        }

        public void Update(UpdateRequest model)
        {
            var betColor = getMatchStatus(model.ColorId);

            if (betColor == null)
                throw new AppException("Status not exists. Please check your entry");

            // copy model to user and save
            //_mapper.Map(model, betColor);

            betColor.ColorName = model.ColorName;

            _context.BetColorConfigs.Update(betColor);
            _context.SaveChanges();
        }

        public void Delete(int id,string TokenId)
        {
            var betColor = getMatchStatus(id);

            if (betColor == null)
                throw new AppException("Status not exists. Please check your entry");

            //Check for admin tokenID
            var user = getUserAdmin(TokenId);

            if (user == null)
                throw new AppException("User is invalid. Please use administrator account");

            _context.BetColorConfigs.Remove(betColor);
            _context.SaveChanges();
        }
        private BetColorConfigs getMatchStatus(int id)
        {
            var betColor = _context.BetColorConfigs.Find(id);
            if (betColor == null) throw new KeyNotFoundException("Bet Color not found");
            return betColor;
        }
        private User getUser(string TokenId)
        {
            var user = _context.Users.Where(x => x.TokenID == TokenId).FirstOrDefault();            
            return user;
        }
        private UserAdmin getUserAdmin(string TokenId)
        {
            var user = _context.UserAdmins.Where(x => x.TokenID == TokenId).FirstOrDefault();               
            return user;
        }
    }
}
