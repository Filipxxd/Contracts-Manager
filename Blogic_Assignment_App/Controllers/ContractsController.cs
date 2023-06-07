using InsuranceApp.Models;
using InsuranceApp.Models.ViewModels;
using InsuranceApp.Models.ViewModels.Shared;
using InsuranceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InsuranceApp.Controllers
{
    /// <summary>
    /// Controller for managing contract-based actions.
    /// </summary>
    public class ContractsController : Controller
    {
        private readonly IContractService ContractService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IContractService"/>.
        /// </summary>
        /// <param name="contractService">The contract service</param>
        public ContractsController(IContractService contractService)
        {
            ContractService = contractService;
        }

        /// <summary>
        /// Displays a paginated list of contracts with sorting and searching options.
        /// </summary>
        /// <param name="sortColumn">The column to sort by</param>
        /// <param name="pageNumber">The current page number</param>
        /// <param name="sortOrder">The sort order - "asc" or "desc"</param>
        /// <param name="searchString">The search string</param>
        /// <param name="perPageCount">The number of contracts to display per page</param>
        /// <returns>The view with the paginated list of contracts.</returns>
        public async Task<IActionResult> Index(string sortColumn, int pageNumber = 1, string sortOrder = "asc", string searchString = "", int perPageCount = 10)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            ViewBag.SortColumn = sortColumn;
            ViewBag.PageNumber = pageNumber;

            var contracts = await ContractService.FilterAsync(searchString, sortOrder, sortColumn);

            return View(PaginatedList<Contract>.Create(contracts, pageNumber, perPageCount));
        }

        /// <summary>
        /// Displays the details of a contract and its related consultants.
        /// </summary>
        /// <param name="id">The ID of the contract.</param>
        /// <param name="pageNumber">The current page number for consultants pagination.</param>
        /// <param name="perPageCount">The number of consultants to display per page.</param>
        /// <returns>The view with the contract details.</returns>
        public async Task<IActionResult> Details(int id, int pageNumber = 1, int perPageCount = 5)
        {
            var contract = await ContractService.GetAsync(id, false);

            if (contract is null)
            {
                return NotFound();
            }

            var con = PaginatedList<Consultant>.Create(contract.Consultants.ToList(), pageNumber, perPageCount);

            return View(new ContractDetailsViewModel(contract, con));
        }

        /// <summary>
        /// Displays the view with the form for creating a new contract.
        /// </summary>
        /// <returns>The create view</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new contract.
        /// </summary>
        /// <param name="viewModel">The contract create view model</param>
        /// <returns>Redirect to the index action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel viewModel)
        {
            if (viewModel.ConsultantIds is null)
            {
                ModelState.AddModelError("ConsultantIds", "Alespoň jeden poradce musí být vybrán");
            }

            if (viewModel.ClientId is null)
            {
                ModelState.AddModelError("ClientId", "Klient musí být vybrán");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var consultants = await ContractService.GetConsultantsAsync(viewModel.ConsultantIds!.Distinct().ToList());
            var client = await ContractService.GetClientAsync(viewModel.ClientId!.Value);

            if (consultants.Count == 0 || client is null)
            {
                return NotFound();
            }

            var contract = new Contract
            {
                EvidenceNumber = viewModel.EvidenceNumber,
                Institution = viewModel.Institution,
                DateSigned = viewModel.DateSigned,
                DateValidStart = viewModel.DateValidStart,
                DateValidEnd = viewModel.DateValidEnd,
                Client = client,
                Consultants = consultants
            };

            TempData["AlertMessage"] = "Smlouva vytvořena.";
            await ContractService.CreateAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Updates an existing contract.
        /// </summary>
        /// <param name="id">The ID of the contract to update.</param>
        /// <param name="viewModel">The view model containing contract details.</param>
        /// <returns>Redirect to the details view.</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await ContractService.GetAsync(id, false);

            if (contract is null)
            {
                return NotFound();
            }

            var viewModel = new ContractCreateViewModel
            {
                EvidenceNumber = contract.EvidenceNumber,
                Institution = contract.Institution,
                DateSigned = contract.DateSigned,
                DateValidStart = contract.DateValidStart,
                DateValidEnd = contract.DateValidEnd,
                ClientId = contract.ClientId,
            };

            ViewBag.Consultants = contract.Consultants.Select(t => new { t.ConsultantId, t.FullName })
               .ToDictionary(t => t.ConsultantId, t => t.FullName);
            ViewBag.ClientName = contract.Client.FullName;
            return View(viewModel);
        }

        /// <summary>
        /// Updates an existing contract.
        /// </summary>
        /// <param name="id">The ID of the contract to update.</param>
        /// <param name="viewModel">The view model containing contract details.</param>
        /// <returns>Redirect to the details view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractCreateViewModel viewModel)
        {
            if (viewModel.ConsultantIds is null)
            {
                ModelState.AddModelError("ConsultantIds", "Alespoň jeden poradce musí být vybrán");
            }

            if (viewModel.ClientId is null)
            {
                ModelState.AddModelError("ClientId", "Klient musí být vybrán");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var consultants = await ContractService.GetConsultantsAsync(viewModel.ConsultantIds!.Distinct().ToList());
            var client = await ContractService.GetClientAsync(viewModel.ClientId!.Value);
            var currentContract = await ContractService.GetAsync(id);

            if (consultants.Count == 0 || client is null || currentContract is null)
            {
                return NotFound();
            }

            var newContract = new Contract
            {
                ContractId = id,
                EvidenceNumber = viewModel.EvidenceNumber,
                Institution = viewModel.Institution,
                DateSigned = viewModel.DateSigned,
                DateValidStart = viewModel.DateValidStart,
                DateValidEnd = viewModel.DateValidEnd,
                Client = client,
                ClientId = client.ClientId,
                Consultants = consultants
            };

            try
            {
                await ContractService.UpdateAsync(currentContract, newContract);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Záznam byl v mezičase změněm, prosím proveďte úpravy znovu.");
                return View(viewModel);
            }

            TempData["AlertMessage"] = "Smlouva upravena.";
            return RedirectToAction(nameof(Details), new RouteValueDictionary { { "id", id } });
        }

        /// <summary>
        /// Deletes a contract.
        /// </summary>
        /// <param name="id">The ID of the contract to delete.</param>
        /// <returns>Redirect to the index action.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await ContractService.GetAsync(id);

            if (contract is null)
            {
                return NotFound();
            }

            TempData["AlertMessage"] = "Smlouva smazána.";
            await ContractService.RemoveAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retrieves information based on the specified entity and search term using AJAX.
        /// </summary>
        /// <param name="entity">The entity to search for - "consultant" or "client"</param>
        /// <param name="searchTerm">The search string</param>
        /// <returns>Json result of entity related information</returns>
        public async Task<IActionResult> SearchFor(string entity, string searchTerm = "")
        {
            return entity switch
            {
                "client" => Json(await ContractService.FilterClientsAsync(searchTerm)),
                "consultant" => Json(await ContractService.FilterConsultantsAsync(searchTerm)),
                _ => BadRequest(),
            };
        }

        /// <summary>
        /// Downloads the list of contracts as a CSV file.
        /// </summary>
        /// <returns>The file content result with the CSV data</returns>
        public async Task<FileContentResult> Download()
        {
            var csv = await ContractService.ExportCSVAsync();

            return File(Encoding.Latin1.GetBytes(csv), "text/csv", "Smlouvy.csv");
        }
    }
}