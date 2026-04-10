using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var postgresConnection = builder.Configuration.GetConnectionString("PostgresConnection");

            // Xử lý link postgres:// từ Render nếu có
            if (!string.IsNullOrEmpty(postgresConnection) && postgresConnection.StartsWith("postgres"))
            {
                postgresConnection = ParsePostgresUrl(postgresConnection);
            }

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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Tự động chạy Migration để tạo bảng dữ liệu khi khởi động
            if (!string.IsNullOrEmpty(postgresConnection))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<AppDbContext>();
                        context.Database.Migrate();
                        Console.WriteLine("Database migration applied successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                    }
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
                var port = uri.Port;
                var database = uri.AbsolutePath.TrimStart('/');

                return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=True;";
            }
            catch
            {
                // Nếu lỗi định dạng thì trả về chuỗi gốc để Npgsql tự xử lý (nếu được)
                return url;
            }
        }
    }
}