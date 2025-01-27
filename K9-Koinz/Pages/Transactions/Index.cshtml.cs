using K9_Koinz.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Data.Repositories;
using K9_Koinz.Models.Helpers;
using NuGet.Protocol;

namespace K9_Koinz.Pages.Transactions {
    public class IndexModel : IndexPageModel<Transaction> {
        private readonly IDropdownPopulatorService _dropdownService;
        private readonly CategoryRepository _categoryRepo;

        public IndexModel(TransactionRepository repository, IDropdownPopulatorService dropdownService, CategoryRepository categoryRepo) : base(repository) {
            _dropdownService = dropdownService;
            _categoryRepo = categoryRepo;
        }

        private bool? _hideTransfers;

        public List<Guid> CategoryFilters { get; set; }
        public Guid MerchantFilter { get; set; }
        public Guid AccountFilter { get; set; }
        public Guid TagFilter { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MinDateFilter { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MaxDateFilter { get; set; }

        public string MerchantSort { get; set; }
        public string DateSort { get; set; }
        public string AmountSort { get; set; }
        public string CurrentSort { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedMerchant { get; set; }
        public string SelectedAccount { get; set; }
        public string SelectedTag { get; set; }
        public string SearchText { get; set; }

        public int BudgetFiltering { get; set; }

        public bool HideTransfers {
            get {
                if (_hideTransfers.HasValue) {
                    return _hideTransfers.Value;
                } else {
                    return false;
                }
            }
        }

        public string MinDateString {
            get {
                if (MinDateFilter.HasValue) {
                    return MinDateFilter.Value.FormatForUrl();
                } else {
                    return null;
                }
            }
        }
        public string MaxDateString {
            get {
                if (MaxDateFilter.HasValue) {
                    return MaxDateFilter.Value.FormatForUrl();
                } else {
                    return null;
                }
            }
        }

        public SelectList CategoryOptions;
        public List<SelectListItem> AccountOptions;

        public async Task OnGetAsync(string sortOrder, string catFilter, string merchFilter, string accountFilter, string tagId, DateTime? minDate, DateTime? maxDate, int? pageIndex, string searchString, bool? hideTransfers, int? budgetFiltering) {
            CategoryOptions = await _categoryRepo.GetCategoriesForList();
            AccountOptions = await _dropdownService.GetAccountListAsync();

            DateSort = string.IsNullOrEmpty(sortOrder) || sortOrder == "Date" ? "date_desc" : "Date";
            MerchantSort = sortOrder == "Merchant" ? "merchant_desc" : "Merchant";
            AmountSort = sortOrder == "Amount" ? "amount_desc" : "Amount";
            CurrentSort = sortOrder;
            SelectedCategory = catFilter;
            SelectedMerchant = merchFilter;
            SelectedAccount = accountFilter;
            SearchText = searchString;
            SelectedTag = tagId;
            MinDateFilter = minDate;
            MaxDateFilter = maxDate;

            CategoryFilters = string.IsNullOrEmpty(catFilter) ? [] : [Guid.Parse(catFilter)];
            CategoryFilters = CategoryFilters.Concat(_categoryRepo.GetChildCategories(CategoryFilters.FirstOrDefault()).Select(cat => cat.Id)).ToList();

            if (!string.IsNullOrEmpty(merchFilter)) {
                MerchantFilter = Guid.Parse(merchFilter);
            }
            if (!string.IsNullOrEmpty(accountFilter)) {
                AccountFilter = Guid.Parse(accountFilter);
            }
            if (!string.IsNullOrEmpty(tagId)) {
                TagFilter = Guid.Parse(tagId);
            }
            _hideTransfers = hideTransfers;

            BudgetFiltering = budgetFiltering ?? 0;

            Response.Cookies.Append("backToTransactions", new TransactionNavPayload {
                AccountFilter = SelectedAccount,
                CatFilter = SelectedCategory,
                HideTransfers = HideTransfers,
                MaxDate = MaxDateString,
                MinDate = MinDateString,
                MerchFilter = SelectedMerchant,
                PageIndex = pageIndex ?? 1,
                SearchString = SearchText,
                SortOrder = CurrentSort
            }.ToJson());

            Records = await (_repository as TransactionRepository).SearchTransactions(
                CategoryFilters, MerchantFilter, AccountFilter, TagFilter, searchString, _hideTransfers,
                MinDateFilter, MaxDateFilter, BudgetFiltering, sortOrder, pageIndex, 100
            );
        }
    }
}
