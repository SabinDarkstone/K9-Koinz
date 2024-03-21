using K9_Koinz.Data;

namespace K9_Koinz.Pages.Meta {
    public class AbstractIndexModel<TEntity> : AbstractDbPage {
        public AbstractIndexModel(RepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public IEnumerable<TEntity> RecordList { get; set; } = default!;
    }
}
