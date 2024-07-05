using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {

    public class DetailsModel : AbstractDetailsModel<Category> {

        public readonly ITrendGraphService _trendGraphService;

        public bool ChartError { get; set; }
        public string SpendingHistory { get; set; }

        public DetailsModel(KoinzContext context, ILogger<AbstractDbPage> logger, ITrendGraphService trendGraph)
            : base(context, logger) {
                _trendGraphService = trendGraph;
            }

        protected override async Task<Category> QueryRecordAsync(Guid id) {
            return await _context.Categories
                .Include(cat => cat.ParentCategory)
                .Include(cat => cat.ChildCategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(cat => cat.Id == id);
        }

        protected override async Task AdditionalActionsAsync() {
            SpendingHistory = await _trendGraphService.CreateGraphData(
                predicate: trans => trans.CategoryId == Record.Id || trans.Category.ParentCategoryId == Record.Id,
                hideSavingsSpending: true
            );
            if (SpendingHistory == null) {
                ChartError = true;
            }
        }
    }
}
