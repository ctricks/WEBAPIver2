using WEBAPI.Entities;
using WEBAPI.Helpers;
using WEBAPI.Models.FightMatchConfig;

namespace WEBAPI.Services
{
    public interface IFightMatchConfigService
    {
        public IEnumerable<FightMatchConfig> GetAll();
        public FightMatchConfig GetFightMatchById(int id);
        public void Start(FightMatchConfigRequest model);

        public void End(DateTime MatchDate);

        public void Delete(int id);

    }
    public class FightMatchConfigService : IFightMatchConfigService
    {
            private DataContext _context;

            public FightMatchConfigService(
                DataContext context
                )
            {
                _context = context;
            }

            public IEnumerable<FightMatchConfig> GetAll()
            {
                return _context.FightMatchConfigs;
            }

            public FightMatchConfig GetFightMatchById(int id)
            {
                return _context.FightMatchConfigs.Find(id);
            }

            public void Start(FightMatchConfigRequest model)
            {
                // validate
                if (_context.FightMatchConfigs.Any(x => x.MatchDate == model.FightMatchDate && x.MatchCurrentNumber == model.MatchNumber))
                    throw new AppException("Fight Current Number: '" + model.MatchNumber + "' is already exists");

                // validate match number in fightmatchconfig CB-10132023 Check Date if fightmatchconfig is already created
                var fightmatchconfig = _context.FightMatchConfigs.Where(x => x.MatchDate.Year == model.FightMatchDate.Year
                                && x.MatchDate.Month == model.FightMatchDate.Month
                                && x.MatchDate.Day == model.FightMatchDate.Day).FirstOrDefault();

                if (fightmatchconfig == null)
                {
                //throw new AppException("No FightMatch Date in Config already created. Please use Fight Match Module");

                    FightMatchConfig matchconfig = new FightMatchConfig
                    {
                        MatchDate = model.FightMatchDate,
                        MatchCurrentNumber = 1,
                        MatchTotalNumber = 999,
                        CreatedDate = DateTime.Now
                    };
                // save user
                _context.FightMatchConfigs.Add(matchconfig);

                _context.SaveChanges();
                }
            }
        public void End(DateTime FightMatchDate)
        {
            // validate match number in fightmatchconfig CB-10132023 Check Date if fightmatchconfig is already created
            var fightmatchconfig = _context.FightMatchConfigs.Where(x => x.MatchDate.Year == FightMatchDate.Year
                            && x.MatchDate.Month == FightMatchDate.Month
                            && x.MatchDate.Day == FightMatchDate.Day).FirstOrDefault();

            if (fightmatchconfig != null)
            {
                int matchNumber = fightmatchconfig.MatchCurrentNumber + 1;
                // save user
                fightmatchconfig.MatchCurrentNumber = matchNumber;

                _context.FightMatchConfigs.Update(fightmatchconfig);

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
            {
                var fight = _context.FightMatches.Find(id);
                if (fight == null) throw new KeyNotFoundException("Fight not found");

                _context.FightMatches.Remove(fight);
                _context.SaveChanges();
            }
                    
    }
}
