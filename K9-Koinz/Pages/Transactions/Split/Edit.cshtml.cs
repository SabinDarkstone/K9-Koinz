using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transactions.Split {
    public class EditModel : AbstractDbPage {
        private readonly IDropdownPopulatorService _dropdownService;

        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(context, logger) {
            _dropdownService = dropdownService;
        }

        public SelectList SavingsGoalList { get; set; }
        public SelectList TagOptions;

        [BindProperty]
        public Transaction SplitTransaction { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id) {
            var transaction = _context.Transactions.Find(id);

            TagOptions = await _dropdownService.GetTagListAsync();

            if (transaction != null) {
                SplitTransaction = transaction;
            }

            var savingsGoals = _context.SavingsGoals
                .AsNoTracking()
                .Where(goal => goal.IsActive)
                .ToList();

            if (savingsGoals.Count > 0) {
                SavingsGoalList = new SelectList(savingsGoals,
                    nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            var beforeTransction = _context.Transactions.Find(SplitTransaction.Id);

            Guid savingsGoalId = SplitTransaction.SavingsGoalId.Value;
            string notes = SplitTransaction.Notes;
            var tagId = SplitTransaction.TagId;

            // Change only the savings goal, tag, and notes
            SplitTransaction = beforeTransction;
            SplitTransaction.SavingsGoalId = savingsGoalId == Guid.Empty ? null : savingsGoalId;
            if (SplitTransaction.SavingsGoalId.HasValue) {
                SplitTransaction.IsSavingsSpending = true;
                var savingsGoal = _context.SavingsGoals.Find(savingsGoalId);
                SplitTransaction.SavingsGoalName = savingsGoal.Name;
            }
            SplitTransaction.Notes = notes;

            SplitTransaction.TagId = tagId == Guid.Empty ? null : tagId;

            _context.Transactions.Update(SplitTransaction);
            await _context.SaveChangesAsync();

            return RedirectToPage(PagePaths.TransactionDetails, new { id = SplitTransaction.Id });
        }
    }
}
