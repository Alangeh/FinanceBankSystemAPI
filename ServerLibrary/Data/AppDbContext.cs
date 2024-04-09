using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        #region Base Library Entities to be added to DB
        
        // Authentication Tables
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

        // Transation Tables
        public DbSet<Bank> Banks { get; set; }
        public DbSet<UserTransactionAccount> UserAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        #endregion
    }
}
