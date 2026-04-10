using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình Database: Mặc định luôn sử dụng PostgreSQL
            var postgresConnection = builder.Configuration.GetConnectionString("PostgresConnection");

            // Xử lý link postgres:// từ Render nếu có
            if (!string.IsNullOrEmpty(postgresConnection) && postgresConnection.StartsWith("postgres"))
            {
                postgresConnection = ParsePostgresUrl(postgresConnection);
            }

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                // Chỉ sử dụng PostgreSQL
                if (!string.IsNullOrEmpty(postgresConnection))
                {
                    options.UseNpgsql(postgresConnection);
                }
                else
                {
                    // Fallback to a dummy for build/migration time if env var is missing
                    options.UseNpgsql("Host=localhost;Database=dummy;Username=dummy;Password=dummy");
                }
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Tự động chạy Migration để tạo bảng dữ liệu khi khởi động
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    // Chỉ chạy migrate nếu là SQL thực sự (trên Render)
                    if (!string.IsNullOrEmpty(postgresConnection) && postgresConnection.Contains("Password"))
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