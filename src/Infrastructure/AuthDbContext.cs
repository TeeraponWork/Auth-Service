using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Roles> Roles => Set<Roles>();
        public DbSet<UserRoles> UserRoles => Set<UserRoles>();

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
                e.ToTable("users", "public");

                // PK
                e.HasKey(x => x.Id).HasName("users_pkey");

                // id uuid default uuid_generate_v4()
                e.Property(x => x.Id)
                    .HasColumnName("id")
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")  // ตรงกับ DDL
                    .ValueGeneratedOnAdd();

                // email citext not null + unique index name ให้ตรงกับ DDL
                e.Property(x => x.Email)
                    .HasColumnName("email")
                    .HasColumnType("citext")
                    .IsRequired();

                e.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("users_email_key");

                // password_hash text not null
                e.Property(x => x.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasColumnType("text")
                    .IsRequired();

                // display_name text null
                e.Property(x => x.DisplayName)
                    .HasColumnName("display_name")
                    .HasColumnType("text");

                // is_active boolean not null default true
                e.Property(x => x.IsActive)
                    .HasColumnName("is_active")
                    .HasColumnType("boolean")
                    .HasDefaultValue(true)
                    .IsRequired();

                // created_at / updated_at timestamptz not null default now()
                // แนะนำให้ property ฝั่ง C# เป็น DateTimeOffset เพื่อแม็ป timestamptz ได้ตรง
                e.Property(x => x.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                e.Property(x => x.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                // created_by / updated_by uuid null
                e.Property(x => x.CreatedBy)
                    .HasColumnName("created_by")
                    .HasColumnType("uuid");

                e.Property(x => x.UpdatedBy)
                    .HasColumnName("updated_by")
                    .HasColumnType("uuid");

                // first_name / last_name text null
                e.Property(x => x.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("text");

                e.Property(x => x.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("text");

                // gender text null + CHECK ('male'|'female')
                // ถ้าใน entity เป็น string?
                e.Property(x => x.Gender)
                    .HasColumnName("gender")
                    .HasColumnType("text")
                    .HasConversion(
                        v => v == null ? null : (v == Gender.Male ? "male" : "female"),   // enum -> string
                        v => v == null ? null : (v == "male" ? Gender.Male : Gender.Female) // string -> enum
                    );

                // บันทึกชื่อ CHECK constraint ให้ตรงกับฐานข้อมูล (เพื่อความสอดคล้อง)
                e.ToTable(t => t.HasCheckConstraint(
                    "users_gender_check",
                    "gender = ANY (ARRAY['male'::text, 'female'::text])"
                ));
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

            // ---------- Roles ----------
            modelBuilder.Entity<Roles>(e =>
            {
                e.ToTable("roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
                e.Property(x => x.Name).HasColumnType("citext").IsRequired();
            });

            // ---------- UserRoles ----------
            modelBuilder.Entity<UserRoles>(e =>
            {
                e.ToTable("user_roles");
                e.HasKey(x => new { x.UserId, x.RoleId }); // Composite Key

                e.HasOne(x => x.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
