using AWSLab03.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AWSLab03.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //These DbSet properties represent the tables in the database
        //It is used by Entity Framework to perform CRUD operations
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Episode> Episodes { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
