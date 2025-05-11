using System;
using System.Collections.Generic;
using HotelManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Data;

public partial class Hotelnhom2Context : DbContext
{
    public Hotelnhom2Context()
    {
    }

    public Hotelnhom2Context(DbContextOptions<Hotelnhom2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=118.69.126.49;Database=data_QLKS_115_Nhom2;User Id=user_115_nhom2;Password=123456789;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.MaFeedback).HasName("PK__Feedback__63042CF68D0745C0");

            entity.ToTable("Feedback");

            entity.Property(e => e.MaFeedback).HasMaxLength(6);
            entity.Property(e => e.BinhLuan).HasMaxLength(500);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.MaPhong).HasMaxLength(6);
            entity.Property(e => e.NgayFeedback)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhanLoai).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
