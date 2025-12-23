using Microsoft.EntityFrameworkCore;

namespace DigitalBankLite.API.Models
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(s => s.CustomerId);

            modelBuilder.Entity<Account>()
          .Property(a => a.Balance)
          .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.BalanceAfterTxn)
                .HasPrecision(18, 2);

            // ? Recommended: limit string sizes (avoids huge TEXT everywhere)
            modelBuilder.Entity<Customer>().Property(x => x.Email).HasMaxLength(256);
            modelBuilder.Entity<Customer>().Property(x => x.Phone).HasMaxLength(20);
            modelBuilder.Entity<Customer>().Property(x => x.Role).HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(x => x.KycStatus).HasMaxLength(50);
        }
    }
}