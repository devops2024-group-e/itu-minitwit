﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Minitwit.Models;

namespace Minitwit;

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

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follower>(entity =>
        {
            entity
                .ToTable("follower")
                .HasKey(e => new { e.WhoId, e.WhomId });

            entity.Property(e => e.WhoId).HasColumnName("who_id");
            entity.Property(e => e.WhomId).HasColumnName("whom_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("message");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Flagged).HasColumnName("flagged");
            entity.Property(e => e.PubDate).HasColumnName("pub_date");
            entity.Property(e => e.Text)
                .HasColumnType("string")
                .HasColumnName("text");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasColumnType("string")
                .HasColumnName("email");
            entity.Property(e => e.PwHash)
                .HasColumnType("string")
                .HasColumnName("pw_hash");
            entity.Property(e => e.Username)
                .HasColumnType("string")
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}