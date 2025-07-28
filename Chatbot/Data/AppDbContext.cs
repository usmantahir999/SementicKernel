using Microsoft.EntityFrameworkCore;
namespace Chatbot.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessage>().HasIndex(c => c.SessionId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
