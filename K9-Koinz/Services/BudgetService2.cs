using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Models.Meta;
using K9_Koinz.Services.Meta;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace K9_Koinz.Services {
    public interface IBudgetService2 : ICustomService {
        public abstract Budget GetBudget(Guid budgetId, DateTime referenceDate);
        public abstract Task<List<Transaction>> GetTransactionsForCurrentBudgetLinePeriodAsync(BudgetLine budgetLine, DateTime refDate);
        public abstract Task<List<Transaction>> GetTransactionsForPreviousLinePeriodAsync(BudgetLine budgetLine, DateTime refDate);
        public abstract void DeleteOldBudgetLinePeriods(BudgetLine budgetLine);
    }
    public class BudgetService2 : AbstractService<BudgetService2>, IBudgetService2 {
        public BudgetService2(KoinzContext context, ILogger<BudgetService2> logger) : base(context, logger) { }

        public Budget GetBudget(Guid budgetId, DateTime referenceDate) {
            // Get basic budget information
            var budget = _context.Budgets
                .AsNoTracking()
                .Where(bud => bud.Id == budgetId)
                .FirstOrDefault();

            // Find out start and end date for selected period
            var (startDate, endDate) = budget.Timespan.GetStartAndEndDate(referenceDate);

            // Build a query for getting all transactions needed to display a budget
            var transactionsIQ = _context.Transactions
                .AsNoTracking()
                .Include(trans => trans.Account)
                .Include(trans => trans.Category)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.IsSplit)
                .Where(trans => !trans.BillId.HasValue)
                .Where(trans => !trans.Account.HideAccountTransactions)
                .Where(trans => trans.Date.Date >= startDate.Date && trans.Date.Date <= endDate.Date);

            if (budget.BudgetTagId.HasValue) {
                transactionsIQ = transactionsIQ
                    .Where(trans => trans.TagId == budget.BudgetTagId.Value);
            }

            var allTransactions = transactionsIQ.ToList();

            // Get categories in budget for grouping
            var budgetCategories = _context.BudgetLines
                .AsNoTracking()
                .Include(line => line.BudgetCategory)
                    .ThenInclude(cat => cat.ParentCategory)
                .Include(line => line.BudgetCategory)
                    .ThenInclude(cat => cat.ChildCategories)
                .AsSplitQuery()
                .Where(line => line.BudgetId == budgetId)
                .ToList();

            // Create dictionary and pre-populate category IDs
            Dictionary<Guid, List<Transaction>> allocated = new();
            foreach (var line in budgetCategories) {
                allocated.Add(line.BudgetCategoryId, new List<Transaction>());
            }

            // Create blank dictionary for unallocated transactions
            Dictionary<Guid, List<Transaction>> unallocated = new();

            // Slot each transactions into a category or marked it as unallocated
            foreach (var transaction in allTransactions) {
                if (!transaction.CategoryId.HasValue) {
                    continue;
                }

                Guid catId = transaction.CategoryId.Value;
                Guid? parentCatId = transaction.Category.ParentCategoryId;

                // First check the transaction category
                if (allocated.ContainsKey(catId)) {
                    _logger.LogInformation("Found allocated category");
                    allocated[catId].Add(transaction);
                    continue;
                }

                // Check parent category, if needed
                if (parentCatId.HasValue && allocated.ContainsKey(parentCatId.Value)) {
                    _logger.LogInformation("Found allocated parent category");
                    allocated[parentCatId.Value].Add(transaction);
                    continue;
                }

                // Add to unallocated lines
                if (unallocated.ContainsKey(catId)) {
                    _logger.LogInformation("Could not find allocated category, but unallocated line exists");
                    unallocated[catId].Add(transaction);
                } else {
                    _logger.LogInformation("Could not find allocated category, creating new unallocated line");
                    unallocated.Add(catId, new List<Transaction> { transaction });
                }
            }

            // Populate budget with grouped transactions
            foreach (var line in budgetCategories) {
                var transactionList = new List<Transaction>();
                if (allocated.ContainsKey(line.BudgetCategoryId)) {
                    transactionList = allocated[line.BudgetCategoryId].ToList();
                }

                line.Transactions = transactionList;
                line.Budget = budget;
                line.SpentAmount = transactionList.GetTotal();
            }

            // Get details about unallocated lines
            var keys = unallocated.Keys;
            var unallocatedCategories = _context.Categories
                .AsNoTracking()
                .Where(cat => keys.Contains(cat.Id))
                .ToDictionary(key => key.Id, value => value);

            // Add in unallocated lines, too
            var unallocatedLines = unallocated.Select(line => {
                return new BudgetLine {
                    BudgetId = budgetId,
                    BudgetCategoryId = line.Key,
                    BudgetCategory = unallocatedCategories[line.Key],
                    Transactions = line.Value,
                    SpentAmount = line.Value.GetTotal(),
                    Budget = budget
                };
            }).Where(line => line.SpentAmount != 0).ToList();

            budget.UnallocatedLines = unallocatedLines;
            budget.BudgetLines = budgetCategories;

            return budget;
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
                .Where(trans => !trans.IsSplit)
                .Where(trans => !trans.IsSavingsSpending)
                .Where(trans => !trans.BillId.HasValue)
                .Where(trans => !trans.Account.HideAccountTransactions)
                .AsNoTracking();

            if (budgetLine.BudgetCategory.CategoryType != CategoryType.ALL) {
                var childCategories = await _context.Categories
                    .Where(cat => cat.ParentCategoryId == budgetLine.BudgetCategoryId)
                    .Select(cat => cat.Id)
                    .ToListAsync();
                transactionsIQ = transactionsIQ
                    .Where(trans => trans.CategoryId.HasValue)
                    .Where(trans => trans.CategoryId == budgetLine.BudgetCategoryId || childCategories.Contains(trans.CategoryId.Value))
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
