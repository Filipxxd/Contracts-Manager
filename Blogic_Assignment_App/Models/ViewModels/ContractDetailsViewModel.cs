using InsuranceApp.Models.ViewModels.Shared;

namespace InsuranceApp.Models.ViewModels
{
    /// <summary>
    /// ViewModel class for displaying contract details and associated consultants inside view.
    /// </summary>
    public class ContractDetailsViewModel
    {
        public Contract _Contract { get; private set; }
        public PaginatedList<Consultant> _Consultants { get; private set; }

        public ContractDetailsViewModel(Contract contract, PaginatedList<Consultant> consultant)
        {
            _Contract = contract;
            _Consultants = consultant;
        }
    }
}
