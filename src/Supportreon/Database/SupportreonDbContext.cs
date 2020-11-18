using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Miru.Databases.EntityFramework;
using Supportreon.Domain;

namespace Supportreon.Database
{
    public class SupportreonDbContext : MiruDbContext
    {
        public SupportreonDbContext(
            DbContextOptions<SupportreonDbContext> options, 
            IEnumerable<IBeforeSaveHandler> handlers) : base(options, handlers)
        {
        }
        
        public DbSet<User> Users { get; set; } 
        public DbSet<Project> Projects { get; set; } 
        public DbSet<Donation> Donations { get; set; } 
        public DbSet<Category> Categories { get; set; } 
    }
}
