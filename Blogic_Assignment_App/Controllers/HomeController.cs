using InsuranceApp.Models.ViewModels;
using InsuranceApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApp.Controllers
{
    /// <summary>
    /// Home for getting basic overall statistical actions to display.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IHomeService HomeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IHomeService"/>.
        /// </summary>
        /// <param name="homeService">The contract service</param>
        public HomeController(IHomeService homeService)
        {
            HomeService = homeService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel()
            {
                ConsultantsInfo = await HomeService.GetConsultantsInfoAsync(),
                ContractsInfo = await HomeService.GetContractsInfoAsync(),
                ClientsInfo = await HomeService.GetClientsInfoAsync()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Default eror handling action
        /// </summary>
        /// <returns>ViewModel for error view</returns>
        public IActionResult Error()
        {
            return View();
        }
    }
}
