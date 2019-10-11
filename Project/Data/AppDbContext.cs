using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TestProject.Models;

namespace TestProject.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Sitemap> Sitemaps { get; set; }
        public virtual DbSet<WebAddress> WebAddresses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("PostgresConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("btree_gin")
                .HasPostgresExtension("btree_gist")
                .HasPostgresExtension("citext")
                .HasPostgresExtension("cube")
                .HasPostgresExtension("dblink")
                .HasPostgresExtension("dict_int")
                .HasPostgresExtension("dict_xsyn")
                .HasPostgresExtension("earthdistance")
                .HasPostgresExtension("fuzzystrmatch")
                .HasPostgresExtension("hstore")
                .HasPostgresExtension("intarray")
                .HasPostgresExtension("ltree")
                .HasPostgresExtension("pg_stat_statements")
                .HasPostgresExtension("pg_trgm")
                .HasPostgresExtension("pgcrypto")
                .HasPostgresExtension("pgrowlocks")
                .HasPostgresExtension("pgstattuple")
                .HasPostgresExtension("plv8")
                .HasPostgresExtension("tablefunc")
                .HasPostgresExtension("unaccent")
                .HasPostgresExtension("uuid-ossp")
                .HasPostgresExtension("xml2")
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(e => e.PId);

                entity.HasIndex(e => e.SId);

                entity.Property(e => e.PId).HasColumnName("PId");

                entity.Property(e => e.PageLink).HasMaxLength(2048);

                entity.Property(e => e.SId).HasColumnName("SId");

                entity.HasOne(d => d.Sitemap)
                    .WithMany(p => p.Pages)
                    .HasForeignKey(d => d.SId);
            });

            modelBuilder.Entity<Sitemap>(entity =>
            {
                entity.HasKey(e => e.SId);

                entity.HasIndex(e => e.UId);

                entity.Property(e => e.SId).HasColumnName("SId");

                entity.Property(e => e.SitemapLink).HasMaxLength(2048);

                entity.Property(e => e.UId).HasColumnName("UId");

                entity.HasOne(d => d.WebAddress)
                    .WithMany(p => p.Sitemaps)
                    .HasForeignKey(d => d.UId);
            });

            modelBuilder.Entity<WebAddress>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.Property(e => e.UId).HasColumnName("UId");
            });
        }
    }
}
