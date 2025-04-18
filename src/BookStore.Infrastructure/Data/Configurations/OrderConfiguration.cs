using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.OrderID);

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(20);

            builder.Property(o => o.CustomerEmail)
                .HasMaxLength(100);

            builder.Property(o => o.CustomerAddress)
                .HasMaxLength(200);

            builder.Property(o => o.CustomerName)
                .HasMaxLength(100);

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.Status)
                .HasMaxLength(50);

            builder.HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderID);

            builder.HasMany(o => o.Reports)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderID);
        }
    }
}
