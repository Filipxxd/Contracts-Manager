using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Models
{
    /// <summary>
    /// Represents a contract entity.
    /// </summary>
    public class Contract
    {
        [Key]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Evidenční číslo")]
        [MaxLength(20)]
        public string EvidenceNumber { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Instituce")]
        [MaxLength(30)]
        public string Institution { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Datum Uzavření")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateSigned { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Platnost Od")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateValidStart { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Platnost Do")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime DateValidEnd { get; set; }

        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public ICollection<Consultant> Consultants { get; set; }
    }
}
