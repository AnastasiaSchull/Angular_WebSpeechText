using Microsoft.EntityFrameworkCore;

namespace Angular_WebSpeechText.Models
{
    public class UserContext : DbContext
    {      
        public DbSet<User> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
