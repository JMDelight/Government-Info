using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GovernmentInformation.Models;

namespace GovernmentInformation.Migrations
{
    [DbContext(typeof(GovernmentInformationDbContext))]
    partial class GovernmentInformationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GovernmentInformation.Models.Query", b =>
                {
                    b.Property<int>("QueryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("UserId");

                    b.HasKey("QueryId");

                    b.HasIndex("UserId");

                    b.ToTable("Queries");
                });

            modelBuilder.Entity("GovernmentInformation.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserName");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GovernmentInformation.Models.Query", b =>
                {
                    b.HasOne("GovernmentInformation.Models.User", "User")
                        .WithMany("Queries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
