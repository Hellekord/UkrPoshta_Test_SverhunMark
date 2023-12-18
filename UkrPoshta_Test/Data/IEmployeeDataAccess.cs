using UkrPoshta_Test.Models;
using UkrPoshta_Test.Data; 

namespace UkrPoshta_Test.Data
{
    public interface IEmployeeDataAccess
    {
        // Получение всех работников
        List<Employee> GetAllEmployees();

        // Получение по id ({id})
        Employee GetEmployeeById(int id);

        CompanyInfo GetCompanyInfo(); // Получает информацию о компании

        IEnumerable<string> GetSalaryReportByDepartment(string departmentName); //Получение инфо про зп, метод принимает отдел по которому делаем выборку

        IEnumerable<string> ExportSalaryReport(string departmentName); // Экспортирует отчет о зарплате для конкретного отдела в текстовый формат.

        IEnumerable<Department> GetAllDepartments(); // Получает список всех отделов из базы данных.

        string GetDepartmentNameById(int departmentId); // Получает название отдела на основе id

        IEnumerable<string> ExportEmployeesData(); //Експорт всех (Потому и IEnumerable) данных из БД в txt

        List<Employee> SearchEmployees(string fullName, string departmentName, string positionName); // Ищет сотрудников по полному имени, названию отдела и названию должности.

        bool UpdateEmployeeDetails(int id,Employee employee); // Обновляет данные существующего сотрудника.

        int? GetDepartmentIdByName(string departmentName); // Получает id отдела по его названию.
        int? GetPositionIdByName(string positionName);// Получает id позиции по ее названию.
    }
}
