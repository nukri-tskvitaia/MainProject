using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcProject.Models;

namespace MvcProject.Data;

public partial class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DepositWithdrawRequest> DepositWithdrawRequests { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Wallet> Wallets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Renaming Identity tables
        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        builder.Entity<DepositWithdrawRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk_DepositWithdrawRequests_Id");

            entity.Property(e => e.Amount).HasColumnType("money").IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.Property(e => e.TransactionType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.HasOne(e => e.User)
            .WithMany(e => e.DepositWithdraws)
            .HasForeignKey(e => e.UserId); 
        });

        builder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk_Transactions_Id");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("money").IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnType("datetime").IsRequired();
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(450);
            entity.HasOne(e => e.User)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.UserId);
        });

        builder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pk_Wallet_Id");

            entity.ToTable("Wallet");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CurrentBalance).HasColumnType("money").IsRequired();
            entity.Property(e => e.BlockedAmount).HasColumnType("money").IsRequired();
            entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            entity.Property(e => e.Currency).IsRequired();
            entity.HasOne(e => e.User)
                .WithOne(e => e.Wallet)
                .HasForeignKey<Wallet>(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(builder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
