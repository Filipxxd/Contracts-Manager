#nullable enable
using InsuranceApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InsuranceApp.Services
{
    /// <summary>
    /// Helper service class for managing client and its relations in database.
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly InsuranceAppDbContext _db;

        public ClientService(InsuranceAppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Retrieves a client by <paramref name="id"/> from the database.
        /// Has an option to disable tracking via setting <paramref name="track"/> to false.
        /// </summary>
        /// <param name="id">The <paramref name="id"/> of the client.</param>
        /// <param name="track">Indicates whether to <paramref name="track"/> possible upcoming changes.</param>
        /// <returns>The client or null if not found.</returns>
        public async Task<Client?> GetAsync(int id, bool track = true)
        {
            var client = _db.Clients
                .Include(ct => ct.Contracts)
                .ThenInclude(cn => cn.Consultants);

            if (!track)
            {
                client.AsNoTracking();
            }

            return await client.FirstOrDefaultAsync(ct => ct.ClientId == id);
        }

        /// <summary>
        /// Filters clients based on <paramref name="search"/> criteria, sorting <paramref name="order"/>, and <paramref name="column"/> from the database.
        /// </summary>
        /// <param name="search">The string to filter by.</param>
        /// <param name="order">The sort <paramref name="order"/>.</param>
        /// <param name="column">The <paramref name="column"/> to sort by.</param>
        /// <returns>A list of clients filtered by <paramref name="search"/>.</returns>
        public async Task<List<Client>> FilterAsync(string search, string order, string column)
        {
            var clients = _db.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                clients = clients.Where(c => c.LastName.Contains(search)
                                           || c.FirstName.Contains(search)
                                           || c.Email.Contains(search)
                                           || c.Phone.Contains(search));
            }

            clients = column switch
            {
                "FirstName" => order.ToLower() == "desc" ? clients.OrderByDescending(c => c.FirstName) : clients.OrderBy(c => c.FirstName),
                "LastName" => order.ToLower() == "desc" ? clients.OrderByDescending(c => c.LastName) : clients.OrderBy(c => c.LastName),
                "Age" => order.ToLower() == "desc" ? clients.OrderByDescending(c => c.Age) : clients.OrderBy(c => c.Age),
                _ => clients.OrderBy(c => c.ClientId),
            };

            return await clients.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Creates a record of the given <paramref name="client"/> in the database.
        /// </summary>
        /// <param name="client">The <paramref name="client"/> to be created.</param>
        public async Task CreateAsync(Client client)
        {
            await _db.Clients.AddAsync(client);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates a record of the given <paramref name="original"/> client and its relations in the database using the <paramref name="updated"/> client.
        /// </summary>
        /// <param name="original">The <paramref name="original"/> client.</param>
        /// <param name="updated">The <paramref name="updated"/> client.</param>
        public async Task UpdateAsync(Client original, Client updated)
        {
            _db.Entry(original).CurrentValues.SetValues(updated);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a <paramref name="client"/> and its relations from the database.
        /// </summary>
        /// <param name="client">The <paramref name="client"/> to be removed.</param>
        public async Task RemoveAsync(Client client)
        {
            _db.Remove(client);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Exports clients as CSV via looping.
        /// </summary>
        /// <returns>A string containing the CSV data.</returns>
        public async Task<string> ExportCSVAsync()
        {
            var clients = await _db.Clients.Select(c => new string[]
            {
            c.ClientId.ToString(),
            c.FirstName,
            c.LastName,
            c.Age.ToString(),
            c.BirthNumber
            }).AsNoTracking().ToListAsync();

            StringBuilder sb = new();
            for (int i = 0; i < clients.Count; i++)
            {
                string[] client = clients[i];
                for (int j = 0; j < client.Length; j++)
                {
                    sb.Append(client[j] + ',');
                }

                sb.Append(Environment.NewLine);
            }

            sb.Insert(0, "ClientId, FirstName, LastName, Age, BirthNumber" + Environment.NewLine);

            return sb.ToString();
        }
    }


    public interface IClientService
    {
        Task<Client?> GetAsync(int clientId, bool track = true);
        Task<List<Client>> FilterAsync(string searchTerm, string order, string column);

        Task CreateAsync(Client client);
        Task UpdateAsync(Client original, Client updated);
        Task RemoveAsync(Client client);

        Task<string> ExportCSVAsync();
    }
}
