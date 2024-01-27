

using APIsDaTKSH.Models;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }
    public DbSet<ContactModel> Contacts { get; set; }
    public DbSet<HeroModel> Hero { get; set; }
    public DbSet<DepositionModel> Depositions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DepositionModel>().ToTable("contact");
        modelBuilder.Entity<ContactModel>().ToTable("contact");
        modelBuilder.Entity<HeroModel>().ToTable("hero");

        // Log to check if the mapping is correct
        Console.WriteLine("Model created: " + modelBuilder.Model);
    }

}


