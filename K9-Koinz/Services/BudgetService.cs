using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Services {
    public interface IBudgetService : ICustomService {
        public abstract List<Transaction> GetTransactions(BudgetLine budgetLine, DateTime period);
        public abstract Task<List<BudgetLine>> GetUnallocatedSpendingAsync(Budget budget, DateTime period);
        public abstract Task<List<Transaction>> GetTransactionsForCurrentBudgetLinePeriodAsync(BudgetLine budgetLine, DateTime refDate);
        public abstract Task<List<Transaction>> GetTransactionsForPreviousLinePeriodAsync(BudgetLine budgetLine, DateTime refDate);
        public abstract void DeleteOldBudgetLinePeriods(BudgetLine budgetLine);
    }

    public class BudgetService : AbstractService<BudgetService>, IBudgetService {
        public BudgetService(KoinzContext context, ILogger<BudgetService> logger) : base(context, logger) { }

        public List<Transaction> GetTransactions(BudgetLine line, DateTime period) {
            var (startDate, endDate) = line.Budget.Timespan.GetStartAndEndDate(period);

            if (line.Budget.DoNotUseCategories) {
                line.BudgetCategory.Transactions = _context.Transactions
                    .Where(trans => trans.Category.CategoryType != CategoryType.TRANSFER)
                    .Where(trans => !trans.IsSavingsSpending)
                    .ToList();
            }

            var transactionsIQ = line.BudgetCategory.Transactions
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .Where(trans => !trans.IsSavingsSpending);

            if (line.Budget.BudgetTagId.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == line.Budget.BudgetTagId.Value);
            }
            var transactions = transactionsIQ.AsEnumerable();

            if (line.BudgetCategory.CategoryType == CategoryType.INCOME || line.BudgetCategory.CategoryType == CategoryType.EXPENSE) {
                var childCategoryTransactionsIQ = line.BudgetCategory.ChildCategories
                    .SelectMany(cat => cat.Transactions
                        .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                        .Where(trans => !trans.IsSavingsSpending)
                    ).AsEnumerable();

                if (line.Budget.BudgetTagId.HasValue) {
                    childCategoryTransactionsIQ = childCategoryTransactionsIQ.Where(trans => trans.TagId == line.Budget.BudgetTagId);
                }
                var childCategoryTransactions = childCategoryTransactionsIQ.AsEnumerable();
                transactions = [.. transactions, .. childCategoryTransactions];
            }

            line.SpentAmount = transactions.Sum(trans => trans.Amount);
            if ((line.BudgetCategory.CategoryType == CategoryType.EXPENSE || line.BudgetCategory.CategoryType == CategoryType.ALL) && line.SpentAmount != 0.0) {
                line.SpentAmount *= -1;
            }
            line.Transactions = transactions.ToList();
            return transactions.ToList();
        }

        public async Task<List<BudgetLine>> GetUnallocatedSpendingAsync(Budget budget, DateTime period) {
            var categoryData = await _context.Categories.AsNoTracking().ToDictionaryAsync(cat => cat.Id);

            // Is this budget not using categories?
            if (budget.BudgetLines == null || budget.BudgetLines.Count == 0) {
                return new List<BudgetLine>();
            }

            // Get all budget categories from income and expense lines
            var allocatedCategories = budget.ExpenseLines
                .Select(line => line.BudgetCategoryId)
                .Concat(budget.IncomeLines.Select(line => line.BudgetCategoryId));

            var transferCategoryIds = categoryData.Values.Where(cat => cat.CategoryType == CategoryType.TRANSFER).Select(cat => cat.Id);
            allocatedCategories = allocatedCategories.Concat(transferCategoryIds).AsEnumerable();

            // Get child categories for those allocated categories, too
            var allocatedChildCategories = budget.ExpenseLines.SelectMany(line => line.BudgetCategory.ChildCategories.Select(cat => cat.Id)).AsEnumerable().Concat(budget.IncomeLines.SelectMany(line => line.BudgetCategory.ChildCategories.Select(cat => cat.Id)).ToList());

            // Concat those two lists of Category GUIDs together
            var allUnallocatedCategories = allocatedCategories.Concat(allocatedChildCategories);

            var (startDate, endDate) = budget.Timespan.GetStartAndEndDate(period);
            var transactionsIQ = _context.Transactions.AsNoTracking()
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .Where(trans => !allUnallocatedCategories.Contains(trans.CategoryId))
                .Where(trans => trans.Account.Type != AccountType.LOAN && trans.Account.Type != AccountType.INVESTMENT && trans.Account.Type != AccountType.PROPERTY)
                .Where(trans => !trans.IsSavingsSpending);
            if (budget.BudgetTagId.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == budget.BudgetTagId.Value);
            }
            var transactions = await transactionsIQ.ToListAsync();

            var unallocatedBudgetLines = new Dictionary<Guid, BudgetLine>();
            foreach (var trans in transactions) {
                if (unallocatedBudgetLines.ContainsKey(trans.CategoryId)) {
                    unallocatedBudgetLines[trans.CategoryId].Transactions.Add(trans);
                } else {
                    var budgetCategory = categoryData[trans.CategoryId];
                    if (budgetCategory.ParentCategoryId.HasValue) {
                        budgetCategory.ParentCategory = categoryData[budgetCategory.ParentCategoryId.Value];
                    }

                    var newBudgetLine = new BudgetLine {
                        BudgetCategoryId = trans.CategoryId,
                        BudgetCategory = categoryData[trans.CategoryId],
                        BudgetId = budget.Id,
                        Transactions = new List<Transaction>()
                    };
                    newBudgetLine.Transactions.Add(trans);
                    unallocatedBudgetLines.Add(trans.CategoryId, newBudgetLine);
                }
            }

            foreach (var line in unallocatedBudgetLines.Values) {
                line.SpentAmount = line.Transactions.Sum(trans => trans.Amount);
            }

            return unallocatedBudgetLines.Values.Where(line => line.Transactions.Sum(trans => trans.Amount) != 0).ToList();
        }

        public async Task<List<Transaction>> GetTransactionsForCurrentBudgetLinePeriodAsync(BudgetLine budgetLine, DateTime refDate) {
            var parentBudget = await _context.Budgets
                .AsNoTracking()
                .FirstAsync(bud => bud.Id == budgetLine.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate(refDate);
            return await GetTransactionsForLineBetweenDatesAsync(budgetLine, startDate, endDate);
        }

        public async Task<List<Transaction>> GetTransactionsForPreviousLinePeriodAsync(BudgetLine budgetLine, DateTime refDate) {
            var parentBudget = await _context.Budgets
                .AsNoTracking()
                .FirstOrDefaultAsync(bud => bud.Id == budgetLine.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate(refDate.GetPreviousPeriod(parentBudget.Timespan));
            return await GetTransactionsForLineBetweenDatesAsync(budgetLine, startDate, endDate);
        }

        public void DeleteOldBudgetLinePeriods(BudgetLine budgetLine) {
            var oldPeriods = _context.BudgetLinePeriods
                .Where(per => per.BudgetLineId == budgetLine.Id)
                .ToList();

            if (oldPeriods.Count > 0) {
                oldPeriods.ForEach(per => per.BudgetLine = null);
                _context.BudgetLinePeriods.RemoveRange(oldPeriods);
                _context.SaveChanges();
            }
        }

        private async Task<List<Transaction>> GetTransactionsForLineBetweenDatesAsync(BudgetLine budgetLine, DateTime startDate, DateTime endDate) {
            var transactionsIQ = _context.Transactions
                .Where(trans => trans.Date >= startDate && trans.Date <= endDate)
                .Where(trans => !trans.IsSavingsSpending)
                .AsNoTracking();

            if (budgetLine.BudgetCategory.CategoryType != CategoryType.ALL) {
                var childCategories = await _context.Categories
                    .Where(cat => cat.ParentCategoryId == budgetLine.BudgetCategoryId)
                    .Select(cat => cat.Id)
                    .ToListAsync();
                transactionsIQ = transactionsIQ
                    .Where(trans => trans.CategoryId == budgetLine.BudgetCategoryId || childCategories.Contains(trans.CategoryId))
                    .Where(trans => !trans.IsSavingsSpending);
            }

            if (budgetLine.Budget.BudgetTagId != null) {
                transactionsIQ = transactionsIQ.Where(trans => trans.TagId == budgetLine.Budget.BudgetTagId);
            }

            var transactionList = await transactionsIQ.ToListAsync();

            transactionList.ForEach(trans => trans.Category = null);

            return transactionList;
        }
    }
}
