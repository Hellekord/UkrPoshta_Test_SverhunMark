using System.ComponentModel.DataAnnotations;

namespace UkrPoshta_Test.Models
{
    public class Position
    {
        [Key]
        public int PositionID { get; set; }

        [Required]
        [StringLength(100,ErrorMessage = "Довжина назви посади не може перевищувати 100.")]
        public string PositionName { get; set; }
    }
}
