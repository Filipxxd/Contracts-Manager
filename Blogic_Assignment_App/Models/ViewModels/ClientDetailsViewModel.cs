using InsuranceApp.Models.ViewModels.Shared;

namespace InsuranceApp.Models.ViewModels
{
    /// <summary>
    /// Represents a ViewModel for displaying client details and associated contracts inside view.
    /// </summary>
    public class ClientDetailsViewModel
    {
        public Client _Client { get; private set; }
        public PaginatedList<Contract> _Contracts { get; private set; }

        public ClientDetailsViewModel(Client client, PaginatedList<Contract> contracts)
        {
            _Client = client;
            _Contracts = contracts;
        }
    }
}
