using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace UkrPoshta_Test.Models
{
    public class Employee
    {

        [Key] //Первичный ключи
        public int EmployeeID { get; set; } //Идентификатор сотрудника из БД

        [Required] //Означает что у свойства должно быть значение (Условная проверка на NULL)
        [StringLength(50, ErrorMessage = "Довжина ПІБ не може бути більше 50")] //Задаем условия что и в БД (Не больше 50 символов).
        public string FullName { get; set; } //Строка из БД (nvarchar) - Имя

        [StringLength(100, ErrorMessage = "Довжина адреси не може перевищувати 100")] //Задаем условия что и в БД (Не больше 100 символов).
        public string Address { get; set; } //Адреса из БД (тоже nvarchar)

        [Phone]
        [StringLength(20, ErrorMessage = "Довжина номера телефону не може перевищувати 20")]
        public string Telephone { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfEmployment { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Зарплата повинна бути від 0 до 1000000.")]
        public decimal Salary { get; set; }

        [Required]
        public int DepartmentID { get; set; } // Foreign key к таблице Department

        [Required]
        public int PositionID { get; set; } // Foreign key к таблице Position 

        public string DepartmentName { get; set; }
        public string PositionName { get; set; }


    }
}
