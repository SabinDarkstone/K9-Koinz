using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.ViewComponents {

    [ViewComponent(Name = "TransactionListModal")]
    public class TransactionListModalController : KoinzController<TransactionListModalController> {
        private List<Transaction> transactions = new();

        public TransactionListModalController(KoinzContext context, ILogger<TransactionListModalController> logger)
            : base(context, logger) { }

        public List<Transaction> SortedTransactions {
            get {
                return transactions
                    .OrderByDescending(trans => Math.Abs(trans.Amount))
                    .ToList();
            }
        }

        public string ModalId { get; set; } = "";

        private (DateTime, DateTime) GetDefaultDateRange(BudgetTimeSpan? timespan) {
            if (!timespan.HasValue) {
                timespan = BudgetTimeSpan.MONTHLY;
            }
            return timespan.Value.GetStartAndEndDate(DateTime.Today);
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? sourceId, string relatedObjectType, DateTime? startDate, DateTime? endDate, BudgetTimeSpan? timespan) {
            if (!startDate.HasValue && !endDate.HasValue) {
                (startDate, endDate) = GetDefaultDateRange(timespan);
            }

            if (!string.IsNullOrEmpty(relatedObjectType)) {
                ModalId = "transactionList" + relatedObjectType;
                var transactionsIQ = _context.Transactions
                    .Where(trans => trans.Date.Date >= startDate.Value.Date && trans.Date.Date <= endDate.Value.Date);

                if (relatedObjectType == "Bills") {
                    transactionsIQ = transactionsIQ.Where(trans => trans.BillId.HasValue);
                } else if (relatedObjectType == "Savings") {
                    transactionsIQ = transactionsIQ
                        .Include(trans => trans.Transfer)
                            .ThenInclude(fer => fer.RecurringTransfer)
                        .Where(trans => trans.SavingsGoalId.HasValue)
                        .Where(trans => trans.Amount > 0)
                        .Where(trans => trans.Transfer.RecurringTransferId.HasValue || trans.CountAgainstBudget);
                } else {
                    throw new Exception("Unknown related object type: " + relatedObjectType);
                }

                transactions = await transactionsIQ.ToListAsync();
            }

            if (sourceId.HasValue) {
                ModalId = "transactionList" + sourceId.ToString();
                transactions = await _context.Transactions
                    .Where(trans => trans.BillId == sourceId || trans.AccountId == sourceId || trans.TagId == sourceId || trans.CategoryId == sourceId || trans.MerchantId == sourceId || trans.SavingsGoalId == sourceId)
                    .Where(trans => trans.Date.Date >= startDate.Value.Date && trans.Date.Date <= endDate.Value.Date)
                    .ToListAsync();
            }

            return View(this);
        }
    }
}