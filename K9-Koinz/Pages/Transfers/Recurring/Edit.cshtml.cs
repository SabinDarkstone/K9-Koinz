using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers.Recurring {
    public class EditModel : AbstractEditModel<Transfer> {
        public EditModel(KoinzContext context, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(context, logger, dropdownService) { }

        public SelectList GoalOptions { get; set; } = default!;

        protected override async Task<Transfer> QueryRecordAsync(Guid id) {
            return await _context.Transfers
                .Include(fer => fer.Tag)
                .Include(fer => fer.Category)
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.SavingsGoal)
                .Include(fer => fer.Transactions)
                .FirstOrDefaultAsync(fer => fer.Id == id);
        }

        protected override async Task AfterQueryActionsAsync() {
            GoalOptions = new SelectList(await _context.SavingsGoals
                .Where(goal => goal.AccountId == Record.ToAccountId)
                .ToListAsync(), nameof(SavingsGoal.Id), nameof(SavingsGoal.Name));
        }

        protected override void BeforeSaveActions() {
            if (Record.TagId == Guid.Empty) {
                Record.TagId = null;
            }

            if (Record.SavingsGoalId == Guid.Empty) {
                Record.SavingsGoalId = null;
            }

            Record.RepeatConfig.DoRepeat = true;
        }

        protected override IActionResult NavigationOnSuccess() {
            return RedirectToPage(PagePaths.TransferManage);
        }
    }
}
