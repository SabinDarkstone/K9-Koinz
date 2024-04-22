using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {

    public interface IDupeCheckerService<TEntity> : ICustomService {
        public abstract Task<List<TEntity>> FindPotentialDuplicates(TEntity record);
    }

    public class TransactionDupeCheckerService : AbstractService<TransactionDupeCheckerService>,
        IDupeCheckerService<Transaction> {
        public TransactionDupeCheckerService(KoinzContext context, ILogger<TransactionDupeCheckerService> logger)
            : base(context, logger) { }

        public async Task<List<Transaction>> FindPotentialDuplicates(Transaction record) {
            var minDate = record.Date.AddDays(-3);
            var maxDate = record.Date.AddDays(3);

            var potentialMatches = await _context.Transactions
                .Where(trans => trans.Amount == record.Amount)
                .Where(trans => trans.AccountId == record.AccountId)
                .Where(trans => trans.Date >= minDate && trans.Date <= maxDate)
                .Where(trans => trans.Id != record.Id)
                .ToListAsync();

            return potentialMatches;
        }
    }
}
