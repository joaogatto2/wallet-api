using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wallet.Core.Entities;

namespace Wallet.Infrastructure.Context
{
    public class WalletDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Deposit> Deposits { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        private IConfiguration configuration { get; }

        public WalletDbContext(DbContextOptions<WalletDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder
            .UseNpgsql(connectionString)
            .UseSeeding((context, _) =>
            {
                var dbSet = context.Set<User>();
                var users = dbSet.Take(1);
                
                if (users.Count() < 1)
                {
                    var usersSeed = new List<User>()
                    {
                        new ()
                        {
                            Balance = 100,
                            Name = "Seeded 1",
                            Deposits = new List<Deposit>
                            {
                                new Deposit
                                {
                                    Date = DateTime.UtcNow,
                                    Value = 100
                                }
                            }
                        },
                        new ()
                        {
                            Balance = 100,
                            Name = "Seeded 2",
                            Deposits = new List<Deposit>
                            {
                                new Deposit
                                {
                                    Date = DateTime.UtcNow,
                                    Value = 100
                                }
                            }
                        }
                    };
                    dbSet.AddRange(usersSeed);
                    context.SaveChanges();

                    context.Set<Transfer>()
                        .AddRange(new List<Transfer>
                        {
                            new ()
                            {
                                Date = DateTime.UtcNow,
                                ByUserId = usersSeed[0].Id,
                                ToUserId = usersSeed[1].Id,
                                Value = 100
                            },
                            new ()
                            {
                                Date = DateTime.UtcNow,
                                ByUserId = usersSeed[1].Id,
                                ToUserId = usersSeed[0].Id,
                                Value = 100
                            },
                        });
                    context.SaveChanges();
                }
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Deposits)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.TransfersOut)
                .WithOne(x => x.ByUser)
                .HasForeignKey(x => x.ByUserId);

            modelBuilder.Entity<User>()
                .HasMany(x => x.TransfersIn)
                .WithOne(x => x.ToUser)
                .HasForeignKey(x => x.ToUserId);

            modelBuilder.Entity<User>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transfer>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Transfer>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Deposit>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Deposit>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();

        } 
    }
}