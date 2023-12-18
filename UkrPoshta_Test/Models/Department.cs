using System.ComponentModel.DataAnnotations;

namespace UkrPoshta_Test.Models
{
    public class Department
    {
        [Key]//Первичный ключи
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Довжина назви відділу не може перевищувати 100")] //Задаем условия что и в БД (Не больше 100 символов).
        public string DepartmentName { get; set; }
    }
}
