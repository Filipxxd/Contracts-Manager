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
    /// Controller for managing consultant based actions.
    /// </summary>
    public class ConsultantsController : Controller
    {
        private readonly IConsultantService ConsultantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IConsultantService"/>.
        /// </summary>
        /// <param name="consultantService">The consultant service</param>
        public ConsultantsController(IConsultantService consultantService)
        {
            ConsultantService = consultantService;
        }

        /// <summary>
        /// Displays a paginated list of clients with sorting and searching options.
        /// </summary>
        /// <param name="sortColumn">The column to sort by</param>
        /// <param name="pageNumber">The current page number</param>
        /// <param name="sortOrder">The sort order - "asc" or "desc"</param>
        /// <param name="searchString">The search string</param>
        /// <param name="perPageCount">The number of clients to display per page</param>
        /// <returns>The view with the paginated list of consultants.</returns>
        public async Task<IActionResult> Index(string sortColumn, int pageNumber = 1, string sortOrder = "asc", string searchString = "", int perPageCount = 10)
        {
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            ViewBag.SortColumn = sortColumn;
            ViewBag.PageNumber = pageNumber;

            var consultants = await ConsultantService.FilterAsync(searchString, sortOrder, sortColumn);

            return View(PaginatedList<Consultant>.Create(consultants, pageNumber, perPageCount));

        }

        /// <summary>
        /// Displays the details of a consultant and their contracts.
        /// </summary>
        /// <param name="id">The ID of the consultant.</param>
        /// <param name="pageNumber">The current page number for contracts pagination.</param>
        /// <param name="perPageCount">The number of contracts to display per page.</param>
        /// <returns>The view with the consultant details.</returns>
        public async Task<IActionResult> Details(int id, int pageNumber = 1, int perPageCount = 5)
        {
            var consultant = await ConsultantService.GetAsync(id, false);

            if (consultant is null)
            {
                return NotFound();
            }

            var contractsList = PaginatedList<Contract>.Create(consultant.Contracts.ToList(), pageNumber, perPageCount);

            return View(new ConsultantDetailsViewModel(consultant, contractsList));
        }

        /// <summary>
        /// Displays the view with form for creating a new consultant.
        /// </summary>
        /// <returns>The create view</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new consultant.
        /// </summary>
        /// <param name="consultant">The consultant to create</param>
        /// <returns>Redirect to the index action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Consultant consultant)
        {
            if (!ModelState.IsValid)
            {
                return View(consultant);
            }

            TempData["AlertMessage"] = "Poradce vytvořen.";
            await ConsultantService.CreateAsync(consultant);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the view with form for editing an existing consultant.
        /// </summary>
        /// <param name="id">The ID of the consultant to edit</param>
        /// <returns>The edit view with current consultant values</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var consultant = await ConsultantService.GetAsync(id, false);

            if (consultant is null)
            {
                return NotFound();
            }
            return View(consultant);
        }

        /// <summary>
        /// Updates an existing consultant.
        /// </summary>
        /// <param name="id">The ID of the consultant to update</param>
        /// <param name="updated">The updated consultant</param>
        /// <returns>Redirect to the details of the updated consultant</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Consultant updated)
        {
            if (!ModelState.IsValid)
            {
                return View(updated);

            }
            if (updated.ConsultantId != id)
            {
                return BadRequest();
            }

            var original = await ConsultantService.GetAsync(id);

            if (original is null)
            {
                return NotFound();
            }

            try
            {
                await ConsultantService.UpdateAsync(original, updated);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Záznam byl v mezičase změněm, prosím proveďte úpravy znovu.");
                return View(updated);
            }

            TempData["AlertMessage"] = "Poradce upraven.";
            return RedirectToAction(nameof(Details), new RouteValueDictionary { { "id", id } });
        }

        /// <summary>
        /// Deletes a consultant.
        /// </summary>
        /// <param name="id">The ID of the consultant to delete</param>
        /// <returns>Redirect to the index action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var consultant = await ConsultantService.GetAsync(id);

            if (consultant is null)
            {
                return NotFound();
            }

            TempData["AlertMessage"] = "Poradce odstraněn.";
            await ConsultantService.RemoveAsync(consultant);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Downloads the list of consultants as a CSV file.
        /// </summary>
        /// <returns>The file content result with the CSV data</returns>
        public async Task<FileContentResult> Download()
        {
            string csv = await ConsultantService.ExportCSVAsync();

            return File(Encoding.Latin1.GetBytes(csv), "text/csv", "Poradci.csv");
        }
    }
}
