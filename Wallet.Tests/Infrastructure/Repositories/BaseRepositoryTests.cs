using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Moq;
using Wallet.Infrastructure.Context;
using Wallet.Infrastructure.Repositories;

namespace Wallet.Tests.Infrastructure.Repositories
{
    public class BaseRepositoryTests
    {
        public class TestDbContext : WalletDbContext
        {
            public TestDbContext(DbContextOptions<WalletDbContext> options, IConfiguration configuration)
                : base(options, configuration) { }

            public DbSet<TestEntity> TestEntities => Set<TestEntity>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                return;
            }
        }
        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
        private readonly TestDbContext _context;
        private readonly BaseRepository<TestEntity> _repository;

        public BaseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<WalletDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique db for isolation
                .Options;

            var configurationData = new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", "Server=localhost;Database=WalletDb;User Id=sa;Password=Your_password123;" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationData)
                .Build();
            _context = new TestDbContext(options, configuration);
            _context.Database.EnsureCreated();
            _repository = new BaseRepository<TestEntity>(_context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            _context.Set<TestEntity>().AddRange(
                new TestEntity { Id = 1, Name = "Test1" },
                new TestEntity { Id = 2, Name = "Test2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "Test" };
            _context.Set<TestEntity>().Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result?.Name);
        }

        [Fact]
        public async Task FindAsync_ReturnsFilteredEntities()
        {
            // Arrange
            _context.Set<TestEntity>().AddRange(
                new TestEntity { Id = 1, Name = "A" },
                new TestEntity { Id = 2, Name = "B" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(e => e.Name == "A");

            // Assert
            Assert.Single(result);
            Assert.Equal("A", result.First().Name);
        }

        [Fact]
        public async Task AddAsync_AddsEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "New" };

            // Act
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var all = await _context.Set<TestEntity>().ToListAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal("New", all[0].Name);
        }

        [Fact]
        public async Task Update_UpdatesEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "Old" };
            _context.Set<TestEntity>().Add(entity);
            await _context.SaveChangesAsync();

            // Act
            entity.Name = "Updated";
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            var updated = await _context.Set<TestEntity>().FindAsync(1);

            // Assert
            Assert.Equal("Updated", updated?.Name);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = 1, Name = "ToDelete" };
            _context.Set<TestEntity>().Add(entity);
            await _context.SaveChangesAsync();

            // Act
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();

            var deleted = await _context.Set<TestEntity>().FindAsync(1);

            // Assert
            Assert.Null(deleted);
        }
    }
}