using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WEBAPI.Entities;

namespace WEBAPI.Helpers
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database            
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
            
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<WalletTxn> WalletTxns { get; set; }
        public DbSet<UserWallet> UserWallet { get; set; }
        public DbSet<FightMatch> FightMatches { get; set; }
        public DbSet<UserBetTxn> UserBetTxns { get; set; }
        public DbSet<BetUserReward> BetUserRewards { get; set; }
        public DbSet<MatchStatusConfig> MatchStatusConfigs { get; set; }
        public DbSet<BetOdd> BetOdds { get; set; }
        public DbSet<BetColorConfigs> BetColorConfigs { get; set; }
        public DbSet<MatchResultConfig>MatchResultConfigs { get; set; }
        public DbSet<FightMatchConfig> FightMatchConfigs { get; set; }
        public DbSet<UserAdmin>UserAdmins { get; set; }
    }
}
