#nullable enable
using InsuranceApp.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Models.ViewModels
{
    /// <summary>
    /// Represents a ViewModel class for displaying consultant details and associated contracts inside view.
    /// </summary>
    public class ContractCreateViewModel
    {
        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Evidenční číslo")]
        [MaxLength(50)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Evidenční číslo musí být složeno pouze z čísel")]
        public string EvidenceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Musí být vyplněno")]
        [MaxLength(30)]
        [Display(Name = "Instituce")]
        [Alphabetical]
        public string Institution { get; set; } = string.Empty;

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Datum Uzavření")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateSigned { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Platnost Od")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [LaterThan("DateSigned")]
        public DateTime DateValidStart { get; set; }

        [Required(ErrorMessage = "Musí být vyplněno")]
        [Display(Name = "Platnost Do")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [LaterThan("DateValidStart")]
        public DateTime DateValidEnd { get; set; }

        [ValidateNever]
        public int? ClientId { get; set; }

        [ValidateNever]
        public List<int>? ConsultantIds { get; set; }
    }
}
