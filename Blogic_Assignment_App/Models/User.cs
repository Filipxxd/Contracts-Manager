using InsuranceApp.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceApp.Models
{
    /// <summary>
    /// Represents an abstract user meant for derivation.
    /// </summary>
    public abstract class User
    {

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Alphabetical]
        [MaxLength(50)]
        [Display(Name = "Křestní jméno")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Alphabetical]
        [MaxLength(50)]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Věk")]
        [Range(1, 150)]
        public int Age { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [MaxLength(50)]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [MaxLength(13)]
        [Phone]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [MaxLength(11)]
        [RegularExpression("\\d{2}(0[1-9]|1[0-2]|5[1-9]|6[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1])\\/?\\d{3,4}", ErrorMessage = "Musí splňovat podmínky rodného čísla")]
        [Display(Name = "Rodné číslo")]
        public string BirthNumber { get; set; }

        [NotMapped]
        [DisplayName("Klient")]
        public string FullName => FirstName + " " + LastName;
    }
}
