﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using archive.Data;

namespace archive.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("archive.Data.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("HomePage")
                        .HasMaxLength(256);

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("timestamp");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("archive.Data.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserId")
                        .IsRequired();

                    b.Property<string>("CachedContent");

                    b.Property<DateTime>("CommentDate")
                        .HasColumnType("timestamp");

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<int>("SolutionId");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("SolutionId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("archive.Data.Entities.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Archive");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ShortcutCode")
                        .HasColumnType("VARCHAR(8)");

                    b.HasKey("Id");

                    b.HasIndex("ShortcutCode")
                        .IsUnique();

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("archive.Data.Entities.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(256)");

                    b.Property<string>("MimeSubtype")
                        .IsRequired()
                        .HasColumnType("VARCHAR(127)");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("VARCHAR(127)");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("archive.Data.Entities.Rating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IdSolution");

                    b.Property<string>("NameUser")
                        .IsRequired();

                    b.Property<bool>("Value");

                    b.HasKey("Id");

                    b.HasAlternateKey("IdSolution", "NameUser");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("archive.Data.Entities.Solution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("CachedContent");

                    b.Property<long?>("CurrentVersionId");

                    b.Property<int>("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CurrentVersionId");

                    b.HasIndex("TaskId");

                    b.ToTable("Solutions");
                });

            modelBuilder.Entity("archive.Data.Entities.SolutionVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("Created");

                    b.Property<int>("SolutionId");

                    b.HasKey("Id");

                    b.HasIndex("SolutionId");

                    b.ToTable("SolutionsVersions");
                });

            modelBuilder.Entity("archive.Data.Entities.SolutionsFiles", b =>
                {
                    b.Property<int>("SolutionId");

                    b.Property<Guid>("FileId");

                    b.HasKey("SolutionId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("SolutionsFiles");
                });

            modelBuilder.Entity("archive.Data.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("TaskId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("archive.Data.Entities.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CachedContent");

                    b.Property<string>("Content");

                    b.Property<string>("Name");

                    b.Property<int>("TasksetId");

                    b.HasKey("Id");

                    b.HasIndex("TasksetId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("archive.Data.Entities.TasksFiles", b =>
                {
                    b.Property<int>("TaskId");

                    b.Property<Guid>("FileId");

                    b.HasKey("TaskId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("TasksFiles");
                });

            modelBuilder.Entity("archive.Data.Entities.Taskset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CourseId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ShortcutCode")
                        .HasColumnType("VARCHAR(8)");

                    b.Property<int>("Type");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.HasIndex("CourseId", "ShortcutCode")
                        .IsUnique()
                        .HasAnnotation("Npgsql:IndexMethod", "btree");

                    b.ToTable("Tasksets");
                });

            modelBuilder.Entity("archive.Data.Entities.TasksetsFiles", b =>
                {
                    b.Property<int>("TasksetId");

                    b.Property<Guid>("FileId");

                    b.HasKey("TasksetId", "FileId");

                    b.HasIndex("FileId");

                    b.ToTable("TasksetsFiles");
                });

            modelBuilder.Entity("archive.Data.Entities.UserAvatar", b =>
                {
                    b.Property<string>("ApplicationUserId");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasMaxLength(3145728);

                    b.HasKey("ApplicationUserId");

                    b.ToTable("Avatars");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("archive.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("archive.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("archive.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("archive.Data.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.Comment", b =>
                {
                    b.HasOne("archive.Data.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany("Comments")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("archive.Data.Entities.Solution", "Solution")
                        .WithMany("Comments")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.Solution", b =>
                {
                    b.HasOne("archive.Data.Entities.ApplicationUser", "Author")
                        .WithMany("Solutions")
                        .HasForeignKey("AuthorId");

                    b.HasOne("archive.Data.Entities.SolutionVersion", "CurrentVersion")
                        .WithMany()
                        .HasForeignKey("CurrentVersionId");

                    b.HasOne("archive.Data.Entities.Task", "Task")
                        .WithMany("Solutions")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.SolutionVersion", b =>
                {
                    b.HasOne("archive.Data.Entities.Solution", "Solution")
                        .WithMany("Versions")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.SolutionsFiles", b =>
                {
                    b.HasOne("archive.Data.Entities.File", "File")
                        .WithMany("SolutionsReferers")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("archive.Data.Entities.Solution", "Solution")
                        .WithMany("Attachments")
                        .HasForeignKey("SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.Tag", b =>
                {
                    b.HasOne("archive.Data.Entities.Task", "Task")
                        .WithMany("Tags")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.Task", b =>
                {
                    b.HasOne("archive.Data.Entities.Taskset", "Taskset")
                        .WithMany("Tasks")
                        .HasForeignKey("TasksetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.TasksFiles", b =>
                {
                    b.HasOne("archive.Data.Entities.File", "File")
                        .WithMany("TasksReferers")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("archive.Data.Entities.Task", "Task")
                        .WithMany("Attachments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.Taskset", b =>
                {
                    b.HasOne("archive.Data.Entities.Course", "Course")
                        .WithMany("Tasksets")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.TasksetsFiles", b =>
                {
                    b.HasOne("archive.Data.Entities.File", "File")
                        .WithMany("TasksetReferers")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("archive.Data.Entities.Taskset", "Taskset")
                        .WithMany("Attachments")
                        .HasForeignKey("TasksetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("archive.Data.Entities.UserAvatar", b =>
                {
                    b.HasOne("archive.Data.Entities.ApplicationUser", "ApplicationUser")
                        .WithOne("Avatar")
                        .HasForeignKey("archive.Data.Entities.UserAvatar", "ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
