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

namespace K9_Koinz.Pages.Budgets {
    public class EditModel : AbstractEditModel<Budget> {
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

            TagOptions = _tagService.GetTagList();

            var budget = await _context.Budgets
                .Include(bud => bud.BudgetLines)
                    .ThenInclude(line => line.BudgetCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budget == null) {
                return NotFound();
            }
            Record = budget;

            if (Record.DoNotUseCategories) {
                Record.BudgetedAmount = Record.BudgetLines.First().BudgetedAmount;
                Record.DoNoCategoryRollover = Record.BudgetLines.First().DoRollover;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            if (Record.BudgetTagId == Guid.Empty) {
                Record.BudgetTagId = null;
            } else {
                var tag = await _context.Tags.FindAsync(Record.BudgetTagId);
                Record.BudgetTagName = tag.Name;
            }

            var oldRecord = _context.BudgetLines
                .Single(line => line.BudgetId == Record.Id);

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

            if (Record.DoNotUseCategories) {
                var allowanceLine = _context.BudgetLines.SingleOrDefault(line => line.BudgetId == Record.Id);
                allowanceLine.BudgetedAmount = Record.BudgetedAmount.Value;
                allowanceLine.DoRollover = Record.DoNoCategoryRollover;
                _context.BudgetLines.Update(allowanceLine);
                _context.SaveChanges();

                if (!oldRecord.DoRollover && Record.DoNoCategoryRollover) {
                    _budgetService.DeleteOldBudgetLinePeriods(allowanceLine);
                    CreateFirstBudgetLinePeriod(allowanceLine);
                }

                if (!Record.DoNoCategoryRollover) {
                    _budgetService.DeleteOldBudgetLinePeriods(allowanceLine);
                }
            }

            return RedirectToPage("./Index");
        }

        private void CreateFirstBudgetLinePeriod(BudgetLine budgetLine) {
            var (startDate, endDate) = Record.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = _budgetService.GetTransactionsForCurrentBudgetLinePeriod(budgetLine, DateTime.Now).Sum(trans => trans.Amount);

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = budgetLine.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
            _context.SaveChanges();
        }
    }
}
