using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;

namespace K9_Koinz.Pages.BudgetLines {
    public class EditModel : AbstractEditModel<BudgetLine> {
        private readonly IBudgetService _budgetService;

        public EditModel(KoinzContext context, IAccountService accountService,
            IAutocompleteService autocompleteService, ITagService tagService, IBudgetService budgetService)
            : base(context, accountService, autocompleteService, tagService) {
            _budgetService = budgetService;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budgetLine = await _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .FirstAsync(line => line.Id == id);
            if (budgetLine == null) {
                return NotFound();
            }

            Record = budgetLine;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var oldRecord = _context.BudgetLines
                .Include(line => line.BudgetCategory)
                .Include(line => line.Budget)
                .AsNoTracking()
                .First(line => line.Id == Record.Id);

            Record.BudgetCategory = oldRecord.BudgetCategory;
            Record.Budget = oldRecord.Budget;

            if (!oldRecord.DoRollover && Record.DoRollover) {
                _budgetService.DeleteOldBudgetLinePeriods(Record);
                CreateFirstBudgetLinePeriod();
            }

            if (oldRecord.DoRollover && !Record.DoRollover) {
                _budgetService.DeleteOldBudgetLinePeriods(Record);
            }

            Record.BudgetCategoryId = oldRecord.BudgetCategoryId;
            Record.BudgetId = oldRecord.BudgetId;
            Record.BudgetCategory = null;
            Record.Budget = null;

            _context.Attach(Record).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!RecordExists(Record.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("/Budgets/Edit", new { id = Record.BudgetId });
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            return _autocompleteService.AutocompleteCategories(text.Trim());
        }

        private void CreateFirstBudgetLinePeriod() {
            var parentBudget = _context.Budgets.Find(Record.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = _budgetService.GetTransactionsForCurrentBudgetLinePeriod(Record, DateTime.Now).Sum(trans => trans.Amount);

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = Record.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
        }
    }
}
