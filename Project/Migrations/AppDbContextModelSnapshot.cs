﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestProject.Data;

namespace TestProject.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:btree_gin", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:citext", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:cube", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:dblink", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:dict_int", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:dict_xsyn", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:earthdistance", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:hstore", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:intarray", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:ltree", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:pg_stat_statements", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:pgrowlocks", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:pgstattuple", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:plv8", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:tablefunc", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:unaccent", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:xml2", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TestProject.Models.Page", b =>
                {
                    b.Property<int>("PId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("PId");

                    b.Property<string>("PageLink")
                        .HasMaxLength(2048);

                    b.Property<double?>("ResponseTime");

                    b.Property<int>("SId")
                        .HasColumnName("SId");

                    b.HasKey("PId");

                    b.HasIndex("SId");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("TestProject.Models.Sitemap", b =>
                {
                    b.Property<int>("SId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("SId");

                    b.Property<string>("SitemapLink")
                        .HasMaxLength(2048);

                    b.Property<int>("UId")
                        .HasColumnName("UId");

                    b.HasKey("SId");

                    b.HasIndex("UId");

                    b.ToTable("Sitemaps");
                });

            modelBuilder.Entity("TestProject.Models.WebAddress", b =>
                {
                    b.Property<int>("UId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("UId");

                    b.Property<string>("Url");

                    b.Property<bool>("isSuccess");

                    b.HasKey("UId");

                    b.ToTable("WebAddresses");
                });

            modelBuilder.Entity("TestProject.Models.Page", b =>
                {
                    b.HasOne("TestProject.Models.Sitemap", "Sitemap")
                        .WithMany("Pages")
                        .HasForeignKey("SId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestProject.Models.Sitemap", b =>
                {
                    b.HasOne("TestProject.Models.WebAddress", "WebAddress")
                        .WithMany("Sitemaps")
                        .HasForeignKey("UId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
