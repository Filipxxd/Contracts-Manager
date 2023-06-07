using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Models
{
    /// <summary>
    /// Represents a consultant entity.
    /// </summary>
    public class Consultant : User
    {
        [Key]
        public int ConsultantId { get; set; }

        public ICollection<Contract> Contracts { get; set; }
    }
}
