using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.DataAccess.Configurations
{
    public class MakerModelConfiguration : IEntityTypeConfiguration<MakerModel>
    {
        public void Configure(EntityTypeBuilder<MakerModel> builder)
        {
            builder.HasIndex(x => x.Id).IsUnique();
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Maker)
               .WithMany(x => x.Models)
               .HasForeignKey(x => x.MakerId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
