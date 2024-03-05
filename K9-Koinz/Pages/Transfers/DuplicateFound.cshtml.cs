using K9_Koinz.Data;
using K9_Koinz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace K9_Koinz.Pages.Transfers {
    public class DuplicateFoundModel : PageModel {
        private KoinzContext _context;

        public Transfer Transfer {  get; set; }
        public List<Transfer> MatchingTransfers { get; set; }

        public DuplicateFoundModel(KoinzContext context) {
            _context = context;
        }

        public IActionResult OnGet(Guid? id) {
            if (!id.HasValue) {
                return NotFound();
            }

            Transfer = _context.Transfers
                .Include(fer => fer.FromAccount)
                .Include(fer => fer.ToAccount)
                .Include(fer => fer.Category)
                .Include(fer => fer.Merchant)
                .Include(fer => fer.RepeatConfig)
                .FirstOrDefault(fer => fer.Id == id.Value);

            MatchingTransfers = _context.Transfers
                .Where(fer => fer.ToAccountId == Transfer.ToAccountId && fer.FromAccountId == Transfer.FromAccountId)
                .Where(fer => fer.Amount == Transfer.Amount)
                .Where(fer => fer.RepeatConfig.FirstFiring == Transfer.RepeatConfig.FirstFiring)
                .Where(fer => fer.RepeatConfig.Mode == Transfer.RepeatConfig.Mode)
                .Where(fer => fer.RepeatConfig.IntervalGap == Transfer.RepeatConfig.IntervalGap)
                .Where(fer => fer.RepeatConfig.Frequency == Transfer.RepeatConfig.Frequency)
                .AsEnumerable()
                .Where(fer => Math.Abs((fer.Date - Transfer.Date).TotalDays) <= 5)
                .ToList();

            return Page();
        }

        public IActionResult OnPost(Guid id, string mode) {
            var transfer = _context.Transfers.Find(id);

            if (mode == "cancel") {
                _context.Transfers.Remove(transfer);
                _context.SaveChanges();
                return RedirectToPage("/Transfers/Manage");
            }

            var toAccount = _context.Accounts.Find(transfer.ToAccountId);
            var accountHasGoals = _context.SavingsGoals.Any(goal => goal.AccountId == toAccount.Id);

            if ((toAccount.Type == AccountType.CHECKING || toAccount.Type == AccountType.SAVINGS) && accountHasGoals) {
                return RedirectToPage("/SavingsGoals/AllocateRecurring", new { relatedId = transfer.Id });
            }

            return RedirectToPage("/Transfers/Manage");
        }
    }
}