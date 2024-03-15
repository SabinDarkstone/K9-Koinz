using Microsoft.AspNetCore.Mvc;
using K9_Koinz.Data;
using K9_Koinz.Models;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Categories {
    public class CreateModel : AbstractCreateModel<Category> {
        public CreateModel(KoinzContext context, ILogger<AbstractDbPage> logger,
            IAccountService accountService, ITagService tagService)
                : base(context, logger, accountService, tagService) { }

        public Category ParentCategory { get; set; }

        protected override async Task BeforePageLoadActions() {
            await base.BeforePageLoadActions();

            if (RelatedId.HasValue) {
                ParentCategory = await _context.Categories.FindAsync(RelatedId);
            }
        }

        protected override async Task BeforeSaveActionsAsync() {
            if (Record.ParentCategoryId.HasValue) {
                var parentCategory = await _context.Categories.FindAsync(Record.ParentCategoryId);
                Record.ParentCategoryName = parentCategory.Name;
            }
        }
    }
}
