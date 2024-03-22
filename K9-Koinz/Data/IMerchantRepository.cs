using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;

namespace K9_Koinz.Data {
    public interface IMerchantRepository : IGenericRepository<Merchant> {
        Task<bool> DoesExistsByName(string name);
        Task<IEnumerable<Merchant>> GetAll();
        Task<Merchant> GetDetailsAsync(Guid id);
        Task<IEnumerable<AutocompleteResult>> GetForAutocomplete(string searchText);
    }
}