using AccountingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } //модель пользовтаеля
        public DbSet<Account> Accounts { get; set; } //модель счета
        public DbSet<CurrencyRate> CurrencyRates { get; set; } //модель курса
        public DbSet<Transaction> Transactions { get; set; } //модель перевода

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}