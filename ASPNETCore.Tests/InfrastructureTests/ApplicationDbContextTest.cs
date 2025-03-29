using ASPNETCore.Domain.Entities;
using ASPNETCore.Infrastructure.Data;
using ASPNETCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ASPNETCore.Tests.InfrastructureTests
{
    public class ApplicationDbContextTest
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ApplicationDbContextTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;
        }

        [Fact]
        public async Task AddAsync_ShouldSaveObject()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new REObjectRepository(context);
                var obj = new REObject { Id = 1, Street = "Test Street", Price = 500000 };

                await repository.AddAsync(obj);
            }

            using (var context = new ApplicationDbContext(_options))
            {
                var obj = await context.Objects.FirstOrDefaultAsync(o => o.Id == 1);
                Assert.NotNull(obj);
                Assert.Equal("Test Street", obj.Street);
            }
        }
    }
}
