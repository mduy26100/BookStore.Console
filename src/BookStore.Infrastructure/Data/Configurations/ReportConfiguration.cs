using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.ReportID);

            builder.Property(r => r.Quantity)
                .IsRequired();

            builder.Property(r => r.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.OrderDate)
                .IsRequired();

            builder.Property(r => r.CustomerReviews)
                .HasMaxLength(1000);

            builder.HasOne(r => r.Order)
                .WithMany(o => o.Reports)
                .HasForeignKey(r => r.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Book)
                .WithMany(b => b.Reports)
                .HasForeignKey(r => r.BookID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
