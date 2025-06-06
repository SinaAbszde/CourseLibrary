﻿// <auto-generated />
using System;
using CourseLibrary.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CourseLibrary.API.Migrations
{
    [DbContext(typeof(CourseLibraryContext))]
    partial class CourseLibraryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.6");

            modelBuilder.Entity("CourseLibrary.API.Entities.Author", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long>("DateOfBirth")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("DateOfDeath")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("MainCategory")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Authors");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                            DateOfBirth = 1279360106496000270L,
                            FirstName = "Berry",
                            LastName = "Griffin Beak Eldritch",
                            MainCategory = "Ships"
                        },
                        new
                        {
                            Id = new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                            DateOfBirth = 1277955145728000270L,
                            FirstName = "Nancy",
                            LastName = "Swashbuckler Rye",
                            MainCategory = "Rum"
                        },
                        new
                        {
                            Id = new Guid("2902b665-1190-4c70-9915-b9c2d7680450"),
                            DateOfBirth = 1264753115136000210L,
                            FirstName = "Eli",
                            LastName = "Ivory Bones Sweet",
                            MainCategory = "Singing"
                        },
                        new
                        {
                            Id = new Guid("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                            DateOfBirth = 1264248815616000210L,
                            FirstName = "Arnold",
                            LastName = "The Unseen Stafford",
                            MainCategory = "Singing"
                        },
                        new
                        {
                            Id = new Guid("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                            DateOfBirth = 1264066560000000210L,
                            FirstName = "Seabury",
                            LastName = "Toxic Reyson",
                            MainCategory = "Maps"
                        },
                        new
                        {
                            Id = new Guid("2aadd2df-7caf-45ab-9355-7f6332985a87"),
                            DateOfBirth = 1279813091328000270L,
                            FirstName = "Rutherford",
                            LastName = "Fearless Cloven",
                            MainCategory = "General debauchery"
                        },
                        new
                        {
                            Id = new Guid("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"),
                            DateOfBirth = 1280793378816000210L,
                            FirstName = "Atherton",
                            LastName = "Crow Ridley",
                            MainCategory = "Rum"
                        });
                });

            modelBuilder.Entity("CourseLibrary.API.Entities.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(1500)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Courses");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                            AuthorId = new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                            Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers.",
                            Title = "Commandeering a Ship Without Getting Caught"
                        },
                        new
                        {
                            Id = new Guid("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                            AuthorId = new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                            Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny.",
                            Title = "Overthrowing Mutiny"
                        },
                        new
                        {
                            Id = new Guid("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
                            AuthorId = new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                            Description = "Every good pirate loves rum, but it also has a tendency to get you into trouble.  In this course you'll learn how to avoid that.  This new exclusive edition includes an additional chapter on how to run fast without falling while drunk.",
                            Title = "Avoiding Brawls While Drinking as Much Rum as You Desire"
                        },
                        new
                        {
                            Id = new Guid("40ff5488-fdab-45b5-bc3a-14302d59869a"),
                            AuthorId = new Guid("2902b665-1190-4c70-9915-b9c2d7680450"),
                            Description = "In this course you'll learn how to sing all-time favourite pirate songs without sounding like you actually know the words or how to hold a note.",
                            Title = "Singalong Pirate Hits"
                        });
                });

            modelBuilder.Entity("CourseLibrary.API.Entities.Course", b =>
                {
                    b.HasOne("CourseLibrary.API.Entities.Author", "Author")
                        .WithMany("Courses")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("CourseLibrary.API.Entities.Author", b =>
                {
                    b.Navigation("Courses");
                });
#pragma warning restore 612, 618
        }
    }
}
