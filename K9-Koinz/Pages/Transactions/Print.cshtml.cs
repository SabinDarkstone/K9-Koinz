using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;
using K9_Koinz.Data.Repositories;

namespace K9_Koinz.Pages.Transactions {
    public class PrintModel : PageModel {
        private readonly KoinzContext _context;
        private readonly ILogger<PrintModel> _logger;
        private readonly CategoryRepository _categoryRepo;

        public PrintModel(KoinzContext context, ILogger<PrintModel> logger, CategoryRepository categoryRepo) {
            _context = context;
            _logger = logger;
            _categoryRepo = categoryRepo;
        }

        public SelectList AccountOptions;

        public SelectList CategoryOptions;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MinDateFilter { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? MaxDateFilter { get; set; }
        public Guid? AccountFilter { get; set; }
        public List<Guid> CategoryFilters { get; set; }

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

        public List<Transaction> Transactions { get; set; }

        public async Task OnGetAsync(DateTime? minDate, DateTime? maxDate, Guid? accountId, string catFilter) {
            AccountOptions = new SelectList(_context.Accounts
                .Where(acct => acct.Type == AccountType.CHECKING || acct.Type == AccountType.CREDIT_CARD || acct.Type == AccountType.SAVINGS)
                .OrderBy(acct => acct.Name), nameof(Account.Id), nameof(Account.Name));
            CategoryOptions = await _categoryRepo.GetCategoriesForList();

            if (minDate == null) {
                minDate = DateTime.Now.StartOfMonth();
            }
            if (maxDate == null) {
                maxDate = DateTime.Now.EndOfMonth();
            }

            MinDateFilter = minDate;
            MaxDateFilter = maxDate;
            AccountFilter = accountId;

            CategoryFilters = string.IsNullOrEmpty(catFilter) ? [] : [Guid.Parse(catFilter)];
            CategoryFilters = CategoryFilters.Concat(_categoryRepo.GetChildCategories(CategoryFilters.FirstOrDefault()).Select(cat => cat.Id)).ToList();

            var transactions = _context.Transactions
                .Where(trans => !trans.ParentTransactionId.HasValue)
                .Where(trans => trans.Date >= MinDateFilter.Value && trans.Date <= MaxDateFilter);

            if (accountId != null) {
                transactions = transactions.Where(trans => trans.AccountId == accountId);
            }

            if (catFilter != null) {
                transactions = transactions.Where(trans => trans.CategoryId == Guid.Parse(catFilter));
            }

            Transactions = await transactions.OrderByDescending(trans => trans.Date).ToListAsync();
        }
    }
}
