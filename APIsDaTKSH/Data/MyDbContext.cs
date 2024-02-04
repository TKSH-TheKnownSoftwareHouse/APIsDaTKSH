

using APIsDaTKSH.Models;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }
    public DbSet<ContactModel> Contacts { get; set; }
    public DbSet<HeroModel> Heroes { get; set; }
    public DbSet<DepositionModel> Depositions { get; set; }
    public DbSet<RegisterModel> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DepositionModel>().ToTable("depositions");
        modelBuilder.Entity<ContactModel>().ToTable("contacts");
        modelBuilder.Entity<HeroModel>().ToTable("heroes");
        modelBuilder.Entity<RegisterModel>().ToTable("users");

        Console.WriteLine("Model created: " + modelBuilder.Model);
    }

}


