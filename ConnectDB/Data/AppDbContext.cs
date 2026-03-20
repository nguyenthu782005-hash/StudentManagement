using System.Collections.Generic;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
}