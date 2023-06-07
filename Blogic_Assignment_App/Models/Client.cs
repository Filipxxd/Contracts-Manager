using System.ComponentModel.DataAnnotations;

namespace InsuranceApp.Models
{
    /// <summary>
    /// Represents a client entity.
    /// </summary>
    public class Client : User
    {
        [Key]
        public int ClientId { get; set; }

        public virtual ICollection<Contract> Contracts { get; }
    }
}
