using System.ComponentModel.DataAnnotations;

namespace UkrPoshta_Test.Models
{
    public class CompanyInfo
    {
        [Key]//Первичный ключ
        public int CompanyInfoID { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Довжина інформації про компанію не може перевищувати 1000 рядків")] //Задаем условия что и в БД (Не больше 1000 символов).
        public string CompanyDetails { get; set; }
    }
}
