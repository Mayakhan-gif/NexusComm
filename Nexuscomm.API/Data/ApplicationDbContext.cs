using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nexuscomm.API.Models;

namespace Nexuscomm.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CommunicationMessage> CommunicationMessages { get; set; }
        public DbSet<CommunicationAttempt> CommunicationAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ----- CommunicationMessage -----
            builder.Entity<CommunicationMessage>(entity =>
            {
                entity.Property(e => e.Channel)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.CommunicationMessages)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.Status, e.ScheduledAt });
            });

            // ----- CommunicationAttempt -----
            builder.Entity<CommunicationAttempt>(entity =>
            {
                entity.Property(e => e.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(e => e.CommunicationMessage)
                      .WithMany(m => m.Attempts)
                      .HasForeignKey(e => e.CommunicationMessageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}