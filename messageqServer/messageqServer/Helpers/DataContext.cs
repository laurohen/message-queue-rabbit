using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using messageqServer.Entities;

namespace messageqServer.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<EventProcess> Events { get; set; }

        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }
    }
}