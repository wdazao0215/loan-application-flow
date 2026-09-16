using LoanFlow.Domain.Applicants;
using LoanFlow.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LoanFlow.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Id).ValueGeneratedNever();

        builder.Property(customer => customer.Ssn)
            .HasConversion(ssn => ssn.Value, value => new Ssn(value))
            .HasMaxLength(9);
        builder.HasIndex(customer => customer.Ssn).IsUnique();

        builder.Property(customer => customer.FirstName).HasMaxLength(LoanRequest.NameMaxLength);
        builder.Property(customer => customer.LastName).HasMaxLength(LoanRequest.NameMaxLength);
        builder.Property(customer => customer.CompanyName).HasMaxLength(LoanRequest.CompanyNameMaxLength);

        builder.ComplexProperty(customer => customer.Address, address =>
        {
            address.Property(value => value.Street).HasColumnName("address_street").HasMaxLength(Address.StreetMaxLength);
            address.Property(value => value.City).HasColumnName("address_city").HasMaxLength(Address.CityMaxLength);
            address.Property(value => value.State).HasColumnName("address_state").HasMaxLength(2);
            address.Property(value => value.ZipCode).HasColumnName("address_zip_code").HasMaxLength(10);
        });

        builder.HasOne(customer => customer.Application)
            .WithOne()
            .HasForeignKey<LoanApplication>(application => application.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(customer => customer.Application).AutoInclude();
    }
}
