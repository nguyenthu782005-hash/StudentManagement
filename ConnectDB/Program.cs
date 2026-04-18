using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Fix PostgreSQL DateTime issues
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);

            // Connect to PostgreSQL
            var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

            // Handle Render's postgres:// URL if applicable
            if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres"))
            {
                connectionString = ParsePostgresUrl(connectionString);
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS support
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            // Automatic Migration
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    // Only migrate if we are not using localhost (usually production/Render)
                    // Or you can always migrate if you want local DB to stay updated.
                    if (!connectionString.Contains("localhost"))
                    {
                        context.Database.Migrate();
                        Console.WriteLine("Database migration applied successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            // Use CORS
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapGet("/", () => "API is running! Visit /swagger to test the endpoints.");

            app.MapControllers();

            app.Run();
        }

        // Hàm hỗ trợ chuyển đổi URI postgres:// sang Connection String chuẩn của Npgsql
        private static string ParsePostgresUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var userInfo = uri.UserInfo.Split(':');
                var user = userInfo[0];
                var password = userInfo.Length > 1 ? userInfo[1] : "";
                var host = uri.Host;
                var port = uri.Port == -1 ? 5432 : uri.Port;
                var database = uri.AbsolutePath.TrimStart('/');

                return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=True;";
            }
            catch
            {
                return url;
            }
        }
    }
}