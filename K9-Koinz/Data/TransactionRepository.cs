using K9_Koinz.Models;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Services;
using K9_Koinz.Triggers;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Data {
    public class TransactionRepository : TriggeredRepository<Transaction> {

        private readonly IDupeCheckerService<Transaction> _dupeChecker;

        public TransactionRepository(KoinzContext context, ITrigger<Transaction> trigger, IDupeCheckerService<Transaction> dupeChecker)
            : base(context, trigger) {
            _dupeChecker = dupeChecker;
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
                    var goodForSavings = CheckIfAvailableForSavingsGoal(t) || t.IsSavingsSpending;
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
