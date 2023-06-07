#nullable enable
using InsuranceApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InsuranceApp.Services
{
    /// <summary>
    /// Helper service class for managing consultant and its relations in database.
    /// </summary>
    public class ConsultantService : IConsultantService
    {
        private readonly InsuranceAppDbContext _db;

        public ConsultantService(InsuranceAppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Retrieves a consultant by <paramref name="id"/> from the database.
        /// Has an option to disable tracking via setting <paramref name="track"/> to false.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the consultant.</param>
        /// <param name="track">Indicates whether to <paramref name="track"/> possible upcoming changes.</param>
        /// <returns>The consultant or null if not found.</returns>
        public async Task<Consultant?> GetAsync(int id, bool track = true)
        {
            var consultant = _db.Consultants
                .Include(ct => ct.Contracts)
                .ThenInclude(cl => cl.Client);

            if (!track)
            {
                consultant.AsNoTracking();
            }

            return await consultant.FirstOrDefaultAsync(ct => ct.ConsultantId == id);
        }

        /// <summary>
        /// Filters consultants based on <paramref name="search"/> criteria, sorting <paramref name="order"/>, and <paramref name="column"/> from the database.
        /// </summary>
        /// <param name="search">The string to filter by.</param>
        /// <param name="order">The sort <paramref name="order"/>.</param>
        /// <param name="column">The <paramref name="column"/> to sort by.</param>
        /// <returns>A list of consultants filtered by <paramref name="search"/>.</returns>
        public async Task<List<Consultant>> FilterAsync(string search, string order, string column)
        {
            var consultants = _db.Consultants.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                consultants = consultants.Where(c => c.LastName.Contains(search)
                                           || c.FirstName.Contains(search)
                                           || c.Email.Contains(search)
                                           || c.Phone.Contains(search));
            }

            consultants = column switch
            {
                "FirstName" => order.ToLower() == "desc" ? consultants.OrderByDescending(c => c.FirstName) : consultants.OrderBy(c => c.FirstName),
                "LastName" => order.ToLower() == "desc" ? consultants.OrderByDescending(c => c.LastName) : consultants.OrderBy(c => c.LastName),
                "Age" => order.ToLower() == "desc" ? consultants.OrderByDescending(c => c.Age) : consultants.OrderBy(c => c.Age),
                _ => consultants.OrderBy(c => c.ConsultantId),
            };

            return await consultants.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Creates a record of the given <paramref name="consultant"/> in the database.
        /// </summary>
        /// <param name="consultant">The <paramref name="consultant"/> to be created.</param>
        public async Task CreateAsync(Consultant consultant)
        {
            await _db.Consultants.AddAsync(consultant);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates a record of the given <paramref name="original"/> consultant and its relations in the database using the <paramref name="updated"/> consultant.
        /// </summary>
        /// <param name="original">The <paramref name="original"/> consultant.</param>
        /// <param name="updated">The <paramref name="updated"/> consultant.</param>
        public async Task UpdateAsync(Consultant original, Consultant updated)
        {
            _db.Entry(original).CurrentValues.SetValues(updated);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the given <paramref name="consultant"/> and associated contracts from the database.
        /// </summary>
        /// <param name="consultant">The <paramref name="consultant"/> to be removed.</param>
        public async Task RemoveAsync(Consultant consultant)
        {
            if (consultant?.Contracts is not null)
            {
                foreach (var contract in consultant.Contracts)
                {
                    var contractEntity = await _db.Contracts.Include(ct => ct.Consultants).FirstOrDefaultAsync(c => c.ContractId == contract.ContractId);
                    if (contractEntity!.Consultants.Count <= 1)
                    {
                        _db.Remove(contractEntity);
                    }
                }
            }
            _db.Remove(consultant!);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Exports consultants as CSV via looping.
        /// </summary>
        /// <returns>A string containing the CSV data.</returns>
        public async Task<string> ExportCSVAsync()
        {
            var consultants = await _db.Consultants.Select(c => new string[]
            {
            c.ConsultantId.ToString(),
            c.FirstName,
            c.LastName,
            c.Age.ToString(),
            c.BirthNumber
            }).AsNoTracking().ToListAsync();

            StringBuilder sb = new();
            for (int i = 0; i < consultants.Count; i++)
            {
                string[] consultant = consultants[i];
                for (int j = 0; j < consultant.Length; j++)
                {
                    sb.Append(consultant[j] + ',');
                }

                sb.Append(Environment.NewLine);
            }

            sb.Insert(0, "ConsultantId, FirstName, LastName, Age, BirthNumber" + Environment.NewLine);

            return sb.ToString();
        }
    }

    public interface IConsultantService
    {
        Task<Consultant?> GetAsync(int consultantId, bool track = true);
        Task<List<Consultant>> FilterAsync(string searchTerm, string order, string column);

        Task CreateAsync(Consultant consultant);
        Task UpdateAsync(Consultant original, Consultant updated);
        Task RemoveAsync(Consultant consultant);

        Task<string> ExportCSVAsync();
    }
}
