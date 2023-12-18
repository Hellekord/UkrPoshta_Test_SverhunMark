using Microsoft.AspNetCore.Mvc;
using UkrPoshta_Test.Data;
using UkrPoshta_Test.Models;
using System.Collections.Generic; // Добавляем для использования List<T>
using System.Text;

namespace UkrPoshta_Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        // Используем интерфейс, а не конкретный класс для внедрения зависимостей
        private readonly IEmployeeDataAccess _employeeDataAccess;

        // Через конструктор внедряем уровень доступа к данным что у нас есть через интерфейс
        public EmployeesController(IEmployeeDataAccess employeeDataAccess)
        {
            _employeeDataAccess = employeeDataAccess;
        }

        // Получение всех пользователей
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            List<Employee> employees = _employeeDataAccess.GetAllEmployees();
            return Ok(employees);
        }

        // Получение пользователя по {id}
        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            Employee employee = _employeeDataAccess.GetEmployeeById(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            return NotFound();
        }

        // Изминение данных
        [HttpPut("changes/{employeeId}")]
        public IActionResult UpdateEmployeeDetails(int employeeId, [FromBody] EmployeeUpdateDto employeeUpdateDto)
        {
            if (employeeUpdateDto == null)
            {
                return BadRequest("Invalid employee data.");
            }

            // Преобразуем имена в id
            var departmentId = _employeeDataAccess.GetDepartmentIdByName(employeeUpdateDto.DepartmentName);
            var positionId = _employeeDataAccess.GetPositionIdByName(employeeUpdateDto.PositionName);

            if (!departmentId.HasValue || !positionId.HasValue)
            {
                return NotFound("Department or position not found.");
            }

            // Сопоставляем DTO с моделью 
            var employeeUpdate = new Employee
            {
                FullName = employeeUpdateDto.FullName,
                DepartmentID = departmentId.Value,
                PositionID = positionId.Value,
                Salary = employeeUpdateDto.Salary
            };

            // Обновляет данные о сотруднике
            var updateResult = _employeeDataAccess.UpdateEmployeeDetails(employeeId, employeeUpdate);
            if (updateResult)
            {
                return Ok("Employee updated successfully.");
            }
            else
            {
                return StatusCode(500, "An error occurred while updating the employee.");
            }
        }



        [HttpGet("companyinfo")]
        public IActionResult GetCompanyInfo()
        {
            try
            {
                var companyInfo = _employeeDataAccess.GetCompanyInfo();
                if (companyInfo != null)
                {
                    return Ok(companyInfo);
                }
                else
                {
                    return NotFound("Company information is not available.");
                }
            }
            catch (Exception ex)
            {
                // В случае исключения установим заголок статусного кода - ошибка 500 и инфо об ошибке
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("departments")]
        public IActionResult GetAllDepartments()
        {
            try
            {
                var departments = _employeeDataAccess.GetAllDepartments();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving departments.");
            }
        }

        [HttpGet("salaryreport")]
        public IActionResult GetSalaryReport([FromQuery] string departmentName)
        {
            try
            {
                //Непосредственно используем название отдела для получения отчета
                var reportLines = _employeeDataAccess.GetSalaryReportByDepartment(departmentName);
                if (!reportLines.Any())
                {
                    return NotFound($"No salary report found for department: {departmentName}.");
                }

                var content = string.Join("\n", reportLines);
                var contentBytes = Encoding.UTF8.GetBytes(content);
                var contentType = "text/plain";
                var fileName = $"{departmentName}_salary_report.txt";

                return File(contentBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while generating the salary report.");
            }
        }


        [HttpGet("exportSalaryReport")]
        public IActionResult ExportSalaryReport([FromQuery] string departmentName)
        {
            try
            {
                var reportData = _employeeDataAccess.ExportSalaryReport(departmentName);
                if (reportData == null)
                {
                    return NotFound($"Department with name {departmentName} not found or no data available.");
                }

                var content = string.Join("\n", reportData);
                var contentBytes = Encoding.UTF8.GetBytes(content);
                var contentType = "text/plain";
                var fileName = $"{departmentName}_salary_report.txt";

                return File(contentBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while exporting the salary report.");
            }
        }




        [HttpGet("search")]
        public IActionResult SearchEmployees([FromQuery] string? fullName, [FromQuery] string? departmentName, [FromQuery] string? positionName)
        {
            //Вызов обновленного метода SearchEmployees с новыми параметрами
            var employees = _employeeDataAccess.SearchEmployees(fullName, departmentName, positionName);

            if (employees.Any())
            {
                return Ok(employees);
            }
            else
            {
                return NotFound("No employees found.");
            }
        }


    }
}
