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
    /// Controller for managing client based actions.
    /// </summary>
    public class ClientsController : Controller
    {
        private readonly IClientService ClientService;

        /// <summary>
        /// Initializes a new instance of the ClientsController class.
        /// </summary>
        /// <param name="clientService">Injected client service</param>
        public ClientsController(IClientService clientService)
        {
            ClientService = clientService;
        }

        /// <summary>
        /// Displays a paginated list of clients with sorting and searching options.
        /// </summary>
        /// <param name="sortColumn">The column to sort by</param>
        /// <param name="pageNumber">The current page number</param>
        /// <param name="sortOrder">The sort order - "asc" or "desc"</param>
        /// <param name="searchString">The search string</param>
        /// <param name="perPageCount">The number of clients to display per page</param>
        /// <returns>The view with the paginated list of clients</returns>
        public async Task<IActionResult> Index(string sortColumn, int pageNumber = 1, string sortOrder = "asc", string searchString = "", int perPageCount = 10)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            ViewBag.SortColumn = sortColumn;
            ViewBag.PageNumber = pageNumber;

            var clients = await ClientService.FilterAsync(searchString, sortOrder, sortColumn);

            return View(PaginatedList<Client>.Create(clients, pageNumber, perPageCount));

        }

        /// <summary>
        /// Displays the details of a client and their contracts.
        /// </summary>
        /// <param name="id">The ID of the client</param>
        /// <param name="pageNumber">The current page number of paginated contracts list</param>
        /// <param name="perPageCount">The number of contracts to display per page</param>
        /// <returns>The view with the client details and contracts</returns>
        public async Task<IActionResult> Details(int id, int pageNumber = 1, int perPageCount = 5)
        {
            var client = await ClientService.GetAsync(id, false);

            if (client is null)
            {
                return NotFound();
            }

            var contractsList = PaginatedList<Contract>.Create(client.Contracts.ToList(), pageNumber, perPageCount);

            return View(new ClientDetailsViewModel(client, contractsList));
        }

        /// <summary>
        /// Displays the view with form for creating a new client.
        /// </summary>
        /// <returns>The create view</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="client">The client to create</param>
        /// <returns>Redirect to the index action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            TempData["AlertMessage"] = "Klient vytvořen.";
            await ClientService.CreateAsync(client);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the view with form for editing an existing client.
        /// </summary>
        /// <param name="id">The ID of the client to edit</param>
        /// <returns>The edit view with current client values</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var client = await ClientService.GetAsync(id, false);

            if (client is null)
            {
                return NotFound();
            }
            return View(client);
        }

        /// <summary>
        /// Updates an existing client.
        /// </summary>
        /// <param name="id">The ID of the client to update</param>
        /// <param name="updated">The updated client data</param>
        /// <returns>Redirects to the details of the updated client</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client updated)
        {
            if (!ModelState.IsValid)
            {
                return View(updated);

            }
            if (updated.ClientId != id)
            {
                return BadRequest();
            }

            var original = await ClientService.GetAsync(id);

            if (original is null)
            {
                return NotFound();
            }

            try
            {
                await ClientService.UpdateAsync(original, updated);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Záznam byl v mezičase změněm, prosím proveďte úpravy znovu.");
                return View(updated);
            }

            TempData["AlertMessage"] = "Klient upraven.";
            return RedirectToAction(nameof(Details), new RouteValueDictionary { { "id", id } });
        }

        /// <summary>
        /// Deletes a client.
        /// </summary>
        /// <param name="id">The ID of the client to delete</param>
        /// <returns>Redirect to the index action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await ClientService.GetAsync(id);

            if (client is null)
            {
                return NotFound();
            }

            TempData["AlertMessage"] = "Klient odstraněn.";
            await ClientService.RemoveAsync(client);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Downloads the list of clients as a CSV file.
        /// </summary>
        /// <returns>The file content result with the CSV data</returns>
        public async Task<FileContentResult> Download()
        {
            string csv = await ClientService.ExportCSVAsync();

            return File(Encoding.Latin1.GetBytes(csv), "text/csv", "Klienti.csv");
        }
    }
}
