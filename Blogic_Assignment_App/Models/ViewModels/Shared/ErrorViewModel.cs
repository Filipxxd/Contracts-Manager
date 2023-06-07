namespace InsuranceApp.Models.ViewModels.Shared
{
    /// <summary>
    /// Default error view model
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = string.Empty;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}