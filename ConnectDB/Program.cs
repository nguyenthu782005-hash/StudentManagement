using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình Database: Ưu tiên PostgreSQL nếu có biến môi trường ConnectionStrings:PostgresConnection
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var postgresConnection = builder.Configuration.GetConnectionString("PostgresConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(postgresConnection))
                {
                    options.UseNpgsql(postgresConnection);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Bật Swagger cho cả môi trường Production để dễ dàng kiểm tra trực tuyến
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Trang chủ mặc định
            app.MapGet("/", () => "API is running! Visit /swagger to test the endpoints.");

            app.MapControllers();

            app.Run();
        }
    }
}