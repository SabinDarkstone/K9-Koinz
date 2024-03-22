using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;
using K9_Koinz.Pages.Meta;
using K9_Koinz.Services;

namespace K9_Koinz.Pages.Transactions {
    public class PrintModel : AbstractIndexModel<Transaction> {
        private readonly IDropdownPopulatorService _dropdownService;

        public List<SelectListItem> AccountOptions;

        public PrintModel(IRepositoryWrapper data, ILogger<AbstractDbPage> logger, IDropdownPopulatorService dropdownService)
            : base(data, logger) {
            _dropdownService = dropdownService;
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MinDateFilter { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MaxDateFilter { get; set; }
        public Guid? AccountFilter { get; set; }

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

        public async Task OnGetAsync(DateTime? minDate, DateTime? maxDate, Guid? accountId) {
            AccountOptions = await _dropdownService.GetAccountListAsync();

            if (minDate == null) {
                minDate = DateTime.Now.StartOfMonth();
            }
            if (maxDate == null) {
                maxDate = DateTime.Now.EndOfMonth();
            }

            MinDateFilter = minDate;
            MaxDateFilter = maxDate;
            AccountFilter = accountId;

            RecordList = await _data.TransactionRepository.GetFiltered(
                new Models.Helpers.TransactionFilterSetting {
                    AccountFilter = AccountFilter,
                    DateRangeStart = MinDateFilter,
                    DateRangeEnd = MaxDateFilter
                });
        }
    }
}
