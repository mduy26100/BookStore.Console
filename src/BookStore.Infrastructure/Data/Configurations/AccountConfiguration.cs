using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.AccountID);

            builder.Property(a => a.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Password)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Address)
                .HasMaxLength(200);

            builder.Property(a => a.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(a => a.Role)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasMany(a => a.Orders)
                .WithOne(o => o.Account)
                .HasForeignKey(o => o.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.ShoppingCarts)
                .WithOne(sc => sc.Account)
                .HasForeignKey(sc => sc.AccountID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
