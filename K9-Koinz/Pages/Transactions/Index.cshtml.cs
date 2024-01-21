using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using K9_Koinz.Data;
using K9_Koinz.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using K9_Koinz.Utils;

namespace K9_Koinz.Pages.Transactions
{
    public class IndexModel : PageModel {
        private readonly KoinzContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(KoinzContext context, IConfiguration configuration, ILogger<IndexModel> logger) {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public string SearchString { get; set; }
        public List<Guid> CategoryFilters { get; set; }
        public Guid MerchantFilter { get; set; }
        public Guid AccountFilter { get; set; }

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

        public async Task OnGetAsync(string sortOrder, string catFilter, string merchFilter, string accountFilter, DateTime? minDate, DateTime? maxDate, string searchText, int? pageIndex) {
            CategoryOptions = new SelectList(_context.Categories.OrderBy(cat => cat.Name).ToList(), nameof(Category.Id), nameof(Category.Name));
            AccountOptions = AccountUtils.GetAccountList(_context, true);

            DateSort = string.IsNullOrEmpty(sortOrder) || sortOrder == "Date" ? "date_desc" : "Date";
            MerchantSort = sortOrder == "Merchant" ? "merchant_desc" : "Merchant";
            AmountSort = sortOrder == "Amount" ? "amount_desc" : "Amount";
            CurrentSort = sortOrder;
            SelectedCategory = catFilter;
            SelectedMerchant = merchFilter;
            SelectedAccount = accountFilter;
            MinDateFilter = minDate;
            MaxDateFilter = maxDate;

            IQueryable<Transaction> transactionsIQ = from trans in _context.Transactions
                                                     select trans;

            if (!string.IsNullOrWhiteSpace(catFilter)) {
                CategoryFilters = [Guid.Parse(SelectedCategory)];
                var childCategories = _context.Categories.Where(cat => cat.ParentCategoryId == Guid.Parse(SelectedCategory)).Include(cat => cat.ChildCategories).Select(cat => cat.Id).ToList();
                CategoryFilters.AddRange(childCategories);
                transactionsIQ = transactionsIQ.Where(trans => CategoryFilters.Contains(trans.CategoryId));
            }

            if (!string.IsNullOrWhiteSpace(merchFilter)) {
                MerchantFilter = Guid.Parse(SelectedMerchant);
                transactionsIQ = transactionsIQ.Where(trans => trans.MerchantId == MerchantFilter);
            }

            if (!string.IsNullOrWhiteSpace(accountFilter)) {
                AccountFilter = Guid.Parse(SelectedAccount);
                transactionsIQ = transactionsIQ.Where(trans => trans.AccountId == AccountFilter);
            }

            if (MinDateFilter.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Date >= MinDateFilter.Value);
            }

            if (MaxDateFilter.HasValue) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Date <= MaxDateFilter.Value);
            }

            switch (sortOrder) {

                case "Merchant":
                    transactionsIQ = transactionsIQ.OrderBy(trans => trans.MerchantName);
                    break;
                case "merchant_desc":
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => trans.MerchantName);
                    break;
                case "Amount":
                    transactionsIQ = transactionsIQ.OrderBy(trans => Math.Abs(trans.Amount));
                    break;
                case "amount_desc":
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => Math.Abs(trans.Amount));
                    break;
                case "Date":
                    transactionsIQ = transactionsIQ.OrderBy(trans => trans.Date);
                    break;
                case "date_desc":
                default:
                    transactionsIQ = transactionsIQ.OrderByDescending(trans => trans.Date);
                    break;
            }

            transactionsIQ = transactionsIQ.Include(trans => trans.Tag);

            if (!string.IsNullOrWhiteSpace(searchText)) {
                transactionsIQ = transactionsIQ.Where(trans => trans.Notes.Contains(searchText) || trans.Account.Name.Contains(searchText) || trans.Category.Name.Contains(searchText) || trans.Amount.ToString().Contains(searchText));
            }

            var pageSize = _configuration.GetValue("PageSize", 10);
            Transactions = await PaginatedList<Transaction>.CreateAsync(transactionsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        public IActionResult OnGetMerchantAutoComplete(string text) {
            text = text.Trim();
            var merchants = _context.Merchants
                .AsNoTracking()
                .AsEnumerable()
                .Where(merch => merch.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                .Select(merch => new {
                    label = merch.Name,
                    val = merch.Id
                }).ToList();
            return new JsonResult(merchants);
        }
    }
}
