﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.DataAccess.Configurations
{
    public class ContactInformationConfiguration : IEntityTypeConfiguration<ContactInformation>
    {
        public void Configure(EntityTypeBuilder<ContactInformation> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.ContactInformations)
                .HasForeignKey(x => x.CustomerId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
