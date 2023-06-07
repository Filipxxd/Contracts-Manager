using InsuranceApp.Models.ViewModels.Shared;

namespace InsuranceApp.Models.ViewModels
{
    /// <summary>
    /// Represents a ViewModel for displaying consultant details and associated contracts inside view.
    /// </summary>
    public class ConsultantDetailsViewModel
    {
        public Consultant _Consultant { get; private set; }
        public PaginatedList<Contract> _Contracts { get; private set; }

        public ConsultantDetailsViewModel(Consultant consultant, PaginatedList<Contract> contracts)
        {
            _Consultant = consultant;
            _Contracts = contracts;
        }
    }
}
