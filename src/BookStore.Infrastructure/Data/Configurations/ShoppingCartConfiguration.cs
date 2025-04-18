using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations
{
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.ToTable("ShoppingCarts");

            builder.HasKey(sc => sc.CartID);

            builder.Property(sc => sc.Quantity)
                .IsRequired();

            builder.Property(sc => sc.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(sc => sc.Account)
                .WithMany(a => a.ShoppingCarts)
                .HasForeignKey(sc => sc.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sc => sc.Book)
                .WithMany(b => b.ShoppingCarts)
                .HasForeignKey(sc => sc.BookID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
