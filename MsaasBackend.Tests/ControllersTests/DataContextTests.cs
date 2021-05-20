using Microsoft.EntityFrameworkCore;
using MsaasBackend.Models;

namespace MsaasBackend.Tests.ControllersTests
{
    public abstract class DataContextTests
    {
        public DbContextOptions<DataContext> DbContextOptions { get; private set; }

        public DataContext DataContext { get; set; }

        protected DataContextTests(DbContextOptions<DataContext> options)
        {
            DbContextOptions = options;
            DataContext = new DataContext(options);
            DataContext.Database.EnsureCreated();
        }
    }

    public abstract class InMemoryDataContextTests : DataContextTests
    {
        private static readonly DbContextOptions<DataContext> _dbContextOptions =
            new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase("msaas").Options;

        protected InMemoryDataContextTests() : base(_dbContextOptions)
        {
        }
    }
}