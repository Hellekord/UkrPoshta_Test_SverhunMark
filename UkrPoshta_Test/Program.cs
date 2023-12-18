using UkrPoshta_Test.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UkrPoshta_Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddControllers(); // Добавление контроллеров в коллекцию сервисов для обработки запросов API.
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // Добавляем конфигурацию в коллекцию сервисов для доступа к настройкам приложения.
            builder.Services.AddTransient<DatabaseConnection>(); // Регистрация временной службы DatabaseConnection для управления подключением к базе данных.
            builder.Services.AddCors(); // Добавление сервисов совместного использования ресурсов между источниками (CORS) в коллекцию сервисов.
            builder.Services.AddScoped<IEmployeeDataAccess, EmployeeDataAccess>(); // Регистрация EmployeeDataAccess в качестве службы с ограниченной областью действия, которая будет использоваться для доступа к данным во всем приложении.

            var app = builder.Build();

            
            app.UseCors(x => x // Настройка политики CORS для разрешения любого метода, заголовка и источника.
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
            // Также разрешен обмен учетными данными, например файлами cookie.

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting(); // Добавление промежуточного программного обеспечения маршрутизации в конвейер запросов.
            app.UseEndpoints(endpoints => // Настройка маршрутизации конечной точки для действий контроллера.
            {
                endpoints.MapControllers();
            });

            app.MapFallbackToFile("index.html");// Определение резервного файла, который будет использоваться, если другой маршрут не найден.



            app.Run();
        }
    }


}