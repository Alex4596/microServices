using Microsoft.EntityFrameworkCore;

namespace InventoryService.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Todos> Todos { get; set; }
}