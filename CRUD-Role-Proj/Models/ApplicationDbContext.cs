using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Role_Proj.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {

    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07BFD91648");

            entity.HasOne(d => d.District).WithMany(p => p.Customers).HasConstraintName("FK_Customers_Districts");

            entity.HasOne(d => d.Gender).WithMany(p => p.Customers).HasConstraintName("FK_Customers_Genders");

            entity.HasOne(d => d.Governorate).WithMany(p => p.Customers).HasConstraintName("FK_Customers_Governorates");

            entity.HasOne(d => d.Village).WithMany(p => p.Customers).HasConstraintName("FK_Customers_Villages");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__District__3214EC077EC823DF");

            entity.HasOne(d => d.Governorate).WithMany(p => p.Districts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Districts_Governorates");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genders__3214EC07EC44EC2C");
        });

        modelBuilder.Entity<Governorate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Governor__3214EC07A3DBA8BB");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC072FDCC080");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07B522650D");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<Village>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Villages__3214EC070EC11E0D");

            entity.HasOne(d => d.District).WithMany(p => p.Villages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Villages_Districts");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
