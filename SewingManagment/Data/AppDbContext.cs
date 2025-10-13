using Microsoft.EntityFrameworkCore;
using SewingManagment.Models;

namespace SewingManagment.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        // 在這裡新增 DbSet 以新增模型，並執行 Migration
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
