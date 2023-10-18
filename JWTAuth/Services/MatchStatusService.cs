using System.Net;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.MatchStatus;

namespace WEBAPI.Services
{
    public interface IMatchStatusService
    {
        IEnumerable<MatchStatusConfig> GetAll();

        void SetDefault();
        MatchStatusConfig GetById(int id);
        void Create(UpdateRequest model);
        void Update(int id,UpdateRequest model);
        void Delete(int id);        
    }
    public class MatchStatusService : IMatchStatusService
    {
        private DataContext _context;
        
        public MatchStatusService(
            DataContext context
                )
        {
                _context = context;                
        }
        public void Create(UpdateRequest model)
        {
            //CB-10042023 Validate if exist value
            if (_context.MatchStatusConfigs.Any(x => x.Status == model.Status))
                throw new AppException("Status is already exists. Please check your entry");

            _context.MatchStatusConfigs.Add(new MatchStatusConfig() { Status = model.Status });
            _context.SaveChanges();
        }

        public IEnumerable<MatchStatusConfig> GetAll()
        {
            return _context.MatchStatusConfigs;
        }
        public void SetDefault()
        {
            //CB-10042023 For migration uses Truncate Table and call this for default values
            //Check if exists
            string[] StatusValues = new string[] {"OPEN","LAST CALL","CLOSED" };

            object ?matchStatus;

            foreach (string statVal in StatusValues)
            {
                matchStatus = _context.MatchStatusConfigs.Where(x => x.Status == statVal).FirstOrDefault();
                if (matchStatus == null)
                    _context.MatchStatusConfigs.Add(new MatchStatusConfig { Status = statVal });
            }

            _context.SaveChanges();

        }


        public MatchStatusConfig GetById(int id)
        {
            return getMatchStatus(id);
        }

        public void Update(int id, UpdateRequest model)
        {
            var matchStatus = getMatchStatus(id);

            if(matchStatus == null)
                throw new AppException("Status not exists. Please check your entry");


            // copy model to user and save
            matchStatus.Status = model.Status;

            _context.MatchStatusConfigs.Update(matchStatus);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var matchStatus = getMatchStatus(id);
            _context.MatchStatusConfigs.Remove(matchStatus);
            _context.SaveChanges();
        }
        private MatchStatusConfig getMatchStatus(int id)
        {
            var matchStatus = _context.MatchStatusConfigs.Find(id);
            if (matchStatus == null) throw new KeyNotFoundException("Match Status not found");
            return matchStatus;
        }
        private UserAdmin getUserAdmin(string TokenId)
        {
            var user = _context.UserAdmins.Where(x => x.TokenID == TokenId).FirstOrDefault();
            if (user == null) throw new KeyNotFoundException("Please use Admin Account.");
            return user;
        }
    }    
}
