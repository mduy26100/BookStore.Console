using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.BookID);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Stock)
                .IsRequired();

            builder.Property(b => b.Description)
                .HasMaxLength(1000);

            builder.HasMany(b => b.BookCategories)
                .WithOne(bc => bc.Book)
                .HasForeignKey(bc => bc.BookID);

            builder.HasMany(b => b.OrderDetails)
                .WithOne(od => od.Book)
                .HasForeignKey(od => od.BookID);

            builder.HasMany(b => b.ShoppingCarts)
                .WithOne(sc => sc.Book)
                .HasForeignKey(sc => sc.BookID);

            builder.HasMany(b => b.Reports)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookID);
        }
    }
}
