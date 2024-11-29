using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;
using K9_Koinz.Models.Helpers;
using K9_Koinz.Factories;

namespace K9_Koinz.Pages.Transactions {
    public class EditModel : EditPageModel<Transaction> {
        private readonly IRepoFactory _repoFactory;

        public SelectList GoalOptions { get; set; } = default!;
        public SelectList BillOptions { get; set; } = default!;

        public EditModel(IDropdownPopulatorService dropdownService, TransactionRepository repository,
            IDupeCheckerService<Transaction> dupeChecker, IRepoFactory repoFactory)
            : base(repository, dropdownService) {
            _repoFactory = repoFactory;
        }

        protected override async Task AfterQueryActions() {
            var savingsRepo = _repoFactory.CreateSpecializedRepository<SavingsRepository>();
            if (Record.Category.CategoryType == CategoryType.TRANSFER) {
                GoalOptions = await savingsRepo.GetGoalOptions(Record.AccountId);
            } else {
                GoalOptions = await savingsRepo.GetGoalOptions();
            }

            if (Record.Category.CategoryType == CategoryType.EXPENSE) {
                var billRepo = _repoFactory.CreateSpecializedRepository<BillRepository>();
                BillOptions = await billRepo.GetBillOptions(Record.AccountId);
            }
        }

        protected override async Task<Transaction> QueryRecord(Guid id) {
            return await (_repository as TransactionRepository).GetTransactionWithDetailsById(id);
        }

        protected override IActionResult HandleNavigate(DbSaveResult saveResult) {
            if (saveResult.BeforeStatus == TriggerStatus.DUPLICATE_FOUND) {
                return RedirectToPage(PagePaths.TransactionDuplicateFound, new { id = Record.Id });
            } else if (saveResult.AfterStatus == TriggerStatus.GO_SAVINGS) {
                return RedirectToPage(PagePaths.SavingsAllocate, new { relatedId = Record.Id });
            } else {
                var routeValues = Bakery.MakeRouteFromCookie(Request.Cookies["backToTransactions"]);
                return RedirectToPage(PagePaths.TransactionIndex, routeValues);
            }
        }
    }
}
