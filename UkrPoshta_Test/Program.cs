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

            
            builder.Services.AddControllers(); // ���������� ������������ � ��������� �������� ��� ��������� �������� API.
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration); // ��������� ������������ � ��������� �������� ��� ������� � ���������� ����������.
            builder.Services.AddTransient<DatabaseConnection>(); // ����������� ��������� ������ DatabaseConnection ��� ���������� ������������ � ���� ������.
            builder.Services.AddCors(); // ���������� �������� ����������� ������������� �������� ����� ����������� (CORS) � ��������� ��������.
            builder.Services.AddScoped<IEmployeeDataAccess, EmployeeDataAccess>(); // ����������� EmployeeDataAccess � �������� ������ � ������������ �������� ��������, ������� ����� �������������� ��� ������� � ������ �� ���� ����������.

            var app = builder.Build();

            
            app.UseCors(x => x // ��������� �������� CORS ��� ���������� ������ ������, ��������� � ���������.
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
            // ����� �������� ����� �������� �������, �������� ������� cookie.

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting(); // ���������� �������������� ������������ ����������� ������������� � �������� ��������.
            app.UseEndpoints(endpoints => // ��������� ������������� �������� ����� ��� �������� �����������.
            {
                endpoints.MapControllers();
            });

            app.MapFallbackToFile("index.html");// ����������� ���������� �����, ������� ����� ��������������, ���� ������ ������� �� ������.



            app.Run();
        }
    }


}