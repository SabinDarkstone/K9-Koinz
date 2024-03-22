using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public interface ITagRepository : IGenericRepository<Tag> {
        Task<IEnumerable<Tag>> GetAll();
        Task<Tag> GetDetails(Guid id);
    }
}