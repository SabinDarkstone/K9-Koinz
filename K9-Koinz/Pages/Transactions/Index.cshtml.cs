using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;
using K9_Koinz.Services;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Models.Helpers;

namespace K9_Koinz.Pages.Transactions
{
    public class IndexModel : AbstractIndexModel<Transaction> {
        private readonly KoinzContext _context;
        private readonly IDropdownPopulatorService _dropdownService;

        public IndexModel(RepositoryWrapper data, ILogger<IndexModel> logger, IDropdownPopulatorService dropdownService)
            :base(data, logger) {
            _dropdownService = dropdownService;
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

        public PaginatedList<Transaction> Transactions { get; set; }
        public SelectList CategoryOptions;
        public List<SelectListItem> AccountOptions;

        public async Task OnGetAsync(string sortOrder, string catFilter, string merchFilter, string accountFilter, string tagId, DateTime? minDate, DateTime? maxDate, int? pageIndex, string searchString, bool? hideTransfers) {
            CategoryOptions = await _data.CategoryRepository.GetDropdown();
            AccountOptions = await _dropdownService.GetAccountListAsync();

            // Sort parameters
            DateSort = string.IsNullOrEmpty(sortOrder) || sortOrder == "Date" ? "date_desc" : "Date";
            MerchantSort = sortOrder == "Merchant" ? "merchant_desc" : "Merchant";
            AmountSort = sortOrder == "Amount" ? "amount_desc" : "Amount";
            CurrentSort = sortOrder;

            // Make a copy of incomeing filters
            SelectedCategory = catFilter;
            SelectedMerchant = merchFilter;
            SelectedAccount = accountFilter;
            SearchText = searchString;
            SelectedTag = tagId;
            MinDateFilter = minDate;
            MaxDateFilter = maxDate;

            // Parse selected filters
            if (!string.IsNullOrWhiteSpace(SelectedCategory)) {
                var selectedCateogryId = Guid.Parse(SelectedCategory);
                // Filter transactions by the selected category
                CategoryFilters = [selectedCateogryId];

                // Add the child categories to the list
                CategoryFilters.AddRange((await _data.CategoryRepository.GetChildrenAsync(selectedCateogryId))
                    .Select(x => x.Id));
            }

            MerchantFilter = SelectedMerchant.ToGuid().Value;
            AccountFilter = SelectedAccount.ToGuid().Value;
            TagFilter = SelectedTag.ToGuid().Value;

            var filters = new TransactionFilterSetting(sortOrder, CategoryFilters, MerchantFilter, AccountFilter, TagFilter, MinDateFilter, MaxDateFilter, pageIndex, searchString, hideTransfers);
            RecordList = await _data.TransactionRepository.GetFiltered(filters);
        }
    }
}
