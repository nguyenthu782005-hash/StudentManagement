using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;
using ConnectDB.Data;
using System;
using System.Linq;

namespace CheckUsers
{
    class Program
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=studentdb;Username=postgres;Password=123");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var users = context.Users.ToList();
                Console.WriteLine($"Total users: {users.Count}");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.UserId}, Name: {user.Name}, Email: {user.Email}, Role: {user.Role}, Password: {user.Password}");
                }
            }
        }
    }
}
