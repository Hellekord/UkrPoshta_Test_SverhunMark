using static System.Net.WebRequestMethods;

namespace UkrPoshta_Test.Data
{
    public class EmployeeUpdateDto //Использую для инкапсуляции данных и передачи их от клиента на сервер во время запроса HTTP PUT.
    { 
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string PositionName { get; set; }
        public decimal Salary { get; set; }
    }
}
