using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Budgets {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly IBudgetService _budgetService;
        private readonly ITagService _tagService;

        public CreateModel(KoinzContext context, IBudgetService budgetService, ITagService tagService) {
            _context = context;
            _budgetService = budgetService;
            _tagService = tagService;
        }

        [BindProperty]
        public Budget Budget { get; set; } = default!;
        public SelectList TagOptions;

        public IActionResult OnGet() {
            TagOptions = _tagService.GetTagList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            if (Budget.BudgetTagId == Guid.Empty) {
                Budget.BudgetTagId = null;
            } else {
                var tag = await _context.Tags.FindAsync(Budget.BudgetTagId);
                Budget.BudgetTagName = tag.Name;
            }

            _context.Budgets.Add(Budget);
            await _context.SaveChangesAsync();

            if (Budget.DoNotUseCategories) {
                BudgetLine allowanceLine;
                var allCategory = await _context.Categories.SingleOrDefaultAsync(cat => cat.CategoryType == CategoryType.ALL);
                if (allCategory == null) {
                    var newAllCategory = new Category {
                        Name = "All",
                        ParentCategoryId = null,
                        CategoryType = CategoryType.ALL
                    };
                    _context.Categories.Add(newAllCategory);
                    _context.SaveChanges();
                    allCategory = newAllCategory;
                }
                allowanceLine = new BudgetLine {
                    BudgetId = Budget.Id,
                    BudgetedAmount = Budget.BudgetedAmount.Value,
                    BudgetCategoryId = allCategory.Id,
                    DoRollover = Budget.DoNoCategoryRollover
                };
                _context.BudgetLines.Add(allowanceLine);
                _context.SaveChanges();

                if (Budget.DoNoCategoryRollover) {
                    CreateFirstBudgetLinePeriod(allowanceLine);
                    _context.SaveChanges();
                }
            }

            return RedirectToPage("./Index");
        }

        private void CreateFirstBudgetLinePeriod(BudgetLine budgetLine) {
            var (startDate, endDate) = Budget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = _budgetService.GetTransactionsForCurrentBudgetLinePeriod(budgetLine, DateTime.Now).Sum(trans => trans.Amount);

            var firstPeriod = new BudgetLinePeriod {
                BudgetLineId = budgetLine.Id,
                StartingAmount = 0,
                StartDate = startDate,
                EndDate = endDate,
                SpentAmount = totalSpentSoFar
            };

            _context.BudgetLinePeriods.Add(firstPeriod);
        }
    }
}
