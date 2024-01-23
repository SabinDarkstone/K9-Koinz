using Humanizer;
using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Utils {
    public class BudgetPeriodUtils {

        private readonly KoinzContext _context;

        public BudgetPeriodUtils(KoinzContext context) {
            _context = context;
        }

        public List<Transaction> GetTransactionsForCurrentBudgetLinePeriod(BudgetLine budgetLine, DateTime refDate) {
            var parentBudget = _context.Budgets.AsNoTracking().First(bud => bud.Id == budgetLine.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate(refDate);
            var transactionsIQ = _context.Transactions
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .AsNoTracking();

            if (budgetLine.BudgetCategory.CategoryType != CategoryType.ALL) {
                transactionsIQ = transactionsIQ.Where(trans => trans.CategoryId == budgetLine.BudgetCategoryId);
            }

            if (budgetLine.Budget.BudgetTagId != null) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == budgetLine.Budget.BudgetTagId);
            }

            var transactionList = transactionsIQ.ToList();

            transactionList.ForEach(trans => trans.Category = null);

            return transactionList;
        }

        public List<Transaction> GetTransactionsForPreviousLinePeriod(BudgetLine budgetLine, DateTime refDate) {
            var parentBudget = _context.Budgets.AsNoTracking().First(bud => bud.Id == budgetLine.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate(refDate.GetPreviousPeriod(parentBudget.Timespan));
            var transactionsIQ = _context.Transactions
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .AsNoTracking();

            if (budgetLine.BudgetCategory.CategoryType != CategoryType.ALL) {
                transactionsIQ = transactionsIQ.Where(trans => trans.CategoryId == budgetLine.BudgetCategoryId);
            }

            if (budgetLine.Budget.BudgetTagId != null) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == budgetLine.Budget.BudgetTagId);
            }

            var transactionList = transactionsIQ.ToList();

            transactionList.ForEach(trans => trans.Category = null);

            return transactionList;
        }

        public void DeleteOldBudgetLinePeriods(BudgetLine budgetLine) {
            var oldPeriods = _context.BudgetLinePeriods
                .Where(per => per.BudgetLineId == budgetLine.Id)
                .ToList();

            if (oldPeriods.Any()) {
                oldPeriods.ForEach(per => per.BudgetLine = null);
                _context.BudgetLinePeriods.RemoveRange(oldPeriods);
                _context.SaveChanges();
            }
        }
    }
}
