using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Role_Proj.Models;

public partial class CrudRoleDbContext : DbContext
{
    public CrudRoleDbContext()
    {
    }

    public CrudRoleDbContext(DbContextOptions<CrudRoleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Governorate> Governorates { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Village> Villages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=CrudRoleDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07B8B2EFE0");

            entity.HasIndex(e => e.NationalId, "UQ__Customer__E9AA321A194E0E94").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.NationalId)
                .HasMaxLength(14)
                .HasColumnName("NationalID");

            entity.HasOne(d => d.District).WithMany(p => p.Customers)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_Customers_Districts");

            entity.HasOne(d => d.Gender).WithMany(p => p.Customers)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_Customers_Genders");

            entity.HasOne(d => d.Governorate).WithMany(p => p.Customers)
                .HasForeignKey(d => d.GovernorateId)
                .HasConstraintName("FK_Customers_Governorates");

            entity.HasOne(d => d.Village).WithMany(p => p.Customers)
                .HasForeignKey(d => d.VillageId)
                .HasConstraintName("FK_Customers_Villages");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__District__3214EC07ACD7A057");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Governorate).WithMany(p => p.Districts)
                .HasForeignKey(d => d.GovernorateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Districts_Governorates");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genders__3214EC0719D37E28");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Governorate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Governor__3214EC073A3AE062");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07E4DDE82E");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC074A2DAFB8");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4D48B36EC").IsUnique();

            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<Village>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Villages__3214EC07349AAD73");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.District).WithMany(p => p.Villages)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Villages_Districts");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
