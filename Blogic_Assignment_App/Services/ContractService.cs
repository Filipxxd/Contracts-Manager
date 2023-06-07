#nullable enable
using InsuranceApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InsuranceApp.Services
{
    /// <summary>
    /// Helper service class for managing contract and its relations in database.
    /// </summary>
    public class ContractService : IContractService
    {

        private readonly InsuranceAppDbContext _db;

        public ContractService(InsuranceAppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Retrieves a contract by <paramref name="id"/> from database.
        /// Has an option to disable tracking via setting <paramref name="track"/> to false.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the contract.</param>
        /// <param name="track">Indicates whether to <paramref name="track"/> possible upcoming changes</param>
        /// <returns>The contract or null if not found</returns>
        public async Task<Contract?> GetAsync(int id, bool track = true)
        {
            var contracts = _db.Contracts
                .Include(ct => ct.Client)
                .Include(cl => cl.Consultants);

            if (!track)
            {
                contracts.AsNoTracking();
            }

            return await contracts.FirstOrDefaultAsync(ct => ct.ContractId == id);
        }

        /// <summary>
        /// Retrieves a client by <paramref name="id"/> from database.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the client</param>
        /// <returns>The client or null if not found</returns>
        public async Task<Client?> GetClientAsync(int id)
        {
            return await _db.Clients.FirstOrDefaultAsync(cl => cl.ClientId == id);
        }

        /// <summary>
        /// Retrieves a collection of consultants by their <paramref name="ids"/> from database.
        /// </summary>
        /// <param name="ids">The <paramref name="ids"/> of the consultants</param>
        /// <returns>A collection of consultants</returns>
        public async Task<ICollection<Consultant>> GetConsultantsAsync(List<int> ids)
        {
            return await _db.Consultants.Include(cn => cn.Contracts).Where(cn => ids.Contains(cn.ConsultantId)).ToListAsync();
        }

        /// <summary>
        /// Creates a record of given <paramref name="contract"/> in database.
        /// </summary>
        /// <param name="contract">The <paramref name="contract"/> to be made</param>
        public async Task CreateAsync(Contract contract)
        {
            await _db.Contracts.AddAsync(contract);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Filters contracts based on <paramref name="search"/> criteria, sorting <paramref name="order"/>, and <paramref name="column"/> from database.
        /// </summary>
        /// <param name="search">The string to filter by</param>
        /// <param name="order">The sort <paramref name="order"/></param>
        /// <param name="column">The <paramref name="column"/> to sort by</param>
        /// <returns>A list of contracts filtered by <paramref name="search"/></returns>
        public async Task<List<Contract>> FilterAsync(string search, string order, string column)
        {
            var contracts = _db.Contracts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                contracts = contracts.Where(c => c.Institution.Contains(search)
                                           || c.EvidenceNumber.Contains(search));
            }

            contracts = column switch
            {
                "EvidenceNumber" => order.ToLower() == "desc" ? contracts.OrderByDescending(c => c.EvidenceNumber) : contracts.OrderBy(c => c.EvidenceNumber),
                "Institution" => order.ToLower() == "desc" ? contracts.OrderByDescending(c => c.Institution) : contracts.OrderBy(c => c.Institution),
                "DateValidEnd" => order.ToLower() == "desc" ? contracts.OrderByDescending(c => c.DateValidEnd) : contracts.OrderBy(c => c.DateValidEnd),
                _ => contracts.OrderBy(c => c.ContractId),
            };

            return await contracts.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Filters consultants based on <paramref name="searchTerm"/> from database.
        /// Has an option to select precie value via <paramref name="count"/>.
        /// </summary>
        /// <param name="searchTerm">The string to filter by</param>
        /// <param name="count">The number of consultants to retrieve</param>
        /// <returns>A dictionary of consultant IDs and names</returns>
        public async Task<Dictionary<int, string>> FilterConsultantsAsync(string searchTerm, int count = 5)
        {
            return await _db.Consultants
                            .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm) || c.Email.Contains(searchTerm))
                            .Take(count)
                            .ToDictionaryAsync(c => c.ConsultantId, c => c.FullName);
        }

        /// <summary>
        /// Filters clients based on <paramref name="searchTerm"/> from database.
        /// Has an option to select precie value via <paramref name="count"/>.
        /// </summary>
        /// <param name="searchTerm">The string to filter by</param>
        /// <param name="count">The number of clients to retrieve</param>
        /// <returns>A dictionary of client IDs and names</returns>
        public async Task<Dictionary<int, string>> FilterClientsAsync(string searchTerm, int count = 5)
        {
            return await _db.Clients
                            .Where(c => c.FirstName.Contains(searchTerm) || c.LastName.Contains(searchTerm) || c.Email.Contains(searchTerm))
                            .Take(count)
                            .ToDictionaryAsync(c => c.ClientId, c => c.FullName);
        }

        /// <summary>
        /// Updates a record of given <paramref name="original"/> contract and its relations in database using the <paramref name="updated"/> contract.
        /// </summary>
        /// <param name="orig">The <paramref name="original"/> contract</param>
        /// <param name="updated">The <paramref name="updated"/> contract</param>
        public async Task UpdateAsync(Contract original, Contract updated)
        {
            _db.Entry(original).CurrentValues.SetValues(updated);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a <paramref name="contract"/> and its relations from database.
        /// </summary>
        /// <param name="contract">The <paramref name="contract"/> to remove</param>
        public async Task RemoveAsync(Contract contract)
        {
            _db.Remove(contract);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Exports contracts as CSV via looping.
        /// </summary>
        /// <returns>A string containing the CSV data</returns>
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

            sb.Insert(0, "ContractId, EvidenceNumber, Institution, DateSigned, DateValidStart, DateValidEnd" + Environment.NewLine);

            return sb.ToString();
        }
    }

    public interface IContractService
    {
        Task<Contract?> GetAsync(int contractId, bool track = true);
        Task<Client?> GetClientAsync(int clientId);
        Task<ICollection<Consultant>> GetConsultantsAsync(List<int> consultantIds);

        Task<Dictionary<int, string>> FilterConsultantsAsync(string searchTerm, int count = 5);
        Task<Dictionary<int, string>> FilterClientsAsync(string searchTerm, int count = 5);
        Task<List<Contract>> FilterAsync(string search, string order, string column);

        Task CreateAsync(Contract contract);
        Task UpdateAsync(Contract original, Contract updated);
        Task RemoveAsync(Contract contract);

        Task<string> ExportCSVAsync();
    }
}