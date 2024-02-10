using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.BudgetLines {
    public class CreateModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<CreateModel> _logger;
        private readonly IBudgetService _budgetService;

        public Budget Budget { get; set; }

        [BindProperty]
        public BudgetLine BudgetLine { get; set; }


        public CreateModel(KoinzContext context, ILogger<CreateModel> logger, IBudgetService budgetService) {
            _context = context;
            _logger = logger;
            _budgetService = budgetService;
        }

        public async Task<IActionResult> OnGetAsync(Guid budgetId) {
            Budget = await _context.Budgets.FindAsync(budgetId);

            ViewData["CategoryId"] = new SelectList(_context.Categories, nameof(Category.Id), nameof(Category.Name)).OrderBy(cat => cat.Text);

            return Page();
        }

        public async Task<IActionResult> OnPost() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var category = await _context.Categories.SingleAsync(cat => cat.Id == BudgetLine.BudgetCategoryId);
            var budget = await _context.Budgets.SingleAsync(bud => bud.Id == BudgetLine.BudgetId);
            BudgetLine.BudgetCategoryName = category.Name;
            BudgetLine.BudgetName = budget.Name;

            _context.BudgetLines.Add(BudgetLine);
            _context.SaveChanges();

            if (BudgetLine.DoRollover) {
                CreateFirstBudgetLinePeriod();
                _context.SaveChanges();
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

        private void CreateFirstBudgetLinePeriod() {
            var parentBudget = _context.Budgets.Find(BudgetLine.BudgetId);
            var (startDate, endDate) = parentBudget.Timespan.GetStartAndEndDate();
            var totalSpentSoFar = _budgetService.GetTransactionsForCurrentBudgetLinePeriod(BudgetLine, DateTime.Now).Sum(trans => trans.Amount);

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
