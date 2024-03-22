using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.BudgetLines {
    public class DeleteModel : AbstractDeleteModel<BudgetLine> {
        public DeleteModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger)
            : base(data, logger) { }

        protected override async Task<BudgetLine> QueryRecordAsync(Guid id) {
            return await _data.BudgetLineRepository.GetByIdAsync(id);
        }

        protected override IActionResult NavigateOnSuccess() {
            return RedirectToPage(PagePaths.BudgetEdit, new { id = Record.BudgetId });
        }
    }
}
