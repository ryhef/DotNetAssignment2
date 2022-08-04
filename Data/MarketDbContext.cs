﻿using Assignment2.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        { 
        }

        public DbSet<Brokerage> Brokerages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brokerage>().ToTable("brokerage");
            modelBuilder.Entity<Client>().ToTable("client");
            modelBuilder.Entity<Subscription>().HasKey(c => new { c.ClientId, c.BrokerageId }); 
        }
    }
}
