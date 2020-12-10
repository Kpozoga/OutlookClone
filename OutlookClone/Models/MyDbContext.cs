using Microsoft.EntityFrameworkCore;

namespace OutlookClone.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
        public DbSet<MailModel> Mails { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<GroupModel> Groups { get; set; }
    }
}
