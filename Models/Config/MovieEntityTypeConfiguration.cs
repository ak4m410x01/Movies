using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Movies.API.Models.Config
{
    public class MovieEntityTypeConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            // DB Table Name
            builder.ToTable("Movies");

            // Configure primary key
            builder.HasKey(x => x.Id);

            // Configure Title property
            builder.Property(x => x.Title)
                   .HasMaxLength(250)
                   .IsRequired();

            // Configure Storeline property
            builder.Property(x => x.Storeline)
                   .HasMaxLength(2500)
                   .IsRequired();

            // Configure Genre relationship
            // Genre [1] => Movie [M]
            builder.HasOne(m => m.Genre)
                   .WithMany(g => g.Movies)
                   .HasForeignKey(m => m.GenreId);
        }
    }
}
