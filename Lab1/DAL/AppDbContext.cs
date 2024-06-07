using Lab1.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Lab1.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FileMetadata> FileMetadata { get; set; }
        public DbSet<UserFilePermission> UserFilePermissions { get; set; }
        public DbSet<IPAddress> IPAddresses { get; set; }
        public DbSet<Operation> OperationHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=wpf;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<FileMetadata>()
                .HasMany(x => x.UserFilePermissions)
                .WithOne(x => x.FileMetadata)
                .HasForeignKey(x => x.FileMetadataId);

            builder.Entity<IPAddress>()
                .HasOne(x => x.User)
                .WithMany(x => x.IPAddresses)
                .HasForeignKey(x => x.UserId);

            builder.Entity<UserFilePermission>()
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserFilePermission>()
                .HasOne(x => x.Permitted)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Operation>()
                .HasOne(x => x.User);

            builder.Entity<Operation>()
                .HasDiscriminator(x => x.OperationType);
        }
    }
}
