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

namespace K9_Koinz.Pages.BudgetLines {
    public class EditModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<EditModel> _logger;
        private readonly BudgetPeriodUtils _budgetPeriodUtils;

        public EditModel(KoinzContext context, ILogger<EditModel> logger) {
            _context = context;
            _logger = logger;
            _budgetPeriodUtils = new BudgetPeriodUtils(context);
        }

        [BindProperty]
        public BudgetLine BudgetLine { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id) {
            if (id == null) {
                return NotFound();
            }

            var budgetLine = await _context.BudgetLines
                .FirstAsync(line => line.Id == id);
            if (budgetLine == null) {
                return NotFound();
            }

            BudgetLine = budgetLine;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var oldRecord = _context.BudgetLines.AsNoTracking().First(line => line.Id == BudgetLine.Id);

            if (!oldRecord.DoRollover && BudgetLine.DoRollover) {
                _budgetPeriodUtils.DeleteOldBudgetLinePeriods(BudgetLine);
                CreateFirstBudgetLinePeriod();
            }

            if (oldRecord.DoRollover && !BudgetLine.DoRollover) {
                _budgetPeriodUtils.DeleteOldBudgetLinePeriods(BudgetLine);
            }

            _context.Attach(BudgetLine).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) {
                if (!BudgetLineExists(BudgetLine.Id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return RedirectToPage("/Budgets/Edit", new { id = BudgetLine.BudgetId });
        }

        public IActionResult OnGetCategoryAutoComplete(string text) {
            text = text.Trim();
            var categories = _context.Categories
                .Include(cat => cat.ParentCategory)
                .AsNoTracking()
                .AsEnumerable()
                .Where(cat => cat.FullyQualifiedName.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(cat => new {
                    label = cat.ParentCategoryId != null ? cat.ParentCategory.Name + ": " + cat.Name : cat.Name,
                    val = cat.Id
                }).ToList();
            return new JsonResult(categories);
        }

        private bool BudgetLineExists(Guid id) {
            return _context.BudgetLines.Any(e => e.Id == id);
        }

        private void CreateFirstBudgetLinePeriod() {
            var parentBudget = _context.Budgets.Find(BudgetLine.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = _budgetPeriodUtils.GetTransactionsForCurrentBudgetLinePeriod(BudgetLine, DateTime.Now).Sum(trans => trans.Amount);

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = BudgetLine.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
        }
    }
}
