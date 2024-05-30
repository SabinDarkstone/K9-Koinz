using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers {
    [Authorize]
    public class ManageModel : AbstractDbPage {
        public ManageModel(KoinzContext context, ILogger<AbstractDbPage> logger)
            : base(context, logger) { }

        public Dictionary<string, List<Transfer>> RecurringTransfersDict;

        public async Task<IActionResult> OnGetAsync() {
            RecurringTransfersDict = await _context.Transfers
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.RepeatConfig)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.Category)
                .Include(fer => fer.SavingsGoal)
                .Where(fer => fer.RepeatConfigId.HasValue)
                .AsNoTracking()
                .GroupBy(fer => fer.FromAccount.Name)
                .ToDictionaryAsync(
                    x => x.Key ?? "Income",
                    x => x.AsEnumerable()
                        .OrderBy(fer => fer.RepeatConfig.CalculatedNextFiring)
                        .ThenBy(fer => fer.ToAccount.Name)
                        .ToList()
                );

            return Page();
        }
    }
}
