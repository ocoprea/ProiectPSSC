﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProiectPSSC.Data.Models;

namespace ProiectPSSC.Data
{
    public class OrderingContext : DbContext
    {
        public DbSet<ProductDto> Products { get; set; }
        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<OrderedProductDto> orderedProducts { get; set; }
        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductDto>().ToTable("Produs").HasKey(p => p.IdProdus);
            modelBuilder.Entity<OrderDto>().ToTable("Comanda").HasKey(c => c.IdComanda);
            modelBuilder.Entity<OrderedProductDto>().ToTable("ProdusComandat").HasKey(pc => pc.IdProdusComandat);
        }
    }
}
