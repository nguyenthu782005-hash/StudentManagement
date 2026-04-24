using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConnectDB.Services;

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

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<InvoiceService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ClothStore",
                        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ClothStoreClient",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "this_is_a_very_long_secret_key_for_jwt_auth_12345"))
                    };
                });

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
                    
                    // Seed data
                    ConnectDB.Data.DataSeeder.Seed(context);
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
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => "API is running! Visit /swagger to test the endpoints.");

            app.MapControllers();

            app.Run();
        }

        // HÃ m há»— trá»£ chuyá»ƒn Ä‘á»•i URI postgres:// sang Connection String chuáº©n cá»§a Npgsql
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
