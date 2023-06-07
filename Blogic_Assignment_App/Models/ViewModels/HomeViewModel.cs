namespace InsuranceApp.Models.ViewModels
{
    /// <summary>
    /// Represents a ViewModel for displaying overall statistics informations for home page.
    /// </summary>
    public class HomeViewModel
    {
        public IDictionary<string, int> ConsultantsInfo { get; set; }

        public IDictionary<string, int> ContractsInfo { get; set; }

        public IDictionary<string, int> ClientsInfo { get; set; }
    }
}
