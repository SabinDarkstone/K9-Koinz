using K9_Koinz.Data;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace K9_Koinz.Pages.Transactions {
    public class RemoveSplitModel : AbstractDbPage {
        public RemoveSplitModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        public async Task<IActionResult> OnGetAsync(Guid parentId) {
            var parent = await _data.TransactionRepository.GetSplitLines(parentId);
            parent.CategoryName = parent.Category.Name;
            parent.IsSplit = false;

            _data.TransactionRepository.Remove(parent.SplitTransactions);

            parent.SplitTransactions = null;
            _data.TransactionRepository.Update(parent);
            _data.Save();

            return RedirectToPage(PagePaths.TransactionDetails, new { id = parentId });
        }
    }
}
