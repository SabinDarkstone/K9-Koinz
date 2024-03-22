using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Utils;

namespace K9_Koinz.Data {
    public interface ITransactionRepository : IGenericRepository<Transaction> {
        bool AnyInMonth(DateTime refDate);
        bool AnyInWeek(DateTime refDate);
        bool AnyInYear(DateTime refDate);
        Task<IEnumerable<Transaction>> FindDuplicatesFromTransfer(Transaction[] pair);
        Task<IEnumerable<Transaction>> GetByAccountId(Guid accountId);
        Task<IEnumerable<Transaction>> GetByCategory(Guid categoryId);
        Task<IEnumerable<Transaction>> GetByMerchant(Guid merchantId);
        Task<Transaction> GetDetailsAsync(Guid id);
        Task<PaginatedList<Transaction>> GetFiltered(TransactionFilterSetting filters);
        Task<IEnumerable<Transaction>> GetForSpendingHistory(Guid categoryId);
        Task<IEnumerable<Transaction>> GetForTrendGraph(Guid categoryId, DateTime startDate, DateTime endDate);
        Transaction GetMatchingFromTransferPair(Guid transferId, Guid transactionId);
        Task<Transaction> GetSplitLines(Guid parentId);
        double GetTransactionTotalSinceBalanceSet(Account account);
        Task<Transaction> GetTransFromMostPopularCategoryByMerchant(string merchantId);
        Transaction GetWithCategory(Guid id);
    }
}