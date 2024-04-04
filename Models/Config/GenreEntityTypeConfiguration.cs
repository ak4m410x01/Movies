using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Movies.API.Models.Config
{
    public class GenreEntityTypeConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            // DB Table Name
            builder.ToTable("Genres");

            // Configure primary key
            builder.HasKey(x => x.Id);

            // Configure Id propery
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd()
                   .UseIdentityColumn(1, 1);

            // Configure Name property
            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired();
        }
    }
}
