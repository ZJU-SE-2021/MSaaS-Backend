using Microsoft.EntityFrameworkCore;

namespace MsaasBackend.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

    }
}