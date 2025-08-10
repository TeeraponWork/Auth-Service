using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ใช้ gen_random_uuid() จาก pgcrypto และอีเมลแบบไม่แคร์ตัวพิมพ์ด้วย citext
            modelBuilder.HasPostgresExtension("pgcrypto");
            modelBuilder.HasPostgresExtension("citext");
            // ถ้าคุณอยากใช้ uuid_generate_v4() ให้เปิด uuid-ossp และเปลี่ยน defaultValueSql ด้านล่าง
            // modelBuilder.HasPostgresExtension("uuid-ossp");

            // ---------- users ----------
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()"); // หรือ "uuid_generate_v4()" ถ้าใช้ uuid-ossp
                e.Property(x => x.Email).HasColumnType("citext").IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("now()");
            });

            // ---------- refresh_tokens ----------
            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.ToTable("refresh_tokens");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()"); // หรือ "uuid_generate_v4()" ตามที่เปิด extension

                e.Property(x => x.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                e.HasOne(x => x.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.TokenHash)
                    .HasColumnName("token_hash")
                    .IsRequired();

                e.Property(x => x.ExpiresAt)
                    .HasColumnName("expires_at")
                    .HasColumnType("timestamptz")
                    .IsRequired();

                e.Property(x => x.RevokedAt)
                    .HasColumnName("revoked_at")
                    .HasColumnType("timestamptz");

                e.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("now()");

                // ช่วยให้คิวรีหา token ที่ยังไม่หมดอายุของ user ได้เร็ว
                e.HasIndex(x => new { x.UserId, x.ExpiresAt });
            });
        }
    }
}
