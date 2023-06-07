using Microsoft.EntityFrameworkCore;

namespace InsuranceApp.Services
{
    /// <summary>
    /// Helper service class for getting statistical information about records in database.
    /// </summary>
    public class HomeService : IHomeService
    {
        private readonly InsuranceAppDbContext _db;

        public HomeService(InsuranceAppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Retrieves a consultants predefined informations like total count.
        /// </summary>
        /// <returns>Dictionary of statistical information</returns>
        public async Task<IDictionary<string, int>> GetConsultantsInfoAsync()
        {
            Dictionary<string, int> result = new();
            var consultants = _db.Consultants.AsNoTracking();

            result.Add("count", await consultants.CountAsync());
            result.Add("oldest", await consultants.Include(cl => cl.Contracts).Where(con => con.Age > 60 && con.Contracts.Count > 1).CountAsync());
            result.Add("without", await consultants.Where(c => c.Contracts.Count == 0).CountAsync());

            return result;
        }

        /// <summary>
        /// Retrieves a clients predefined informations like total count.
        /// </summary>
        /// <returns>Dictionary of statistical information</returns>
        public async Task<IDictionary<string, int>> GetClientsInfoAsync()
        {
            Dictionary<string, int> result = new();
            var clients = _db.Clients.AsNoTracking();

            result.Add("count", await clients.CountAsync());
            result.Add("contracts", await clients.CountAsync(c => c.Contracts.Count > 3));
            result.Add("without", await clients.CountAsync(c => c.Contracts.Count == 0));

            return result;
        }

        /// <summary>
        /// Retrieves a contracts predefined informations like total count.
        /// </summary>
        /// <returns>Dictionary of statistical information</returns>
        public async Task<IDictionary<string, int>> GetContractsInfoAsync()
        {
            Dictionary<string, int> result = new();
            var contracts = _db.Contracts.AsNoTracking();

            result.Add("count", await contracts.CountAsync());
            result.Add("valid", await contracts.CountAsync(c => c.DateValidEnd > DateTime.Now));
            result.Add("moreThan1", await contracts.CountAsync(c => c.Consultants.Count > 1));

            return result;
        }
    }
    public interface IHomeService
    {
        Task<IDictionary<string, int>> GetConsultantsInfoAsync();
        Task<IDictionary<string, int>> GetClientsInfoAsync();
        Task<IDictionary<string, int>> GetContractsInfoAsync();
    }
}
