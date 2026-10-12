using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
     : base(options){}
}

public class Item
{
    [Key]
    public int Id { get; set;}

    public required string Name { get; set; }
}