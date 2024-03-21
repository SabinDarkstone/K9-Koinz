using K9_Koinz.Models;

namespace K9_Koinz.Data {
    public class TagRepository : GenericRepository<Tag> {
        public TagRepository(KoinzContext context) : base(context) { }
    }
}