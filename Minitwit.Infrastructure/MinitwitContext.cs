using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Minitwit.Infrastructure.Models;

namespace Minitwit.Infrastructure;

public partial class MinitwitContext : DbContext
{
    public MinitwitContext()
    {
    }

    public MinitwitContext(DbContextOptions<MinitwitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<Latest> Latests { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follower>(entity =>
        {
            entity.HasKey(e => new { e.WhoId, e.WhomId }).HasName("follower_pkey");

            entity.ToTable("follower");

            entity.Property(e => e.WhoId).HasColumnName("who_id");
            entity.Property(e => e.WhomId).HasColumnName("whom_id");
        });

        modelBuilder.Entity<Latest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("latest_pkey");

            entity.ToTable("latest");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommandId).HasColumnName("command_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("message_pkey");

            entity.ToTable("message");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Flagged).HasColumnName("flagged");
            entity.Property(e => e.PubDate).HasColumnName("pub_date");
            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(300)
                .HasColumnName("email");
            entity.Property(e => e.PwHash)
                .HasMaxLength(200)
                .HasColumnName("pw_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
