using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Models.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class TransactionRepository : GenericRepository<Transaction> {
        public TransactionRepository(KoinzContext context)
            : base(context) { }

        public async Task<Transaction> GetDetailsAsync(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Bill)
                .Include(trans => trans.Tag)
                .Include(trans => trans.Category)
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.Transactions
                        .OrderByDescending(trans => trans.Date))
                .Include(trans => trans.SplitTransactions
                    .OrderBy(trans => trans.CategoryName))
                .SingleOrDefaultAsync(trans => trans.Id == id);
        }

        public double GetTransactionTotalSinceBalanceSet(Account account) {
            var runningTotal = _context.Transactions
                .Where(trans => trans.Date > account.InitialBalanceDate || (trans.Date.Date == account.InitialBalanceDate.Date && trans.DoNotSkip))
                .Where(trans => trans.AccountId == account.Id).GetTotal();

            return runningTotal;
        }

        public async Task<IEnumerable<Transaction>> GetByAccountId(Guid accountId) {
            return await _context.Transactions
                .Where(trans => trans.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetForSpendingHistory(Guid categoryId) {
            return await _context.Transactions
                .Include(trans => trans.Category)
                .AsNoTracking()
                .Where(trans => trans.CategoryId == categoryId || trans.Category.ParentCategoryId == categoryId)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .Where(trans => trans.Date <= DateTime.Today.Date.Date && trans.Date.Date >= DateTime.Today.AddMonths(-11))
                .ToListAsync();
        }

        public async Task<Transaction> GetSplitLines(Guid parentId) {
            return await _context.Transactions
                .Include(trans => trans.Category)
                .Include(trans => trans.SplitTransactions
                    .OrderBy(splt => splt.CategoryName))
                .Where(trans => trans.Id == parentId)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public bool AnyInMonth(DateTime refDate) {
            return _context.Transactions
                .AsNoTracking()
                .Any(trans => trans.Date >= refDate.StartOfMonth() && trans.Date <= refDate.EndOfMonth());
        }

        public bool AnyInWeek(DateTime refDate) {
            return _context.Transactions
                .AsNoTracking()
                .Any(trans => trans.Date >= refDate.StartOfWeek() && trans.Date <= refDate.EndOfWeek());
        }

        public bool AnyInYear(DateTime refDate) {
            return _context.Transactions
                .AsNoTracking()
                .Any(trans => trans.Date >= refDate.StartOfYear() && trans.Date <= refDate.EndOfYear());
        }

        public Transaction GetMatchingFromTransferPair(Guid transferId, Guid transactionId) {
            return _context.Transactions
                .Where(trans => trans.TransferId == transferId)
                .Where(trans => trans.Id != transactionId)
                .SingleOrDefault();
        }

        public Transaction GetWithCategory(Guid id) {
            return _context.Transactions
                .Include(trans => trans.Category)
                .Where(trans => trans.Id == id)
                .SingleOrDefault();
        }

        public async Task<PaginatedList<Transaction>> GetFiltered(TransactionFilterSetting filters) {
            IQueryable<Transaction> transIQ = _context.Transactions
                .Include(trans => trans.Tag)
                .AsNoTracking();

            // CATEGORY FILTER LOGIC
            if (filters.CategoryFilters != null && filters.CategoryFilters.Count > 0) {
                // Apply categories filter to the query
                transIQ = transIQ.Where(trans => trans.CategoryId.HasValue && filters.CategoryFilters.Contains(trans.CategoryId.Value));
            } else {
                // If no category is selected, do not show split transactions
                transIQ = transIQ.Where(trans => !trans.ParentTransactionId.HasValue);
            }

            // MERCHANT FILTER LOGIC
            if (filters.MerchantFilter.Value != Guid.Empty) {
                transIQ = transIQ.Where(trans => trans.MerchantId == filters.MerchantFilter);
            }

            // ACCOUNT FILTER LOGIC
            if (filters.AccountFilter.Value != Guid.Empty) {
                transIQ = transIQ.Where(trans => trans.AccountId == filters.AccountFilter);
            }

            // TAG FILTER LOGIC
            if (filters.TagFilter.Value != Guid.Empty) {
                transIQ = transIQ.Where(trans => trans.TagId == filters.TagFilter);
            }

            // DATE RANGE FILTERS
            if (filters.DateRangeStart.HasValue) {
                transIQ = transIQ.Where(trans => trans.Date.Date >= filters.DateRangeStart.Value.Date);
            }
            if (filters.DateRangeEnd.HasValue) {
                transIQ = transIQ.Where(trans => trans.Date.Date <= filters.DateRangeEnd.Value.Date);
            }

            // BASIC TEXT SEARCH LOGIC
            if (!string.IsNullOrWhiteSpace(filters.SearchString)) {
                if (float.TryParse(filters.SearchString, out float value)) {
                    transIQ = transIQ.Where(trans => trans.Amount == value || trans.Amount == -1 * value);
                } else {
                    var lcSearchString = filters.SearchString.ToLower();
                    transIQ = transIQ.Where(trans => trans.Notes.ToLower().Contains(lcSearchString) ||
                        trans.AccountName.ToLower().Contains(lcSearchString) || trans.CategoryName.ToLower().Contains(lcSearchString) ||
                        trans.MerchantName.ToLower().Contains(lcSearchString) || trans.SavingsGoalName.ToLower().Contains(lcSearchString));
                }
            }

            // TRANSFER FILTER LOGIC
            if (filters.HideTransfers.HasValue && filters.HideTransfers.Value) {
                transIQ = transIQ.Include(trans => trans.Category)
                    .Where(trans => trans.Category.CategoryType != CategoryType.TRANSFER);
            }

            // SORT ORDER LOGIC
            switch (filters.SortOrder) {
                case "Merchant":
                    transIQ = transIQ.OrderBy(trans => trans.MerchantName);
                    break;
                case "merchant_desc":
                    transIQ = transIQ.OrderByDescending(trans => trans.MerchantName);
                    break;
                case "Amount":
                    transIQ = transIQ.OrderBy(trans => Math.Abs(trans.Amount));
                    break;
                case "amount_desc":
                    transIQ = transIQ.OrderByDescending(trans => Math.Abs(trans.Amount));
                    break;
                case "Date":
                    transIQ = transIQ.OrderBy(trans => trans.Date);
                    break;
                case "date_desc":
                default:
                    transIQ = transIQ.OrderByDescending(trans => trans.Date);
                    break;
            }

            return await PaginatedList<Transaction>.CreateAsync(transIQ, filters.PageIndex ?? 1, 50);
        }

        public async Task<IEnumerable<Transaction>> GetByCategory(Guid categoryId) {
            return await _context.Transactions
                .Where(trans => trans.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByMerchant(Guid merchantId) {
            return await _context.Transactions
                .Where(trans => trans.MerchantId == merchantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> FindDuplicatesFromTransfer(Transaction[] pair) {
            var startDate = pair[1].Date.AddDays(-5);
            var endDate = pair[1].Date.AddDays(5);

            return await _context.Transactions
                .Where(trans => trans.AccountId == pair[0].AccountId)
                .Where(trans => trans.Amount == pair[0].Amount)
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .ToListAsync();
        }
    }
}
