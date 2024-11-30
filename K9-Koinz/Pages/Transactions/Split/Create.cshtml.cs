using K9_Koinz.Data;
using K9_Koinz.Data.Repositories;
using K9_Koinz.Factories;
using K9_Koinz.Models;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace K9_Koinz.Pages.Transactions.Split {
    public class CreateModel : PageModel {
        private readonly TransactionRepository _repository;
        private readonly IRepoFactory _repoFactory;

        public CreateModel(TransactionRepository repository, IRepoFactory repoFactory) {
            _repository = repository;
            _repoFactory = repoFactory;
        }

        public Transaction ParentTransaction { get; set; }
        public string ErrorMessage { get; set; }

        public SelectList SavingsGoalsList;

        [BindProperty]
        public List<Transaction> SplitTransactions { get; set; } = new List<Transaction>();

        public async Task<IActionResult> OnGetAsync(Guid parentId) {
            ParentTransaction = await _repository.GetTransactionWithDetailsById(parentId);
            SplitTransactions = await _repository.GetChildTransactions(parentId);

            while (SplitTransactions.Count < 40) {
                SplitTransactions.Add(new Transaction {
                    AccountId = ParentTransaction.AccountId,
                    TagId = ParentTransaction.TagId,
                    MerchantId = ParentTransaction.MerchantId,
                    MerchantName = ParentTransaction.MerchantName,
                    ParentTransactionId = parentId
                });
            }

            var savingsRepo = _repoFactory.CreateSpecializedRepository<SavingsRepository>();
            SavingsGoalsList = await savingsRepo.GetGoalOptions(ParentTransaction.AccountId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() {
            if (!ModelState.IsValid) {
                return Page();
            }

            var allTransactions = await _repository.CreateSplitTransaction(SplitTransactions);
            if (allTransactions.Count() > 0) {
                return RedirectToPage(PagePaths.TransactionDetails, new { id = allTransactions[0].Id });
            } else {
                return Page();
            }
        }
    }
}
