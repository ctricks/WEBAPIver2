using Microsoft.AspNetCore.Mvc;
using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.FightMatch;
using WEBAPI.Models.Users;

namespace WEBAPI.Services
{
    public interface IFightMatchService
    {
        public IEnumerable<FightMatch> GetAll();
        public FightMatch GetFightMatchById(int id);
        public FightMatch OpenMatch(DateTime FightDate);

        public FightMatch GetLastFightMatchByDate(DateTime FightDate);
        public string Register(FightMatchRequest model);
        public void UpdateFightStatus(FightMatchRequest model);
        public void UpdateFightResult(FightMatchRequest model);
        void Update(int id, FightMatchRequest model);
        void Delete(int id);



    }
    public class FightMatchService : IFightMatchService
    {
        private DataContext _context;
        
        public FightMatchService(
            DataContext context
            )
        {
            _context = context;            
        }

        public IEnumerable<FightMatch> GetAll()
        {
            return _context.FightMatches;
        }

        public FightMatch GetFightMatchById(int id)
        {
            return getFightMatchById(id);
        }

        public FightMatch GetLastFightMatchByDate(DateTime FightDate)
        {
            var FightMatch = _context.FightMatches.Where(x => x.MatchDate.Date == FightDate.Date).OrderBy(o=>o.MatchDate).LastOrDefault();

            return FightMatch;
        }

        //CB-10182023 Open Match From Admin affected FightMatchConfig
        public FightMatch OpenMatch(DateTime FightDate)
        {
            string Message = "ok";
            // validate match number in fightmatchconfig CB-10132023 Check Date if fightmatchconfig is already created
            var fightmatchconfig = _context.FightMatchConfigs.Where(x => x.MatchDate.Year == FightDate.Year
                            && x.MatchDate.Month == FightDate.Month
                            && x.MatchDate.Day == FightDate.Day);

            if(fightmatchconfig.ToList().Count == 1)
            {
                int matchNumber = fightmatchconfig.FirstOrDefault().MatchCurrentNumber;

                FightMatch fightMatch = new FightMatch() {
                    MatchDate = FightDate,
                    MatchNumber = matchNumber + 1,
                    MatchResultId = 0,
                    MatchStatusId = 0,
                };

                return fightMatch;
            }else
            {
                throw new AppException("No Fight Date found on Match Config. Please check");
            }

        }


        public string Register(FightMatchRequest model)
        {
            // validate
            if (_context.FightMatches.Any(x => x.MatchDate == model.FightDate && x.MatchNumber == model.MatchNumber))
            {
                //throw new AppException("Fight Number: '" + model.MatchNumber + "' is already exists");
                return "Error: Fight Number: '" + model.MatchNumber + "' is already exists";
            }

            // validate match number in fightmatchconfig CB-10132023 Check Date if fightmatchconfig is already created
            var fightmatchconfig = _context.FightMatchConfigs.Where(x=>x.MatchDate.Year == model.FightDate.Year
                            && x.MatchDate.Month == model.FightDate.Month
                            && x.MatchDate.Day == model.FightDate.Day);

            if (fightmatchconfig.ToList().Count() == 0)
            {
                //throw new KeyNotFoundException("No FightMatch Number found for today. Please start the Fight Match first");
                return "Error: No FightMatch Number found for today. Please start the Fight Match first";
            }

            // map model to new user object
            //var useradmin = _mapper.Map<UserAdmin>(model);

            // hash password
            //useradmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            FightMatch match = new FightMatch
            {
                MatchDate = model.FightDate,
                MatchNumber = model.MatchNumber,
                MatchStatusId = (int)model.MatchStatusId,
                MatchResultId = (int)model.MatchResultId,
            };


            // save user
            _context.FightMatches.Add(match);

            _context.SaveChanges();
            
            return "Ok";
        }

        public void UpdateFightStatus(FightMatchRequest model)
        {
            //CB-09302023 Get User via id
            var fight = getFightMatch(model);
            fight.MatchStatusId = (int)model.MatchStatusId;

            _context.FightMatches.Update(fight);
            _context.SaveChanges();
        }
        public void UpdateFightResult(FightMatchRequest model)
        {
            //CB-09302023 Get User via id
            var fight = getFightMatch(model);
            fight.MatchResultId = (int)model.MatchResultId;

            _context.FightMatches.Update(fight);
            _context.SaveChanges();
        }

        public void Update(int id, FightMatchRequest model)
        {
            var fightmatch = getFightMatch(model);
            if (fightmatch == null) throw new KeyNotFoundException("Fight not found");
            _context.FightMatches.Update((FightMatch)fightmatch);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var fight = _context.FightMatches.Find(id);
            if (fight == null) throw new KeyNotFoundException("Fight not found");

            _context.FightMatches.Remove(fight);
            _context.SaveChanges();
        }
        private FightMatch getFightMatch(FightMatchRequest model)
        {
            var fight = _context.FightMatches.Where(x=>x.MatchDate == model.FightDate
             && x.MatchNumber == model.MatchNumber);

            if (fight == null) throw new KeyNotFoundException("Fight not found");

            return (FightMatch)fight;
        }
        private FightMatch getFightMatchById(int id)
        {
            var fight = _context.FightMatches.Find(id);

            if (fight == null) throw new KeyNotFoundException("Fight not found");

            return fight;
        }
    }
}
