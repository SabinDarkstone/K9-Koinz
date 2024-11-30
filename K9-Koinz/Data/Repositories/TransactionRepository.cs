using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Pages;
using K9_Koinz.Services;
using K9_Koinz.Triggers;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using System.Linq;

namespace K9_Koinz.Data.Repositories {
    public class TransactionRepository : TriggeredRepository<Transaction> {

        private readonly IDupeCheckerService<Transaction> _dupeChecker;

        public TransactionRepository(KoinzContext context, ITrigger<Transaction> trigger, IDupeCheckerService<Transaction> dupeChecker)
            : base(context, trigger) {
            _dupeChecker = dupeChecker;
        }

        public async Task<PaginatedList<Transaction>> SearchTransactions(List<Guid> categoryIds, Guid merchantId,
            Guid accountId, Guid tagId, string searchString, bool? hideTransfers, DateTime? startDate, DateTime? endDate,
            string sortOrder, int? pageIndex, int pageSize) {
            var transactionsIQ = _context.Transactions
                .AsNoTracking()
                .Include(trans => trans.Category)
                    .ThenInclude(cat => cat.ParentCategory)
                .AsQueryable();

            if (categoryIds.Count > 0) {
                transactionsIQ = transactionsIQ.Where(trans => trans.CategoryId.HasValue && categoryIds.Contains(trans.CategoryId.Value));
                transactionsIQ = transactionsIQ.Where(trans => !trans.IsSplit);
            } else {
                transactionsIQ = transactionsIQ.Where(trans => !trans.ParentTransactionId.HasValue);
            }

            if (merchantId != Guid.Empty) {
                transactionsIQ = transactionsIQ.Where(trans => trans.MerchantId == merchantId);
            }

            if (accountId != Guid.Empty) {
                transactionsIQ = transactionsIQ.Where(trans => trans.AccountId == accountId);
            }

            if (tagId != Guid.Empty) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == tagId);
            }

            if (startDate.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Date.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Date.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(searchString)) {
                if (double.TryParse(searchString, out double value)) {
                    transactionsIQ = transactionsIQ.Where(trans => trans.Amount == value || trans.Amount == -1 * value);
                } else if (searchString.Equals("hidden", StringComparison.CurrentCultureIgnoreCase)) {
                    transactionsIQ = transactionsIQ.Where(trans => trans.IsSavingsSpending);
                } else {
                    var lcSearchString = searchString.ToLower();
                    transactionsIQ = transactionsIQ.Where(trans => trans.Notes.ToLower().Contains(lcSearchString) ||
                        trans.AccountName.ToLower().Contains(lcSearchString) || trans.CategoryName.ToLower().Contains(lcSearchString) ||
                        trans.MerchantName.ToLower().Contains(lcSearchString) || trans.SavingsGoalName.ToLower().Contains(lcSearchString));
                }
            }

            if (hideTransfers.HasValue && hideTransfers.Value) {
                transactionsIQ = transactionsIQ.Include(trans => trans.Category)
                    .Where(trans => trans.Category.CategoryType != CategoryType.TRANSFER);
            }

            switch (sortOrder) {
                case "Merchant":
                    transactionsIQ = transactionsIQ.OrderBy(trans => trans.MerchantName);
                    break;
                case "merchant_desc":
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => trans.MerchantName);
                    break;
                case "Amount":
                    transactionsIQ = transactionsIQ.OrderBy(trans => Math.Abs(trans.Amount));
                    break;
                case "amount_desc":
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => Math.Abs(trans.Amount));
                    break;
                case "Date":
                    transactionsIQ = transactionsIQ.OrderBy(trans => trans.Date);
                    break;
                case "date_desc":
                default:
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => trans.Date);
                    break;
            }

            transactionsIQ = transactionsIQ.Include(trans => trans.Tag);
            return await PaginatedList<Transaction>.CreateAsync(transactionsIQ.AsSplitQuery(), pageIndex ?? 1, pageSize);
        }

        public async Task<Transaction> GetTransactionWithDetailsById(Guid id) {
            return await _context.Transactions
                .Include(trans => trans.Tag) 
                .Include(trans => trans.Bill)
                .Include(trans => trans.SplitTransactions)
                    .ThenInclude(splt => splt.Tag)
                .Include(trans => trans.Category)
                .Include(trans => trans.Transfer)
                    .ThenInclude(fer => fer.RecurringTransfer)
                .Include(trans => trans.SavingsGoal)
                .SingleOrDefaultAsync(trans => trans.Id == id);
        }

        public async Task<List<Transaction>> GetChildTransactions(Guid parentId) {
            return await _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.ParentTransactionId == parentId)
                .OrderBy(trans => trans.CategoryName)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetRecentMerchantTransactions(Guid merchantId, int count) {
            return await _context.Transactions
                .AsNoTracking()
                .Where(trans => trans.MerchantId == merchantId)
                .OrderByDescending(trans => trans.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Transaction>> CreateSplitTransaction(List<Transaction> splitTransactions) {
            var parentTransaction = await GetTransactionWithDetailsById(splitTransactions[0].ParentTransactionId.Value);

            var isTransfer = parentTransaction.TransferId.HasValue;
            var validSplits = splitTransactions.Where(splt => splt.Amount != 0).ToList();

            foreach (var split in validSplits) {
                split.Date = parentTransaction.Date;
            }

            if (validSplits.Count() > 0) {
                if (!isTransfer) {
                    // Set data on the parent transaction for non-transfers
                    parentTransaction.CategoryName = "Multiple";
                    parentTransaction.IsSplit = true;
                    parentTransaction.SplitTransactions = validSplits;
                } else {
                    // Unallocate parent transcation since child transactions will be allocated to savings goals
                    parentTransaction.IsSavingsSpending = false;
                    parentTransaction.SavingsGoalId = null;
                    parentTransaction.SavingsGoalName = string.Empty;

                    var transfer = await _context.Transfers.FindAsync(parentTransaction.TransferId);
                    transfer.IsSplit = true;

                    _context.Transfers.Update(transfer);
                }
            }

            var saveResult = AddManyAsync(validSplits);

            return new List<Transaction> { parentTransaction }.Concat(validSplits).ToList();
        }

        public override TriggerActionResult BeforeSave(TriggerType triggerType, IEnumerable<Transaction> oldList, IEnumerable<Transaction> newList) {
            var totalMatches = 0;
            if (triggerType == TriggerType.INSERT || triggerType == TriggerType.UPDATE) {
                foreach (var t in newList) {
                    var matches = _dupeChecker.FindPotentialDuplicates(t).Result;
                    totalMatches += matches.Count;
                }
            }

            var triggerResults = base.BeforeSave(triggerType, oldList, newList);

            if (totalMatches > 0) {
                return new TriggerActionResult {
                    Status = TriggerStatus.DUPLICATE_FOUND,
                    ErrorMessage = "Duplicate transaction found",
                    Exception = null
                };
            } else {
                return triggerResults;
            }
        }

        public override TriggerActionResult AfterSave(TriggerType triggerType, IEnumerable<Transaction> oldList, IEnumerable<Transaction> newList) {
            var triggerResults = base.AfterSave(triggerType, oldList, newList);

            var promptSavingsGoal = false;
            if (triggerType == TriggerType.INSERT || triggerType == TriggerType.UPDATE) {
                foreach (var t in newList) {
                    var goodForSavings = !t.ParentTransactionId.HasValue && (CheckIfAvailableForSavingsGoal(t) || t.IsSavingsSpending);
                    if (goodForSavings) {
                        promptSavingsGoal = true;
                        break;
                    }
                }
            }

            if (promptSavingsGoal) {
                return new TriggerActionResult {
                    Status = TriggerStatus.GO_SAVINGS,
                    ErrorMessage = "Transaction is good for savings goal",
                    Exception = null
                };
            } else {
                return triggerResults;
            }
        }

        private bool CheckIfAvailableForSavingsGoal(Transaction t) {
            if (t.Category.CategoryType != CategoryType.TRANSFER && t.Category.CategoryType != CategoryType.INCOME) {
                return false;
            }

            var account = _context.Accounts.Find(t.AccountId);
            if (account.Type != AccountType.SAVINGS && account.Type != AccountType.CHECKING) {
                return false;
            }

            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == t.AccountId);
            return accountHasGoals;
        }
    }
}
